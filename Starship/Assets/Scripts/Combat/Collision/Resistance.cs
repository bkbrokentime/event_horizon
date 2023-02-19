using UnityEngine;

namespace Combat.Collision
{
    public struct Resistance
    {
        public float Kinetic;
        public float Energy;
        public float Heat;
        public float Quantum;

        public float ShieldKinetic;
        public float ShieldEnergy;
        public float ShieldHeat;
        public float ShieldQuantum;

        public float EnergyShieldKinetic;
        public float EnergyShieldEnergy;
        public float EnergyShieldHeat;
        public float EnergyShieldQuantum;

        public float EnergyDrain;

        public float MinResistance { get { return Mathf.Min(Mathf.Min(Mathf.Min(Kinetic, Energy), Heat),Quantum); } }
        public float MinShieldResistance { get { return Mathf.Min(Mathf.Min(Mathf.Min(ShieldKinetic, ShieldEnergy), ShieldHeat), ShieldQuantum); } }
        public float MinEnergyShieldResistance { get { return Mathf.Min(Mathf.Min(Mathf.Min(EnergyShieldKinetic, EnergyShieldEnergy), EnergyShieldHeat), EnergyShieldQuantum); } }

        public static readonly Resistance Empty = new Resistance();
    }
}
