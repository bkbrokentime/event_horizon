using System.Linq;
using Combat.Ai;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controls;
using Combat.Component.Engine;
using Combat.Component.Physics;
using Combat.Component.Platform;
using Combat.Component.Stats;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Unit.Classification;
using Combat.Component.Triggers;
using Combat.Component.View;
using Combat.Helpers;
using Combat.Scene;
using Constructor;
using Constructor.Model;
using GameDatabase;
using GameDatabase.Enums;
using Services.Audio;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using Zenject;
using IShip = Combat.Component.Ship.IShip;
using Ship = Combat.Component.Ship.Ship;
using GameDatabase.Model;
using Combat.Unit;
using Combat.Unit.Ship.Effects.Special;

namespace Combat.Factory
{
    public class ShipFactory
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly IAiManager _aiManager;
        [Inject] private readonly IScene _scene;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly WeaponFactory _weaponFactory;
        [Inject] private readonly DeviceFactory _deviceFactory;
        [Inject] private readonly DroneBayFactory _droneBayFactory;
        [Inject] private readonly SatelliteFactory _satelliteFactory;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly PrefabCache _prefabCache;

        public ShipFactory(Settings settings)
        {
            _settings = settings;
        }

        private Ship CreateShip(
            IShipSpecification spec,
            IControllerFactory controllerFactory,
            Vector2 position,
            float rotation,
            IShip motherShip,
            UnitSide unitSide,
            bool createShadow,
            bool spacejump)
        {
            UnityEngine.Debug.Log("CreateShip: " + spec.Type.Id);

            bool isDrone = motherShip != null;

            if (isDrone)
                spec.isdrone = true;
            else
                spec.isdrone = false;

            var stats = spec.Stats;
            var shipStats = new Component.Stats.ShipStats(spec);
            var hasjoint = false;
            var wormtailicon = SpriteId.Empty;
            var wormoffset = new Vector2(-0.415f, 0);
            foreach (var item in spec.Devices)
            {
                if (item.Device.DeviceClass == DeviceClass.WormTail)
                {
                    hasjoint = true;
                    wormtailicon = item.Device.SecondObjectIconImage;
                    wormoffset = item.Device.SecondObjectOffset;
                }
            }
            var shipGameObject = CreateShipObject(stats, spec.Stats.ShipColor, shipStats, hasjoint, wormtailicon, wormoffset);
            var body = CreateBody(shipGameObject, stats, position, rotation);
            var collider = CreateCollider(shipGameObject);
            var view = CreateView(shipGameObject);
            var physics = shipGameObject.GetComponent<PhysicsManager>();

            var ship = isDrone ?
                new Ship(spec, motherShip, body, view, shipStats, collider, physics) :
                new Ship(spec, unitSide, body, view, shipStats, collider, physics);

            ship.AddResource(shipGameObject);

            if (!_settings.NoDamageIndicator && !isDrone)
                shipStats.DamageIndicator = new DamageIndicator(ship, _effectFactory, unitSide == UnitSide.Player ? 0.75f : 0.5f);

            ship.Engine = CreateEngine(stats);
            ship.Controls = new CommonControls();

            CreateEngineEffect(ship, stats, isDrone ? "DroneTrail" : "ShipTrail");

            if (createShadow)
                ship.AddTrigger(CreateShadow(ship));

            if (stats.ModelScale < 0.9f)
                ship.AddTrigger(new DroneExplosionAction(ship, _effectFactory, _soundPlayer));
            else
            {
                ship.AddTrigger(new ShipExplosionAction(ship, _effectFactory, _soundPlayer));
                ship.AddTrigger(new ShipWreckAction(ship, _effectFactory, _resourceLocator.GetSprite(stats.ModelImage), spec.Stats.ShipColor.Color, _settings.StaticWrecks));
            }

            if (spec.Stats.ShieldPoints > 0)
                ship.AddTrigger(CreateShield(ship, stats.EngineColor));

            foreach (var item in spec.Platforms)
            {
                if (item.Companion == null && !item.Weapons.Any() && !item.WeaponsObsolete.Any())
                    continue;

                var platform = CreatePlatform(ship, item, 0.02f, spec.Stats.TurretColor);
                ship.AddPlatform(platform);

                foreach (var weaponSpec in item.Weapons)
                {
                    var weapon = _weaponFactory.Create(weaponSpec, platform, spec.Stats.ArmorMultiplier.Value, ship);
                    ship.AddSystem(weapon);
                }

                foreach (var weaponSpec in item.WeaponsObsolete)
                {
                    var weapon = _weaponFactory.Create(weaponSpec, platform, spec.Stats.ArmorMultiplier.Value, ship);
                    ship.AddSystem(weapon);
                }
            }

            foreach (var item in spec.Devices)
            {
                if (isDrone && item.Device.DeviceClass == DeviceClass.ClonningCenter) continue; // TODO: 

                if (item.Device.DeviceClass == DeviceClass.EnergyShield || item.Device.DeviceClass == DeviceClass.PartialShield)
                {
                    if (!ship.Stats.EnergyShield.Exists)
                        continue;
                }
                var device = _deviceFactory.Create(item, ship, spec);
                if (device != null)
                    ship.AddSystem(device);
            }

            if (spec.DroneBays.Any() && motherShip == null)
            {
                IDroneReplicator droneReplicator = null;
                if (spec.Stats.DroneBuildSpeed > 0)
                {
                    droneReplicator = new DroneReplicator(ship, spec.Stats.DroneBuildSpeed);
                    ship.AddSystem(droneReplicator);
                }
                if (spec.UAVPlatforms.Count() > 0) 
                {
                    IWeaponPlatform[] datas = new IWeaponPlatform[spec.UAVPlatforms.Count()];
                    var num = 0;
                    foreach (var item in spec.UAVPlatforms)
                    {
                        var platform = CreateUAVPlatform(ship, item, _droneBayPlatformCooldown, spec.Stats.TurretColor);
                        ship.AddPlatform(platform);
                        datas[num++] = platform;
                    }
                    num = 0;
                    foreach (var drone in spec.DroneBays)
                    {
                        var droneBay = _droneBayFactory.Create(datas[num++], drone, ship, droneReplicator);
                        ship.AddSystem(droneBay);
                        if (num >= spec.UAVPlatforms.Count())
                            num = 0;
                    }
                }
                else
                {
                    var platformBody = SimpleBody.Create(ship.Body, Vector2.zero, 0f, 1.0f, 0, 0);
                    var droneBayPlatform = new FixedPlatform(ship, platformBody, _droneBayPlatformCooldown, 0, 0, Vector2.zero, 0, Vector2.zero);
                    ship.AddPlatform(droneBayPlatform);

                    foreach (var drone in spec.DroneBays)
                    {
                        var droneBay = _droneBayFactory.Create(droneBayPlatform, drone, ship, droneReplicator);
                        ship.AddSystem(droneBay);
                    }
                }
            }

            if (spacejump)
            {
                shipGameObject.IsActive = false;
                ship.Stats.SpaceJump = true;
                var shipobj = CreateShipObject(stats, spec.Stats.ShipColor, shipStats, false, SpriteId.Empty, Vector2.zero, false);
                var shipbody = CreateBody(shipobj, stats, position, rotation);
                //var shipcollider = CreateCollider(shipobj);
                var shipview = CreateView(shipobj);
                shipobj.IsActive = false;

                var chargeEffect = new ShipRetreatingEffect(ship, _effectFactory, ConditionType.OnActivate, ConditionType.OnDeactivate);
                var warpEffect = new ShipWarpEffect(ship, _effectFactory, _soundPlayer,null, ConditionType.OnDeactivate);
                //var soundEffect = new SoundLoopEffect(_soundPlayer, null, ConditionType.OnActivate, ConditionType.OnDeactivate);
                ship.AddEffect(new ShipSpaceJumpEffect((2.0f + ship.Body.WorldScale() / 20) * (1.5f - ship.Specification.Stats.EngineMAXPower / 50), shipobj.GameObject, shipGameObject.GameObject, ship, warpEffect, chargeEffect));

            }
            else
            {
                shipGameObject.IsActive = true;
                if (isDrone)
                    ship.Body.ApplyAcceleration(new Vector2(Mathf.Cos(ship.Body.Rotation * Mathf.Deg2Rad), Mathf.Sin(ship.Body.Rotation * Mathf.Deg2Rad)) * ship.Specification.Stats.EngineMAXPower * 1.5f);
            }

            _scene.AddUnit(ship);
            _aiManager.Add(controllerFactory.Create(ship));

            if (stats.Autopilot)
                _aiManager.Add(new Computer.Factory(_scene, 100, true).Create(ship));

            return ship;
        }

        public Ship CreateEnemyShip(IShipSpecification spec, Vector2 position, float rotation, int aiLevel)
        {
            return CreateShip(spec, new Computer.Factory(_scene, aiLevel), UnitSide.Enemy, position, rotation);
        }

        public Ship CreateClone(IShipSpecification spec, Vector2 position, float rotation, IShip motherShip)
        {
            return CreateShip(spec, new Clone.Factory(_scene), position, rotation, motherShip, UnitSide.Undefined, _settings.Shadows, true);
        }

        public Ship CreateShip(IShipSpecification spec, IControllerFactory controllerFactory, UnitSide side, Vector2 position, float rotation)//
        {
            return CreateShip(spec, controllerFactory, position, rotation, null, side, _settings.Shadows, true);
        }

        public IShip CreateDrone(IShipSpecification spec, IShip mothership, float range, Vector2 position, float rotation, DroneBehaviour behaviour, bool improvedAi)
        {
            return CreateShip(spec, new Drone.Factory(_scene, range, behaviour, improvedAi), position, rotation, mothership, UnitSide.Undefined, _settings.Shadows, false);
        }

        public Ship CreateStarbase(IShipSpecification spec, Vector2 position, float rotation, UnitSide unitSide)
        {
            var ship = CreateShip(spec, new Starbase.Factory(_scene, true), position, rotation, null, unitSide, _settings.Shadows, true);
            ship.Engine = new StarbaseEngine(10f);
            return ship;
        }

        public Ship CreateTurret(IShipSpecification spec, Vector2 position, float rotation, UnitSide side, bool isCombatMode)
        {
            var ship = CreateShip(spec, new Starbase.Factory(_scene, isCombatMode), position, rotation, null, side, false, false);
            ship.Engine = new NullEngine();
            return ship;
        }

        public GameObjectHolder CreateShipObject(IShipStats stats, ColorScheme colorScheme, ShipStats shipStats, bool hasjoint, SpriteId jointicon, Vector2 wormoffset, bool hascollider = true)
        { 
            GameObjectHolder gameObject;
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Ships/" + stats.ModelImage.Id, true);
            if (prefab != null)
            {
                gameObject = new GameObjectHolder(prefab, _objectPool);
            }
            else
            {
                if (!hasjoint)
                    prefab = _prefabCache.LoadResourcePrefab("Combat/Ships/Default");
                else
                    prefab = _prefabCache.LoadResourcePrefab("Combat/Ships/Default_worm_head");

                gameObject = new GameObjectHolder(prefab, _objectPool);
                if (hasjoint)
                {
                    gameObject.GetComponent<ChangeView>().ChangeableSprite[0].sprite = _resourceLocator.GetSprite(new SpriteId(jointicon.Id, SpriteId.Type.Ship));
                    gameObject.GetComponent<ChangeView>().ChangeableSprite[0].gameObject.transform.localPosition = wormoffset;
                }
                var sprite = _resourceLocator.GetSprite(stats.ModelImage);
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                if (hascollider)
                {
                    if (stats.ShipCategory == ShipCategory.Drone)
                        gameObject.AddComponent<CircleCollider2D>();
                    else
                        gameObject.AddComponent<PolygonCollider2D>();
                }
            }

            if (colorScheme.IsHsv)
                gameObject.GetComponent<IView>().ApplyHsv(colorScheme.Hue, colorScheme.Saturation);

            gameObject.GetComponent<ShipStatsShow>().stats = stats;
            gameObject.GetComponent<ShipStatsShow>().shipStats = shipStats;

            return gameObject;
        }

        private void CreateEngineEffect(Ship ship, IShipStats model, string effectType = "ShipTrail")
        {
            foreach (var engine in model.Engines)
            {
                ship.AddTrigger(CreateEngineLight(ship, engine.Position * 0.5f, 0f, 5 * engine.Size / model.ModelScale, model.EngineColor));
                ship.AddTrigger(CreateTrail(ship, engine.Position * 0.5f, engine.Size, model.EngineColor, effectType));
            }
        }

        public IEngine CreateEngine(IShipStats stats, bool isDrone = false)
        {
            var propulsion = stats.EnginePower;
            var turnRate = stats.TurnRate * Mathf.Rad2Deg;

            var velocity = stats.EngineMAXPower;
            var angularVelocity = stats.TurnMAXRate * Mathf.Rad2Deg;

            var settings = _database.ShipSettings;
            var maxVelocity = settings.MaxVelocity;
            var maxAngularVelocity = settings.MaxTurnRate * 36f;

            /*
            var propulsion = 2 * stats.EnginePower / Mathf.Sqrt(stats.Weight);
            var turnRate = stats.TurnRate * 120 / stats.Weight;
            var velocity = stats.EnginePower;
            var angularVelocity = stats.TurnRate * 30f;

            var settings = _database.ShipSettings;
            var maxVelocity = settings.MaxVelocity;
            var maxAngularVelocity = settings.MaxTurnRate * 30f;
            */
            if (isDrone)
                return new DroneEngine(propulsion, turnRate, velocity, angularVelocity, maxVelocity, maxAngularVelocity);
            else
                return new ShipEngine(propulsion, turnRate, velocity, angularVelocity, maxVelocity, maxAngularVelocity);
        }

        public static IBody CreateBody(GameObjectHolder gameObjectHolder, IShipStats stats, Vector2 position, float rotation)
        {
            var body = gameObjectHolder.GetComponent<IBodyComponent>();
            body.Initialize(null, position, rotation, stats.ModelScale, Vector2.zero, 0f, stats.Weight);
            return body;
        }

        public static IView CreateView(GameObjectHolder gameObjectHolder)
        {
            var view = gameObjectHolder.GetComponent<IView>();
            return view;
        }

        public static ICollider CreateCollider(GameObjectHolder gameObjectHolder)
        {
            var collider = gameObjectHolder.GetComponent<ICollider>();
            return collider;
        }

        private ShieldEffect CreateShield(IShip ship, Color color)
        {
            var effect = _effectFactory.CreateEffect("MainShield", ship.Body);
            effect.Color = color;

            return new ShieldEffect(effect, ship);
        }

        private EngineLightEffect CreateEngineLight(IShip ship, Vector2 position, float rotation, float size, Color color)
        {
            var effect = _effectFactory.CreateEffect("EngineLight", ship.Body);
            effect.Position = position;
            effect.Rotation = rotation;
            effect.Size = size;
            effect.Color = color;

            return new EngineLightEffect(ship, effect);
        }

        private ConstantEffect CreateShadow(IShip ship)
        {
            var effect = _effectFactory.CreateEffect("Shadow", ship.Body);
            return new ConstantEffect(effect);
        }

        private EngineLightEffect CreateTrail(IShip ship, Vector2 position, float size, Color color, string effectType)
        {
            var effect = _effectFactory.CreateEffect(effectType, ship.Body);
            effect.Position = position;
            effect.Size = size;
            effect.Color = color;
            return new EngineLightEffect(ship, effect);
        }
        
        private IWeaponPlatform CreatePlatform(Ship ship, IWeaponPlatformData data, float cooldown, ColorScheme color)
        {
            var parent = data.Companion == null ? ship : _satelliteFactory.CreateSatellite(ship, data, cooldown);
            var rotation = data.Rotation;
            var position = data.MoveSpeed == 0 ? data.Position * 0.5f : new Vector2(data.MoveCenterPosition.x * 0.5f + data.MoveCenterRange.x * 0.5f * Mathf.Cos(rotation * Mathf.Deg2Rad), data.MoveCenterPosition.y * 0.5f + data.MoveCenterRange.y * 0.5f * Mathf.Sin(rotation * Mathf.Deg2Rad));
            var offset = (data.Offset + data.Size) * 0.5f;
            var isTurret = (bool)data.Image && (data.Weapons.Any() || data.WeaponsObsolete.Any());

            IWeaponPlatform platform;

            if (data.AutoAimingArc > 0)
            {
                platform = new AutoAimingPlatform(ship, parent, _scene, position, rotation, offset, data.AutoAimingArc, cooldown, data.RotationSpeed, data.MoveCenterPosition, data.MoveSpeed, data.MoveCenterRange);
            }
            else
            {
                var body = SimpleBody.Create(parent.Body, position, rotation, 1f / parent.Body.Scale, 0, offset);
                platform = new FixedPlatform(ship, body, cooldown, rotation, 0, data.MoveCenterPosition, data.MoveSpeed, data.MoveCenterRange);
            }

            if (isTurret)
            {
                var prefab = _prefabCache.LoadResourcePrefab("Combat/Guns/turret");
                var gameObject = new GameObjectHolder(prefab, _objectPool);
                var sprite = _resourceLocator.GetSprite(data.Image);
                var x = sprite.pivot.x / sprite.rect.width;
                var y = sprite.pivot.y / sprite.rect.height;
                var scale = 0.5f / Mathf.Max(Mathf.Max(x, 1f - x), Mathf.Max(y, 1f - y));
                var view = gameObject.GetComponent<IView>();
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                if (color.IsHsv) view.ApplyHsv(color.Hue, color.Saturation);
                platform.SetView(view, color.Color);
                gameObject.GetComponent<IBodyComponent>().Initialize(platform.Body, new Vector2(-0.5f * data.Size * parent.Body.Scale, 0), 0, scale * data.Size * parent.Body.Scale, Vector2.zero, 0, 0);
                gameObject.IsActive = true;
                ship.AddResource(gameObject);
            }

            return platform;
        }
        private IWeaponPlatform CreateUAVPlatform(Ship ship, IUAVPlatformData data, float cooldown, ColorScheme color)
        {
            var parent = data.Companion == null ? ship : _satelliteFactory.CreateUAVSatellite(ship, data, cooldown);
            var position = data.Position * 0.5f;
            var rotation = data.Rotation;
            var spread = data.BaseSpread;

            IWeaponPlatform platform;

            var body = SimpleBody.Create(parent.Body, position, rotation, 1f / parent.Body.Scale, 0, 0);
            platform = new FixedPlatform(ship, body, cooldown, rotation, spread, Vector2.zero, 0, Vector2.zero);

            return platform;
        }

        private readonly Settings _settings;
        private const float _droneBayPlatformCooldown = 1.5f;

        public struct Settings
        {
            public bool Shadows;
            public bool StaticWrecks;
            public bool NoDamageIndicator;
        }


        public struct _createship
        {
            public float settime;
        }

    }
}
