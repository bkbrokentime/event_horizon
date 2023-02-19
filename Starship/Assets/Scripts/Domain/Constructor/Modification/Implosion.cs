using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Implosion : IModification
    {
        public Implosion(ModificationQuality quality)
        {
            _damageMultiplier = quality.PowerMultiplier(0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f);
            _areaOfEffectMultiplier = quality.PowerMultiplier(0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 0.8f, 0.7f, 0.6f, 0.4f, 0.2f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString(
                "$ImplosionMod",
                Maths.Format.SignedPercent(_damageMultiplier - 1.0f),
                Maths.Format.SignedPercent(_areaOfEffectMultiplier - 1.0f));
        }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            ammunition.AreaOfEffect *= _areaOfEffectMultiplier;
            ammunition.Damage *= _damageMultiplier;
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.DamageMultiplier *= _damageMultiplier;
            statModifier.AoeRadiusMultiplier *= _areaOfEffectMultiplier;
        }

        private readonly float _areaOfEffectMultiplier;
        private readonly float _damageMultiplier;
    }
}
