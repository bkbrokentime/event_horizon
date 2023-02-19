using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class Fortified : IModification
    {
        public Fortified(ModificationQuality quality)
        {
            _defenseMultiplier = quality.PowerMultiplier(0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.2f, 1.5f, 2.0f, 2.5f, 3.0f);
            Quality = quality;
        }

		public string GetDescription(ILocalization localization) { return localization.GetString("$DefenseMod", Maths.Format.SignedPercent(_defenseMultiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.ArmorPoints > 0)
                stats.ArmorPoints *= _defenseMultiplier;
            if (stats.EnergyResistance > 0)
                stats.EnergyResistance *= _defenseMultiplier;
            if (stats.ThermalResistance > 0)
                stats.ThermalResistance *= _defenseMultiplier;
            if (stats.KineticResistance > 0)
                stats.KineticResistance *= _defenseMultiplier;
            if (stats.QuantumResistance > 0)
                stats.QuantumResistance *= _defenseMultiplier;

            if (stats.ShieldEnergyResistance > 0)
                stats.ShieldEnergyResistance *= _defenseMultiplier;
            if (stats.ShieldThermalResistance > 0)
                stats.ShieldThermalResistance *= _defenseMultiplier;
            if (stats.ShieldKineticResistance > 0)
                stats.ShieldKineticResistance *= _defenseMultiplier;
            if (stats.ShieldQuantumResistance > 0)
                stats.ShieldQuantumResistance *= _defenseMultiplier;

            if (stats.EnergyShieldEnergyResistance > 0)
                stats.EnergyShieldEnergyResistance *= _defenseMultiplier;
            if (stats.EnergyShieldThermalResistance > 0)
                stats.EnergyShieldThermalResistance *= _defenseMultiplier;
            if (stats.EnergyShieldKineticResistance > 0)
                stats.EnergyShieldKineticResistance *= _defenseMultiplier;
            if (stats.EnergyShieldQuantumResistance > 0)
                stats.EnergyShieldQuantumResistance *= _defenseMultiplier;

            if (stats.ShieldPoints > 0)
                stats.ShieldPoints *= _defenseMultiplier;
            if (stats.EnergyShieldPoints > 0)
                stats.EnergyShieldPoints *= _defenseMultiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref DroneBayStats droneBay) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        private readonly float _defenseMultiplier;
    }
}
