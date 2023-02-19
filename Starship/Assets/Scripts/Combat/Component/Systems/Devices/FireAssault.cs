using Combat.Collision;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using Combat.Factory;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class FireAssault : SystemBase, IDevice, IFeaturesModification, IStatsWeaponModification
    {
        public FireAssault(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _activeColor = deviceSpec.Color;
            _energyCost = deviceSpec.EnergyConsumption;
            _lifetime = deviceSpec.Lifetime;
            _power= deviceSpec.Power;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && (_isEnabled || _ship.Stats.Energy.Value >= _energyCost); } }
        public override IStatsWeaponModification StatsWeaponModification { get { return this; } }
        public bool TryApplyModification(ref WeaponUpgrade data)
        {
            if (_isEnabled)
            {
                data.DamageMultiplier *= _power + 1f;
                data.FireRateMultiplier *= _power + 1f;
            }
            return true;
        }
        public override IFeaturesModification FeaturesModification { get { return this; } }
        public bool TryApplyModification(ref FeaturesData data)
        {
            data.Color *= _color;
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
            if (Active && !_isEnabled && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost))
            {
                InvokeTriggers(ConditionType.OnActivate);
                _timeLeft = _lifetime;
                _isEnabled = true;
            }
            else if (Active && _timeLeft > 0 && _isEnabled && _ship.Stats.Energy.TryGet(_energyCost * elapsedTime))
            {
                // InvokeTriggers(SystemConditionType.OnRemainActive);
                _timeLeft -= elapsedTime;
            }
            else if (_isEnabled)
            {
                Deactivate();
            }
        }

        protected override void OnUpdateView(float elapsedTime)
        {
            var color = _isEnabled ? _activeColor : Color.white;
            _color = Color.Lerp(_color, color, 5 * elapsedTime);
        }

        protected override void OnDispose() { }

        private float _timeLeft;
        private bool _isEnabled;
        private Color _color = Color.white;
        private readonly float _lifetime;
        private readonly Color _activeColor;
        private readonly float _energyCost;
        private readonly float _power;
        private readonly IShip _ship;
    }
}
