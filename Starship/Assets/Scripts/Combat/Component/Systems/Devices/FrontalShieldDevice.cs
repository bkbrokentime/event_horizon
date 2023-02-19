using Combat.Component.Ship;
using Combat.Component.Triggers;
using GameDatabase.DataModel;

namespace Combat.Component.Systems.Devices
{
    public class FrontalShieldDevice : SystemBase, IDevice
    {
        public FrontalShieldDevice(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _energyCost = deviceSpec.EnergyConsumption;
            _power = deviceSpec.Power;
        }

        public override bool CanBeActivated { get { return base.CanBeActivated && (_isEnabled || _ship.Stats.Energy.Value >= _energyCost); } }

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
            if (Active && CanBeActivated && !_isEnabled)
            {

                if (_ship.Stats.EnergyShield.Value >= _ship.Stats.EnergyShield.MaxValue * 0.2f)
                {
                    if (_ship.Stats.Energy.TryGet(_energyCost * elapsedTime))
                    {
                        InvokeTriggers(ConditionType.OnActivate);
                        _isEnabled = true;
                    }
                }
            }
            else if (Active && CanBeActivated && _isEnabled)
            {
                if (_ship.Stats.EnergyShield.Value >= 0)
                {
                    if (_ship.Stats.Energy.TryGet(_energyCost * elapsedTime))
                    {
                        InvokeTriggers(ConditionType.OnActivate);
                    }
                }
                else
                {
                    Deactivate();
                }
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

        private readonly float _power;
        private bool _isEnabled;
        private readonly float _energyCost;
        private readonly IShip _ship;
    }
}
