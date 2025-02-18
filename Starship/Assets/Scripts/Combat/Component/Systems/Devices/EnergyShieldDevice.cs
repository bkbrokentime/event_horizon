﻿using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class EnergyShieldDevice : SystemBase, IDevice, IFeaturesModification
    {
        public EnergyShieldDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && (_isEnabled || _ship.Stats.Energy.Value >= _energyCost); } }

        public override IFeaturesModification FeaturesModification { get { return this; } }
        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Invulnerable |= _isEnabled;
            return true;
        }

        public void Deactivate()
        {
            if (!_isEnabled)
                return;

            _isEnabled = false;
            TimeFromLastUse = 0;
            InvokeTriggers(ConditionType.OnDeactivate);
        }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && !_isEnabled && CanBeActivated)
            {
                if (_ship.Stats.EnergyShield.Value >= _ship.Stats.EnergyShield.MaxValue * 0.2f)
                    if (_ship.Stats.Energy.TryGet(_energyCost))
                    {
                        InvokeTriggers(ConditionType.OnActivate);
                        _isEnabled = true;
                    }
            }
            else if (Active && _isEnabled && CanBeActivated)
            {
            }
            else if (_isEnabled)
            {
                Deactivate();
            }
        }

        protected override void OnUpdateView(float elapsedTime)
        {
        }

        protected override void OnDispose() { }

        private bool _isEnabled;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
