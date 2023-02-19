using System.Collections.Generic;
using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using Combat.Factory;
using Combat.Unit;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems.DroneBays
{
    public class DroneBay : SystemBase, IDroneBay
    {
        public DroneBay(IWeaponPlatform platform, IShip mothership, IDroneBayData data, ShipFactory shipFactory, ShipSettings shipSettings, IDroneReplicator replicator, IShip ship)
            : base(data.KeyBinding, data.DroneBay.ControlButtonIcon, ship)
        {
            _shipFactory = shipFactory;
            _platform = platform;
            _mothership = mothership;
            _replicator = replicator;

            var stats = data.DroneBay;

            _dronesLeft = _capacity = stats.Capacity;
            _energyCost = stats.EnergyConsumption;
            _range = stats.Range;
            _behaviour = data.Behaviour;
            _improvedAi = data.DroneBay.ImprovedAi;

            _squadron = stats.Squadron;

            var random = new System.Random();
            var builder = new ShipBuilder(data.Drone);
            builder.Bonuses.ArmorPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier);
            builder.Bonuses.ShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier);
            builder.Bonuses.EnergyShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier);
            builder.Bonuses.DamageMultiplier = StatMultiplier.FromValue(stats.DamageMultiplier);
            builder.Bonuses.VelocityMultiplier = StatMultiplier.FromValue(stats.SpeedMultiplier + (random.NextFloat() - 0.5f) * 0.4f);
            _shipSpec = builder.Build(shipSettings);

            var captainbuilder = new ShipBuilder(data.Drone);
            captainbuilder.Bonuses.ArmorPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier)*1.5f;
            captainbuilder.Bonuses.ShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier) * 1.5f;
            captainbuilder.Bonuses.EnergyShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier) * 1.5f;
            captainbuilder.Bonuses.DamageMultiplier = StatMultiplier.FromValue(stats.DamageMultiplier) * 1.5f;
            captainbuilder.Bonuses.VelocityMultiplier = StatMultiplier.FromValue(stats.SpeedMultiplier + (random.NextFloat() - 0.5f) * 0.4f) * 1.5f;
            captainbuilder.Bonuses.ScaleMultiplier = StatMultiplier.FromValue(1.5f);
            _captainshipSpec = captainbuilder.Build(shipSettings);

            var twincaptainbuilder = new ShipBuilder(data.Drone);
            twincaptainbuilder.Bonuses.ArmorPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier)*1.25f;
            twincaptainbuilder.Bonuses.ShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier) * 1.25f;
            twincaptainbuilder.Bonuses.EnergyShieldPointsMultiplier = StatMultiplier.FromValue(stats.DefenseMultiplier) * 1.25f;
            twincaptainbuilder.Bonuses.DamageMultiplier = StatMultiplier.FromValue(stats.DamageMultiplier) * 1.25f;
            twincaptainbuilder.Bonuses.VelocityMultiplier = StatMultiplier.FromValue(stats.SpeedMultiplier + (random.NextFloat() - 0.5f) * 0.4f) * 1.25f;
            twincaptainbuilder.Bonuses.ScaleMultiplier = StatMultiplier.FromValue(1.25f);
            _twincaptainshipSpec = twincaptainbuilder.Build(shipSettings);

            MaxCooldown = _platform.CooldownTime * Mathf.Max(_squadron, 1);
        }

        public override bool CanBeActivated { get { return base.CanBeActivated && _dronesLeft > Mathf.Max(_squadron - 1, 0) && _platform.IsReady && _platform.EnergyPoints.Value >= Mathf.Max(_squadron, 1) * _energyCost; } }
        
        public override float Cooldown
        {
            get
            {
                if (_squadron == 0)
                {
                    if (_dronesLeft > 0)
                        return _platform.Cooldown;
                    else if (_drones.Count >= _capacity)
                        return 1.0f;
                    else if (_replicator != null)
                        return _replicator.Cooldown;
                    else
                        return 1.0f;
                }
                else
                {
                    if (_dronesLeft >= _squadron)
                        return _platform.Cooldown;
                    else if (_drones.Count >= _capacity)
                        return 1.0f;
                    else if (_replicator != null)
                        return _replicator.Cooldown;
                    else
                        return 1.0f;
                }
            }
        }

        public float Range { get { return _range; } }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyCost))
            {
                _platform.OnShot();
                CreateDrone();
                InvokeTriggers(ConditionType.OnActivate);
            }

            _refreshCooldown -= elapsedTime;
            if (_refreshCooldown < 0)
            {
                _refreshCooldown = _refreshInterval;
                _drones.RemoveAll(item => !item.IsActive());
                var count = _drones.Count;
                if (count + _dronesLeft < _capacity && _replicator != null)
                    _replicator.Start();
            }
        }

        protected override void OnUpdateView(float elapsedTime) {}

        public bool TryRestoreDrone()
        {
            if (_drones.Count + _dronesLeft >= _capacity)
                return false;

            _dronesLeft++;
            return true;
        }

        protected override void OnDispose() {}

        private void CreateDrone()
        {
            if (_dronesLeft >= Mathf.Max(_squadron, 1))
                _dronesLeft -= Mathf.Max(_squadron, 1);

            float rotation = _platform.Body.WorldRotation() + (Random.value - 0.5f) * _platform.BaseSpread;
            if (_squadron > 0)
            {
                Vector2[] offset = new Vector2[_squadron];
                for (int i = 0; i < _squadron; i++)
                {
                    int dis = _squadron % 2 == 1 ? (i + 1) / 2 : i / 2 + 1;
                    offset[i] = new Vector2(Mathf.Cos((rotation + (i % 2 == 0 ? 120 : -120)) * Mathf.Deg2Rad), Mathf.Sin((rotation + (i % 2 == 0 ? 120 : -120)) * Mathf.Deg2Rad)) * dis;

                }
                for (int i = 0; i < _squadron; i++)
                {
                    IShip model;
                    if (_squadron % 2 == 1)
                        model = _shipFactory.CreateDrone(i == 0 ? _captainshipSpec : _shipSpec, _mothership, _range, _platform.Body.WorldPosition() + offset[i] * _shipSpec.Stats.ModelScale * 1.2f, rotation, _behaviour, _improvedAi);
                    else
                        model = _shipFactory.CreateDrone(i <= 1 ? _twincaptainshipSpec : _shipSpec, _mothership, _range, _platform.Body.WorldPosition() + offset[i] * _shipSpec.Stats.ModelScale * 1.2f, rotation, _behaviour, _improvedAi);
                    _drones.Add(model);
                }
            }
            else
            {
                var model = _shipFactory.CreateDrone(_shipSpec, _mothership, _range, _platform.Body.WorldPosition(), rotation, _behaviour, _improvedAi);
                _drones.Add(model);
            }
        }

        private int _dronesLeft;
        private float _refreshCooldown;
        private readonly bool _improvedAi;
        private readonly float _energyCost;
        private readonly int _capacity;
        private readonly int _squadron = 0;
        private readonly List<IShip> _drones = new List<IShip>();
        private readonly IWeaponPlatform _platform;
        private readonly IShipSpecification _shipSpec;
        private readonly IShipSpecification _captainshipSpec;
        private readonly IShipSpecification _twincaptainshipSpec;
        private readonly ShipFactory _shipFactory;
        private readonly IShip _mothership;
        private readonly float _range;
        private readonly DroneBehaviour _behaviour;
        private readonly IDroneReplicator _replicator;
        private const float _refreshInterval = 0.5f;
    }
}
