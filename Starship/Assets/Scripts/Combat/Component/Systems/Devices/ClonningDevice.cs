﻿using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Factory;
using Combat.Unit;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class ClonningDevice : SystemBase, IDevice
    {
        public ClonningDevice(IShip mothership, DeviceStats stats, ShipFactory shipFactory, IShipSpecification shipSpec, EffectFactory effectFactory, int keyBinding, IShip ship)
            : base(keyBinding, stats.ControlButtonIcon, ship)
        {
            _effectFactory = effectFactory;
            _effectPrefab = stats.EffectPrefab;

            MaxCooldown = stats.Cooldown;
            TimeFromLastUse = MaxCooldown * 0.75f;

            _shipFactory = shipFactory;
            _mothership = mothership;
            _shipSpec = shipSpec;
 
            _energyCost = stats.EnergyConsumption;
        }

        public override bool CanBeActivated => _clone == null && base.CanBeActivated;
        public override float Cooldown => _clone == null ? base.Cooldown : 1.0f;

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _mothership.Stats.Energy.TryGet(_energyCost))
            {
                SpawnShip();
                InvokeTriggers(ConditionType.OnActivate);
            }

            if (_clone != null && _clone.State == UnitState.Destroyed)
            {
                _clone = null;
                TimeFromLastUse = 0;
            }
        }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnDispose() {}

        private void SpawnShip()
        {
            if (_clone != null)
            {
                _clone.Vanish();
                _clone = null;
            }

            var random = new System.Random(GetHashCode());
            var rotation = random.Next(360);
            var direction = RotationHelpers.Direction(rotation);
            var position = _mothership.Body.WorldPosition() + _mothership.Body.Scale * direction * random.Next(1, 4);
            _clone = _shipFactory.CreateClone(_shipSpec, position, rotation, _mothership);
            _clone.Body.ApplyAcceleration(_mothership.Body.Velocity);

            _effectFactory.CreateEffect(_effectPrefab, _clone.Body)?.Run(0.5f, Vector2.zero, 0);
        }

        public void Deactivate() {}

        private IShip _clone;
        private readonly PrefabId _effectPrefab;
        private readonly EffectFactory _effectFactory;
        private readonly IShipSpecification _shipSpec;
        private readonly ShipFactory _shipFactory;
        private readonly IShip _mothership;
        private readonly float _energyCost;
    }
}
