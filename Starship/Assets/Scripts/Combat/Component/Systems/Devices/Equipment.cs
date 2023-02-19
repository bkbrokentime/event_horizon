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
    public class Equipment : SystemBase, IDevice, IFeaturesModification, IStatsWeaponModification, IStatsModification
    {
        public Equipment(IShip ship, DeviceStats deviceSpec, int keyBinding)
            : base(keyBinding, deviceSpec.ControlButtonIcon, ship)
        {
            MaxCooldown = deviceSpec.Cooldown;

            _ship = ship;
            _activeColor = deviceSpec.Color;
            _energyCost = deviceSpec.EnergyConsumption;
            _lifetime = deviceSpec.Lifetime;
            _power= deviceSpec.Power;
            _equipmentStats= deviceSpec.EquipmentStats;
        }

        public override float ActivationCost { get { return _energyCost; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && (_isEnabled || _ship.Stats.Energy.Value >= _energyCost); } }
        public override IStatsWeaponModification StatsWeaponModification { get { return this; } }
        public bool TryApplyModification(ref WeaponUpgrade data)
        {
            if (_isEnabled)
            {
                data.DamageMultiplier += _equipmentStats.WeaponDamageMultiplier;
                data.RangeMultiplier += _equipmentStats.WeaponRangeMultiplier;
                data.EnergyCostMultiplier += _equipmentStats.WeaponEnergyCostMultiplier;
                data.LifetimeMultiplier += _equipmentStats.WeaponLifetimeMultiplier;
                data.AoeRadiusMultiplier += _equipmentStats.WeaponAoeRadiusMultiplier;
                data.VelocityMultiplier += _equipmentStats.WeaponVelocityMultiplier;
                data.WeightMultiplier += _equipmentStats.WeaponWeightMultiplier;
                data.SizeMultiplier += _equipmentStats.WeaponSizeMultiplier;
                data.FireRateMultiplier += _equipmentStats.WeaponFireRateMultiplier;
                data.SpreadMultiplier += _equipmentStats.WeaponSpreadMultiplier;
                data.MagazineMultiplier += _equipmentStats.WeaponMagazineMultiplier;
            }
            return true;
        }
        public bool TryApplyModification(ref Resistance data)
        {
            if (_isEnabled)
            {
                data.Kinetic *= 1 - _equipmentStats.KineticResistance;
                data.Energy *= 1 - _equipmentStats.EnergyResistance;
                data.Heat *= 1 - _equipmentStats.ThermalResistance;
                data.Quantum *= 1 - _equipmentStats.QuantumResistance;

                data.ShieldKinetic *= 1 - _equipmentStats.ShieldKineticResistance;
                data.ShieldEnergy *= 1 - _equipmentStats.ShieldEnergyResistance;
                data.ShieldHeat *= 1 - _equipmentStats.ShieldThermalResistance;
                data.ShieldQuantum *= 1 - _equipmentStats.ShieldQuantumResistance;

                data.EnergyShieldKinetic *= 1 - _equipmentStats.EnergyShieldKineticResistance;
                data.EnergyShieldEnergy *= 1 - _equipmentStats.EnergyShieldEnergyResistance;
                data.EnergyShieldHeat *= 1 - _equipmentStats.EnergyShieldThermalResistance;
                data.EnergyShieldQuantum *= 1 - _equipmentStats.EnergyShieldQuantumResistance;

                data.Kinetic += _equipmentStats.KineticResistance;
                data.Energy += _equipmentStats.EnergyResistance;
                data.Heat += _equipmentStats.ThermalResistance;
                data.Quantum += _equipmentStats.QuantumResistance;

                data.ShieldKinetic += _equipmentStats.ShieldKineticResistance;
                data.ShieldEnergy += _equipmentStats.ShieldEnergyResistance;
                data.ShieldHeat += _equipmentStats.ShieldThermalResistance;
                data.ShieldQuantum += _equipmentStats.ShieldQuantumResistance;

                data.EnergyShieldKinetic += _equipmentStats.EnergyShieldKineticResistance;
                data.EnergyShieldEnergy += _equipmentStats.EnergyShieldEnergyResistance;
                data.EnergyShieldHeat += _equipmentStats.EnergyShieldThermalResistance;
                data.EnergyShieldQuantum += _equipmentStats.EnergyShieldQuantumResistance;
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

        private readonly EquipmentStats _equipmentStats;
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
