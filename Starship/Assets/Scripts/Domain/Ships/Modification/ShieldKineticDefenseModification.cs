using Constructor.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class ShieldKineticDefenseModification : IShipModification
    {
        public ShieldKineticDefenseModification(int seed)
        {
            Seed = seed;
            _value = 0.5f;
        }

        public ModificationType Type => ModificationType.ShieldKineticDefense;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_ShieldKineticDefense");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            stats.ShieldKineticResistanceMultiplier += _value;
        }

        public int Seed { get; }

        private readonly float _value;
    }
}
