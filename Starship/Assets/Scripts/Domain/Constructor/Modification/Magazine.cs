using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    class Magazine : IModification
    {
        public Magazine(ModificationQuality quality)
        {
            _Multiplier = quality.PowerMultiplier(0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f);
            Quality = quality;
        }

        public string GetDescription(ILocalization localization) { return localization.GetString("$MagazineMod", Maths.Format.SignedPercent(_Multiplier - 1.0f)); }

        public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            weapon.Magazine = (int)(weapon.Magazine * _Multiplier);
        }

        public void Apply(ref WeaponStatModifier statModifier)
        {
            statModifier.MagazineMultiplier *= _Multiplier;
        }

        private readonly float _Multiplier;
    }
}
