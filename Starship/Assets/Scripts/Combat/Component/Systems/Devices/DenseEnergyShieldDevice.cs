using Combat.Collision;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;

namespace Combat.Component.Systems.Devices
{
    public class DenseEnergyShieldDevice : SystemBase, IDevice, IFeaturesModification, IStatsModification
    {
        public DenseEnergyShieldDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _activeColor = deviceSpec.Color;
            _energyCost = deviceSpec.EnergyConsumption;
        }

        public override bool CanBeActivated { get { return base.CanBeActivated && (_isEnabled || _ship.Stats.Energy.Value >= _energyCost); } }

        public override IStatsModification StatsModification { get { return this; } }
        public bool TryApplyModification(ref Resistance data)
        {
            if (_isEnabled)
            {
                data.EnergyShieldEnergy = 0.3f + data.EnergyShieldEnergy * 0.7f;
                data.EnergyShieldHeat = 0.3f + data.EnergyShieldHeat * 0.7f;
                data.EnergyShieldKinetic = 0.3f + data.EnergyShieldKinetic * 0.7f;
                data.EnergyShieldQuantum = 0.3f + data.EnergyShieldQuantum * 0.7f;
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
            if (Active && CanBeActivated && _ship.Stats.Energy.TryGet(_energyCost*elapsedTime))
            {
                if (!_isEnabled)
                {
                    InvokeTriggers(ConditionType.OnActivate);
                    _isEnabled = true;
                }
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

        private bool _isEnabled;
        private Color _color = Color.white;
        private readonly Color _activeColor;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
