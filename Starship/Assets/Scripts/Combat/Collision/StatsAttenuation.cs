using GameDatabase.Model;
using UnityEngine;

namespace Combat.Collision
{
    public struct StatsAttenuation
    {
        public float ArmorPointsAttenuatableRate;
        public float ArmorRepairAttenuatableRate;
        public float EnergyPointsAttenuatableRate;
        public float EnergyRechargeAttenuatableRate;
        public float ShieldPointsAttenuatableRate;
        public float ShieldRechargeAttenuatableRate;
        public float EnergyShieldPointsAttenuatableRate;
        public float EnergyShieldRechargeAttenuatableRate;

        public static readonly StatsAttenuation Empty = new StatsAttenuation();
    }
}
