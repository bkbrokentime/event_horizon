using System;
using Combat.Collision;
using Combat.Component.Mods;
using Combat.Unit.HitPoints;
using Constructor;
using GameDatabase.DataModel;

namespace Combat.Component.Stats
{
    public interface IStats : IDisposable
    {
        bool IsAlive { get; }
        bool SpaceJump { get; set; }
        bool IsStealth { get; set; }

        IResourcePoints Armor { get; }
        IResourcePoints Shield { get; }
        IResourcePoints Energy { get; }
        IResourcePoints EnergyShield { get; }

        float WeaponDamageMultiplier { get; }
        float RammingDamageMultiplier { get; }
        float HitPointsMultiplier { get; }

        Resistance Resistance { get; }
        WeaponUpgrade WeaponUpgrade { get; }
        Modifications<Resistance> Modifications { get; }
        Modifications<WeaponUpgrade> WeaponModifications { get; }
        Modifications<StatsAttenuation> StatsAttenuationModifications { get; }

        float TimeFromLastHit { get; }

        void ApplyDamage(Impact damage);
        void UpdatePhysics(float elapsedTime);
    }
}
