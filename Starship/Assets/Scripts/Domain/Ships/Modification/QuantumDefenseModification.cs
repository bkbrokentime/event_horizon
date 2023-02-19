using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class QuantumDefenseModification : IShipModification
    {
        public QuantumDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.QuantumDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_QuantumDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.QuantumResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
