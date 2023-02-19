using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Scatter : IModification
    {
        public Scatter(ModificationQuality quality)
        {
            _spreadMultiplier = quality.PowerMultiplier(1.25f, 1.2f, 1.15f, 1.1f, 1.05f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f);
            _cooldownMultiplier = quality.PowerMultiplier(1.25f, 1.2f, 1.15f, 1.1f, 1.05f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString(
                "$ScatterMod",
                Maths.Format.SignedPercent(_spreadMultiplier - 1.0f),
                Maths.Format.SignedPercent(_cooldownMultiplier - 1.0f));
        }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            weapon.Spread *= _spreadMultiplier;
            weapon.FireRate /= _cooldownMultiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.SpreadMultiplier *= _spreadMultiplier;
            statModifier.FireRateMultiplier *= 1f / _cooldownMultiplier;
        }

        private readonly float _spreadMultiplier;
        private readonly float _cooldownMultiplier;
    }
}
