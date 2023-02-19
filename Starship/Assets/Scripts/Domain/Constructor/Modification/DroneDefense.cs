using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
    class DroneDefense : IModification
    {
        public DroneDefense(ModificationQuality quality)
        {
            _multiplier = quality.PowerMultiplier(0.2f, 0.4f, 0.6f, 0.7f, 0.8f, 1.2f, 1.5f, 2.0f, 3.0f, 4.0f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$DefenseMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats)
        {
            if (stats.DroneDefenseMultiplier.HasValue)
                stats.DroneDefenseMultiplier %= _multiplier;
        }

        public void Apply(ref DeviceStats device) { }
        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
        public void Apply(ref WeaponStatModifier statModifier) { }

        public void Apply(ref DroneBayStats droneBay)
        {
            droneBay.DefenseMultiplier += _multiplier - 1f;
        }

        private readonly float _multiplier;
    }
}
