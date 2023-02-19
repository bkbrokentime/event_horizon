using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class ShieldQuantumDefenseModification : IShipModification
    {
        public ShieldQuantumDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.ShieldQuantumDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_ShieldQuantumDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.ShieldQuantumResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
