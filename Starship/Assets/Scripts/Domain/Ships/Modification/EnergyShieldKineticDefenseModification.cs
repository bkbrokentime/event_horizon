using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EnergyShieldKineticDefenseModification : IShipModification
    {
        public EnergyShieldKineticDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.EnergyShieldKineticDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_EnergyShieldKineticDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.EnergyShieldKineticResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
