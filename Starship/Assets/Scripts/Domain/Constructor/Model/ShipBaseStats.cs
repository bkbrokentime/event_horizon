using GameDatabase.DataModel;
using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipBaseStats
    {
        public StatMultiplier BaseArmorMultiplier;
        public StatMultiplier BaseWeightMultiplier;

        public StatMultiplier EnergyResistanceMultiplier;
        public StatMultiplier HeatResistanceMultiplier;
        public StatMultiplier KineticResistanceMultiplier;
        public StatMultiplier QuantumResistanceMultiplier;

        public StatMultiplier ShieldEnergyResistanceMultiplier;
        public StatMultiplier ShieldHeatResistanceMultiplier;
        public StatMultiplier ShieldKineticResistanceMultiplier;
        public StatMultiplier ShieldQuantumResistanceMultiplier;

        public StatMultiplier EnergyShieldEnergyResistanceMultiplier;
        public StatMultiplier EnergyShieldHeatResistanceMultiplier;
        public StatMultiplier EnergyShieldKineticResistanceMultiplier;
        public StatMultiplier EnergyShieldQuantumResistanceMultiplier;

        public float RegenerationRate;
        public bool AutoTargeting;
        public Layout Layout;
        public Layout SecondLayout;
        public ImmutableCollection<Device> BuiltinDevices;
    }
}
