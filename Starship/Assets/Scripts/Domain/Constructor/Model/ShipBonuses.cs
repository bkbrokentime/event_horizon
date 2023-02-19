using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipBonuses
    {
        public StatMultiplier ArmorPointsMultiplier;

        public StatMultiplier ShieldPointsMultiplier;
        public StatMultiplier ShieldRechargeMultiplier;

        public StatMultiplier EnergyShieldPointsMultiplier;
        public StatMultiplier EnergyShieldRechargeMultiplier;

        public StatMultiplier EnergyMultiplier;
        public StatMultiplier DamageMultiplier;
        public StatMultiplier VelocityMultiplier;
        public StatMultiplier RammingDamageMultiplier;

        public float ExtraHeatResistance;
        public float ExtraEnergyResistance;
        public float ExtraKineticResistance;
        public float ExtraQuantumResistance;

        public float ExtraShieldHeatResistance;
        public float ExtraShieldEnergyResistance;
        public float ExtraShieldKineticResistance;
        public float ExtraShieldQuantumResistance;

        public float ExtraEnergyShieldHeatResistance;
        public float ExtraEnergyShieldEnergyResistance;
        public float ExtraEnergyShieldKineticResistance;
        public float ExtraEnergyShieldQuantumResistance;

        public StatMultiplier ScaleMultiplier;
    }
}
