using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class ShieldHeatDefenseModification : IShipModification
    {
        public ShieldHeatDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.ShieldHeatDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_ShieldHeatDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.ShieldHeatResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
