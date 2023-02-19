using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EnergyShieldQuantumDefenseModification : IShipModification
    {
        public EnergyShieldQuantumDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.EnergyShieldQuantumDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_EnergyShieldQuantumDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.EnergyShieldQuantumResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
