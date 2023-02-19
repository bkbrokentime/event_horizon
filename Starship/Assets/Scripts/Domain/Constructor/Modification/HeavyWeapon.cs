using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class HeavyWeapon : IModification
    {
        public HeavyWeapon(ModificationQuality quality)
        {
            _damageMultiplier = quality.PowerMultiplier(0.6f, 0.8f, 0.9f, 1.1f, 1.2f, 2.0f, 3.0f, 4.0f, 6.0f, 8.0f);
            _cooldownMultiplier = quality.PowerMultiplier(2.5f, 2.0f, 1.75f, 1.5f, 1.25f, 1.75f, 2.0f, 2.25f, 2.5f, 3.0f);

            Quality = quality;
        }

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString(
                "$HeavyWeaponMod",
                Maths.Format.SignedPercent(_damageMultiplier - 1.0f),
                Maths.Format.SignedPercent(_cooldownMultiplier - 1.0f));
        }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            weapon.FireRate /= _cooldownMultiplier;
            ammunition.Damage *= _damageMultiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.DamageMultiplier *= _damageMultiplier;
            statModifier.FireRateMultiplier *= 1f / _cooldownMultiplier;
        }

        private readonly float _cooldownMultiplier;
        private readonly float _damageMultiplier;
    }
}
