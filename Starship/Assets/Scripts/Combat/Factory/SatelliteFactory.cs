using Combat.Collision.Behaviour;
using Combat.Collision.Behaviour.Action;
using Combat.Component.Body;
using Combat.Component.Collider;
using Combat.Component.Controller;
using Combat.Component.DamageHandler;
using Combat.Component.Physics;
using Combat.Component.Satellite;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Helpers;
using Combat.Scene;
using Combat.Unit.Auxiliary;
using Constructor;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Audio;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class SatelliteFactory
    {
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly IScene _scene;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly PrefabCache _prefabCache;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public IUnit CreateSatellite(IShip ship, IWeaponPlatformData data, float cooldown)
        {
            var satelliteData = data.Companion;
            var offset = Vector2.zero;
            var rotation = 0f;
            if (satelliteData.Formation != null)
            {
                offset = satelliteData.Formation.Position;
                rotation = satelliteData.Formation.Rotation;
            }
            GameObjectHolder gameObject;

            var prefab = _prefabCache.LoadResourcePrefab("Combat/Satellites/" + satelliteData.Satellite.ModelImage.Id);

            if (prefab != null)
            {
                gameObject = new GameObjectHolder(prefab, _objectPool);
            }
            else
            {
                prefab = _prefabCache.LoadResourcePrefab("Combat/Satellites/Default");
                gameObject = new GameObjectHolder(prefab, _objectPool);
                var sprite = _resourceLocator.GetSprite(satelliteData.Satellite.ModelImage);
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }

            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, ship.Body.WorldPosition(), ship.Body.WorldRotation(), satelliteData.Satellite.ModelScale, Vector2.zero, 0, satelliteData.Weight);

            var view = gameObject.GetComponent<IView>();
            var collider = gameObject.GetComponent<ICollider>();

            var satellite = new Satellite(new UnitType(UnitClass.Drone, ship.Type.Side, ship), body, view, collider);
            satellite.AddResource(gameObject);

            Vector2[] position=new Vector2[4];
            float minAngle;
            float maxAngle;

            var satellitescale = satelliteData.Satellite.ModelScale;
            var shipscale = ship.Body.Scale;

            if (satelliteData.Location == CompanionLocation.Left)
            {
                position[0] = new Vector2(0, 0.75f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.5f, 0.5f) * shipscale * (1 + satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(1f, 0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(1.2f, 0.1f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right)
            {
                position[0] = new Vector2(0, -0.75f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.5f, -0.5f) * shipscale * (1 + satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(1f, -0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(1.2f, -0.1f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_2)
            {
                position[0] = new Vector2(0.44f, 0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.6f, 0.6f) * shipscale * (1 + 3 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0, 0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.6f, 0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_2)
            {
                position[0] = new Vector2(-0.44f, -0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.6f, -0.6f) * shipscale * (1 + 3 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0f, -0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.6f, -0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_3)
            {
                position[0] = new Vector2(-0.44f, 0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.7f, 0.7f) * shipscale * (1 + 5 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-1f, 0.7f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0, 0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_3)
            {
                position[0] = new Vector2(0.44f, -0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.7f, -0.7f) * shipscale * (1 + 5 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-1f, -0.7f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0f, -0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_4)
            {
                position[0] = new Vector2(0.71f, 0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.8f, 0.8f) * shipscale * (1 + 7 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0.5f, 0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.9f, 0.2f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_4)
            {
                position[0] = new Vector2(-0.71f, -0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.8f, -0.8f) * shipscale * (1 + 7 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0.5f, -0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.9f, -0.2f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_5)
            {
                position[0] = new Vector2(-0.71f, 0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.9f, 0.9f) * shipscale * (1 + 9 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-0.5f, 0.6f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.3f, 0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else
            {
                position[0] = new Vector2(0.71f, -0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.9f, -0.9f) * shipscale * (1 + 9 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-0.5f, -0.6f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.3f, -0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }

            var isTurret = (bool)data.Image;

            if (data.AutoAimingArc < 10 || isTurret)
                satellite.Controller = new SatelliteControllser(ship, satellite, gameObject ,position, rotation);
            else
                satellite.Controller = new AutoAimingSatelliteController(ship, satellite, gameObject, position, rotation, minAngle, maxAngle, _scene);

            satellite.AddTrigger(new DroneExplosionAction(satellite, _effectFactory, _soundPlayer));

            gameObject.IsActive = false;
            _scene.AddUnit(satellite);

            return satellite;
        }

        public IUnit CreateUAVSatellite(IShip ship, IUAVPlatformData data, float cooldown)
        {
            var satelliteData = data.Companion;
            var offset = data.Position;
            var rotation = data.Rotation;

            GameObjectHolder gameObject;

            var prefab = _prefabCache.LoadResourcePrefab("Combat/Satellites/" + satelliteData.Satellite.ModelImage.Id);

            if (prefab != null)
            {
                gameObject = new GameObjectHolder(prefab, _objectPool);
            }
            else
            {
                prefab = _prefabCache.LoadResourcePrefab("Combat/Satellites/Default");
                gameObject = new GameObjectHolder(prefab, _objectPool);
                var sprite = _resourceLocator.GetSprite(satelliteData.Satellite.ModelImage);
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }

            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, ship.Body.WorldPosition(), ship.Body.WorldRotation(), satelliteData.Satellite.ModelScale, Vector2.zero, 0, satelliteData.Weight);

            var view = gameObject.GetComponent<IView>();
            var collider = gameObject.GetComponent<ICollider>();

            var satellite = new Satellite(new UnitType(UnitClass.Drone, ship.Type.Side, ship), body, view, collider);
            satellite.AddResource(gameObject);

            Vector2[] position = new Vector2[4];
            float minAngle;
            float maxAngle;

            var satellitescale = satelliteData.Satellite.ModelScale;
            var shipscale = ship.Body.Scale;

            if (satelliteData.Location == CompanionLocation.Left)
            {
                position[0] = new Vector2(0, 0.75f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.5f, 0.5f) * shipscale * (1 + satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(1f, 0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(1.2f, 0.1f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right)
            {
                position[0] = new Vector2(0, -0.75f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.5f, -0.5f) * shipscale * (1 + satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(1f, -0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(1.2f, -0.1f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_2)
            {
                position[0] = new Vector2(0.44f, 0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.6f, 0.6f) * shipscale * (1 + 3 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0, 0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.6f, 0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_2)
            {
                position[0] = new Vector2(-0.44f, -0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.6f, -0.6f) * shipscale * (1 + 3 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0f, -0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.6f, -0.3f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_3)
            {
                position[0] = new Vector2(-0.44f, 0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.7f, 0.7f) * shipscale * (1 + 5 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-1f, 0.7f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0, 0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_3)
            {
                position[0] = new Vector2(0.44f, -0.61f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.7f, -0.7f) * shipscale * (1 + 5 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-1f, -0.7f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0f, -0.5f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_4)
            {
                position[0] = new Vector2(0.71f, 0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.8f, 0.8f) * shipscale * (1 + 7 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0.5f, 0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.9f, 0.2f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else if (satelliteData.Location == CompanionLocation.Right_4)
            {
                position[0] = new Vector2(-0.71f, -0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.8f, -0.8f) * shipscale * (1 + 7 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(0.5f, -0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.9f, -0.2f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }
            else if (satelliteData.Location == CompanionLocation.Left_5)
            {
                position[0] = new Vector2(-0.71f, 0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.9f, 0.9f) * shipscale * (1 + 9 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-0.5f, 0.6f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.3f, 0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -10;
                maxAngle = 190;
            }
            else
            {
                position[0] = new Vector2(0.71f, -0.23f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[1] = new Vector2(-0.9f, -0.9f) * shipscale * (1 + 9 * satellitescale / shipscale) + offset * satellitescale;
                position[2] = new Vector2(-0.5f, -0.6f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                position[3] = new Vector2(0.3f, -0.4f) * shipscale * (1 + 2 * satellitescale / shipscale) + offset * satellitescale;
                minAngle = -190;
                maxAngle = 10;
            }

            satellite.Controller = new SatelliteControllser(ship, satellite, gameObject, position, rotation);

            satellite.AddTrigger(new DroneExplosionAction(satellite, _effectFactory, _soundPlayer));

            gameObject.IsActive = true;
            _scene.AddUnit(satellite);

            return satellite;
        }

        public IAuxiliaryUnit CreateRepairBot(IShip ship, float repairRate, float size, float weight, float hitPoints, Color color, AudioClipId activationSound, float toward = 0)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/RepairBot");
            var gameObject = new GameObjectHolder(prefab, _objectPool);

            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(null, ship.Body.WorldPosition(), ship.Body.WorldRotation(), size, Vector2.zero, 0, weight);

            var view = gameObject.GetComponent<IView>();
            var collider = gameObject.GetComponent<ICollider>();

            var repairBot = new RepairBot(ship, body, view, collider, hitPoints);
            var radius = ship.Body.Scale + 1f + size;

            repairBot.Controller = new RepairBotContoller(ship, repairBot, radius, repairRate, toward);
            repairBot.AddResource(gameObject);

            repairBot.AddTrigger(new DroneExplosionAction(repairBot, _effectFactory, _soundPlayer));

            var effect = _effectFactory.CreateEffect("Laser", body);
            effect.Position = new Vector2(size / 2, 0);
            effect.Rotation = 0;
            effect.Color = color;
            effect.Size = radius - size;

            repairBot.AddTrigger(new UnitEffect(effect, 0.1f, ConditionType.OnActivate, ConditionType.OnDeactivate));

            if (activationSound)
                repairBot.AddTrigger(new SoundEffect(_soundPlayer, activationSound, ConditionType.OnActivate, ConditionType.OnDeactivate));

            gameObject.IsActive = true;
            _scene.AddUnit(repairBot);

            return repairBot;
        }

        public IAuxiliaryUnit CreatePointDefense(IShip ship, float energyConsumption, float radius, float damage, float cooldown, AudioClipId activationSound, Color color)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Objects/PointDefense");
            var gameObject = new GameObjectHolder(prefab, _objectPool);

            var body = gameObject.GetComponent<IBodyComponent>();
            body.Initialize(ship.Body, Vector2.zero, 0, 2*radius/ship.Body.Scale, Vector2.zero, 0f, 0.1f);
            var view = gameObject.GetComponent<IView>();
            var collider = gameObject.GetComponent<ICollider>();
            collider.MaxRange = radius;

            var collisionBehaviour = new BulletCollisionBehaviour();
            collisionBehaviour.AddAction(new PointDefenseCollisionAction(damage, _effectFactory, color));

            var pointDefense = new PointDefenseShield(ship, body, view, collider, collisionBehaviour, energyConsumption, cooldown);
            pointDefense.AddResource(gameObject);

            pointDefense.AddTrigger(new SoundEffect(_soundPlayer, activationSound, ConditionType.OnActivate));

            gameObject.IsActive = true;
            _scene.AddUnit(pointDefense);

            return pointDefense;
        }

        public IAuxiliaryUnit CreateEnergyShield(IShip ship, float energyConsumption, float size, Color color)
        {
            return CreateEnergyShield(ship, energyConsumption, size, Vector2.zero, color, 0.3f, "Combat/Objects/EnergyShield");
        }

        public IAuxiliaryUnit CreateFrontalShield(IShip ship, float energyConsumption, Vector2 offset, float size, Color color)
        {
            return CreateEnergyShield(ship, energyConsumption, size, offset / ship.Body.Scale, color, 1.0f, "Combat/Objects/FrontalShield");
        }

        private IAuxiliaryUnit CreateEnergyShield(IShip ship, float energyConsumption, float size, Vector2 offset, Color color, float defaultOpacity, string prefabName)
        {
            var prefab = _prefabCache.LoadResourcePrefab(prefabName);
            var gameObject = new GameObjectHolder(prefab, _objectPool);
            gameObject.IsActive = true;

            var body = gameObject.GetComponent<IBodyComponent>();
            var position = offset*ship.Body.WorldScale();
            body.Initialize(ship.Body, position, 0, size, Vector2.zero, 0f, 0f);

            var view = gameObject.GetComponent<IView>();
            view.Color = color;

            var collider = gameObject.GetComponent<ICollider>();

            //var physics = gameObject.GetComponent<PhysicsManager>();

            var energyShield = new EnergyShield(ship, body, view, collider, defaultOpacity);
            energyShield.DamageHandler = new EnergyShieldDamageHandler(energyShield, energyConsumption);

            energyShield.AddResource(gameObject);

            _scene.AddUnit(energyShield);

            return energyShield;
        }
    }
}
