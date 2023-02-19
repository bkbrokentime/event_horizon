using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EnergyShieldEnergyDefenseModification : IShipModification
    {
        public EnergyShieldEnergyDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.EnergyShieldEnergyDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_EnergyShieldEnergyDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.EnergyShieldEnergyResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
