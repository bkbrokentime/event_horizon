using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class ShieldEnergyDefenseModification : IShipModification
    {
        public ShieldEnergyDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.ShieldEnergyDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_ShieldEnergyDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.ShieldEnergyResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
