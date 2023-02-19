using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class EnergyShieldHeatDefenseModification : IShipModification
    {
        public EnergyShieldHeatDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.EnergyShieldHeatDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_EnergyShieldHeatDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.EnergyShieldHeatResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
