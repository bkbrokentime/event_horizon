using GameDatabase.Model;
using UnityEngine;

namespace Combat.Collision
{
    public struct WeaponUpgrade
    {
        public float DamageMultiplier;
        public float RangeMultiplier;
        public float EnergyCostMultiplier;
        public float LifetimeMultiplier;
        public float AoeRadiusMultiplier;
        public float VelocityMultiplier;
        public float WeightMultiplier;
        public float SizeMultiplier;

        public float FireRateMultiplier;
        public float SpreadMultiplier;
        public float MagazineMultiplier;



        public static readonly WeaponUpgrade Empty = new WeaponUpgrade();
    }
}
