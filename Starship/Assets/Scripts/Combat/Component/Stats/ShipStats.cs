using Combat.Collision;
using Combat.Component.Mods;
using Combat.Unit.HitPoints;
using Constructor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameDatabase.Enums;
using DebugLogSetting;

namespace Combat.Component.Stats
{
    public class ShipStats : IStats
    {
        public ShipStats(IShipSpecification spec)
        {
            var stats = spec.Stats;
            //Level = spec.Type.Level;
            _resistance = new Resistance
            {
                Energy = stats.EnergyResistancePercentage,
                EnergyDrain = stats.EnergyAbsorptionPercentage,
                Heat = stats.ThermalResistancePercentage,
                Kinetic = stats.KineticResistancePercentage,
                Quantum = stats.QuantumResistancePercentage,

                ShieldEnergy = stats.ShieldEnergyResistancePercentage,
                ShieldHeat = stats.ShieldThermalResistancePercentage,
                ShieldKinetic = stats.ShieldKineticResistancePercentage,
                ShieldQuantum = stats.ShieldQuantumResistancePercentage,

                EnergyShieldEnergy = stats.EnergyShieldEnergyResistancePercentage,
                EnergyShieldHeat = stats.EnergyShieldThermalResistancePercentage,
                EnergyShieldKinetic = stats.EnergyShieldKineticResistancePercentage,
                EnergyShieldQuantum = stats.EnergyShieldQuantumResistancePercentage


            };
            _weaponupgrade = new WeaponUpgrade
            {
                DamageMultiplier = 1f,
                RangeMultiplier = 1f,
                EnergyCostMultiplier = 1f,
                LifetimeMultiplier = 1f,
                AoeRadiusMultiplier = 1f,
                VelocityMultiplier = 1f,
                WeightMultiplier = 1f,
                SizeMultiplier = 1f,

                FireRateMultiplier = 1f,
                SpreadMultiplier = 1f,
                MagazineMultiplier = 1f,
            };

            _statsattenuation = new StatsAttenuation
            {
                ArmorPointsAttenuatableRate = stats.ArmorPointsAttenuatableRate,
                ArmorRepairAttenuatableRate = stats.ArmorRepairAttenuatableRate,
                EnergyPointsAttenuatableRate = stats.EnergyPointsAttenuatableRate,
                EnergyRechargeAttenuatableRate = stats.EnergyRechargeAttenuatableRate,
                ShieldPointsAttenuatableRate = stats.ShieldPointsAttenuatableRate,
                ShieldRechargeAttenuatableRate = stats.ShieldRechargeAttenuatableRate,
                EnergyShieldPointsAttenuatableRate = stats.EnergyShieldPointsAttenuatableRate,
                EnergyShieldRechargeAttenuatableRate = stats.EnergyShieldRechargeAttenuatableRate,
            };

            WeaponDamageMultiplier = stats.DamageMultiplier.Value;
            RammingDamageMultiplier = stats.RammingDamageMultiplier;
            HitPointsMultiplier = stats.ArmorMultiplier.Value;

            if (stats.ArmorPoints < 0.1f)
                _armorPoints = new EmptyResources();
            else if (stats.ArmorRepairRate > 0)
                _armorPoints = new Energy(stats.ArmorPoints, stats.ArmorRepairRate, stats.ArmorRepairCooldown, 1, stats.ArmorRepairAttenuatableRate > 0, stats.ArmorRepairAttenuatableRate, stats.ArmorPointsAttenuatableRate > 0, stats.ArmorPointsAttenuatableRate);
            else
                _armorPoints = new HitPoints(stats.ArmorPoints);

            _shieldPoints = new Energy(stats.ShieldPoints, stats.ShieldRechargeRate, stats.ShieldRechargeCooldown, 1, stats.ShieldRechargeAttenuatableRate > 0, stats.ShieldRechargeAttenuatableRate, stats.ShieldPointsAttenuatableRate > 0, stats.ShieldPointsAttenuatableRate);
            _energyPoints = new Energy(stats.EnergyPoints, stats.EnergyRechargeRate, stats.EnergyRechargeCooldown, 1, stats.EnergyRechargeAttenuatableRate > 0, stats.EnergyRechargeAttenuatableRate, stats.EnergyPointsAttenuatableRate > 0, stats.EnergyPointsAttenuatableRate);
            _energyshieldPoints = new Energy(stats.EnergyShieldPoints, stats.EnergyShieldRechargeRate, stats.EnergyShieldRechargeCooldown, 1, stats.EnergyShieldRechargeAttenuatableRate > 0, stats.EnergyShieldRechargeAttenuatableRate, stats.EnergyShieldPointsAttenuatableRate > 0, stats.EnergyShieldPointsAttenuatableRate);

            _damageStorage = new DamageStorage();
        }

        public bool IsAlive { get { return _armorPoints.Value > 0; } }
        public bool SpaceJump { get; set; }
        public bool IsStealth { get; set; }

        public IResourcePoints Armor { get { return _armorPoints; } }
        public IResourcePoints Shield { get { return _shieldPoints; } }
        public IResourcePoints Energy { get { return _energyPoints; } }
        public IResourcePoints EnergyShield { get { return _energyshieldPoints; } }

        public float WeaponDamageMultiplier { get; private set; }
        public float RammingDamageMultiplier { get; private set; }
        public float HitPointsMultiplier { get; private set; }

        public Resistance Resistance
        {
            get
            {
                var resistance = _resistance;
                _modifications.Apply(ref resistance);
                return resistance;
            }
        }
        public WeaponUpgrade WeaponUpgrade
        {
            get
            {
                var weaponupgrade = _weaponupgrade;
                _weaponmodifications.Apply(ref weaponupgrade);
                return weaponupgrade;
            }
        }
        public StatsAttenuation StatsAttenuation
        {
            get
            {
                var statsattenuation = _statsattenuation;
                _statsattenuationmodifications.Apply(ref statsattenuation);
                return statsattenuation;
            }
        }
        //public int Level { get; set; }

        public Modifications<Resistance> Modifications { get { return _modifications; } }
        public Modifications<WeaponUpgrade> WeaponModifications { get { return _weaponmodifications; } }
        public Modifications<StatsAttenuation> StatsAttenuationModifications { get { return _statsattenuationmodifications; } }

        public IDamageIndicator DamageIndicator { get; set; }

        public float TimeFromLastHit { get; private set; }

        public void ApplyDamage(Impact impact)
        {
            var resistance = Resistance;

            if (EnergyShield.Exists)
                impact.EnergyShieldReduceQuantumDamage(resistance);


            if (Shield.Exists)
                impact.ApplyShield(Shield.Value - impact.AllDamageData.ShieldDamage.GetShieldDamage(), resistance);
            else
                impact.AllDamageData.ShieldDamage.RemoveTypeDamage(DefenseType.Shield);

            if (_damageStorage.Flame > 0)
                impact.DamageStorageRead_Flame(_damageStorage);
            if (_damageStorage.Corrosion > 0)
                impact.DamageStorageRead_Corrosion(_damageStorage);

            if (DamageIndicator != null)
            {
                if (DamageDebugLogSetting.DamageDebugLog)
                {
                    Debug.Log("ApplyDamage.impact:      ----");
                    impact.AllDamageData.DarkenergyDamage.DebugList();
                }
                var newimpact = impact.GetDamage(resistance);

                if (DamageDebugLogSetting.DamageDebugLog)
                {
                    Debug.Log("ApplyDamage.newimpact:   ----");
                    newimpact.AllDamageData.DarkenergyDamage.DebugList();
                }
                DamageIndicator.ApplyDamage(newimpact);
            }
            var damage = impact.ArmorGetTotalDamage(resistance);
            if (damage > 0.1f)
                TimeFromLastHit = 0;

            if (resistance.EnergyDrain > 0.01f)
            {
                var energy = resistance.EnergyDrain * impact.AllDamageData.EnergyDamage.Value / HitPointsMultiplier;
                Energy.Get(-energy);
            }

            damage -= impact.AllDamageData.Repair.Value;
            Armor.Get(damage);
            Energy.Get(impact.AllDamageData.EnergyDrain.Value);
            Shield.Get(impact.AllDamageData.ShieldDamage.GetShieldDamage());
            EnergyShield.Get(impact.AllDamageData.EnergyShieldDamage.GetEnergyShieldDamage());

            if (impact.Flame > 0)
            {
                //_flamelasttime = _flamenewtime;
                _damageStorage.ALLDamageStorage(impact);
            }
            if (impact.Corrosion > 0)
            {
                //_corrosionlasttime = _corrosionnewtime;
                _damageStorage.ALLDamageStorage(impact);
            }
            if (impact.Effects.Contains(CollisionEffect.Destroy))
                Armor.Get(Armor.MaxValue);
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (!IsAlive)
                return;

            if (DamageIndicator != null)
                DamageIndicator.Update(elapsedTime);

            _energyPoints.Update(elapsedTime);
            _energyshieldPoints.Update(elapsedTime);
            _armorPoints.Update(elapsedTime);
            _shieldPoints.Update(elapsedTime);

            _flamelasttime += elapsedTime;
            if (_flamelasttime >= _flamecoldtime && _damageStorage.Flame > 0)
            {
                _damageStorage.Flame--;
                _flamelasttime = 0;
            }

            _corrosionlasttime += elapsedTime;
            if (_corrosionlasttime >= _corrosioncoldtime && _damageStorage.Corrosion > 0)
            {
                _damageStorage.Corrosion--;
                _corrosionlasttime = 0;
            }


            TimeFromLastHit += elapsedTime;
        }

        public void Dispose()
        {
            if (DamageIndicator != null)
                DamageIndicator.Dispose();
        }

        private readonly IResourcePoints _armorPoints;
        private readonly IResourcePoints _shieldPoints;
        private readonly IResourcePoints _energyPoints;
        private readonly IResourcePoints _energyshieldPoints;
        private readonly Resistance _resistance;
        private readonly WeaponUpgrade _weaponupgrade;
        private readonly StatsAttenuation _statsattenuation;
        private readonly Modifications<Resistance> _modifications = new Modifications<Resistance>();
        private readonly Modifications<WeaponUpgrade> _weaponmodifications = new Modifications<WeaponUpgrade>();
        private readonly Modifications<StatsAttenuation> _statsattenuationmodifications = new Modifications<StatsAttenuation>();

        private readonly DamageStorage _damageStorage;

        private float _flamelasttime;
        private float _flamecoldtime = 0.2f;
        private float _corrosionlasttime;
        private float _corrosioncoldtime = 0.5f;

        public DamageStorage damageStorage()
        {
            return _damageStorage;
        }


    }
}
