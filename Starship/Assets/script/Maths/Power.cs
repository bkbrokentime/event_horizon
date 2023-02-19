using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Combat.Factory;
using Combat.Unit.HitPoints;
using Constructor;
using Constructor.Component;
using Constructor.Model;
using Constructor.Ships;
using DebugLogSetting;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using ModestTree;
using SA.Common.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Maths
{
    public static class EasyString
    {
        public static string ValueToString(int value)
        {
            if (value < 1e3f)
                return value.ToString("N3") + "  ";
            if (value < 1e6f)
                return (value / 1e3f).ToString("N3") + " K";
            if (value < 1e9f)
                return (value / 1e6f).ToString("N3") + " M";
            return (value / 1e9f).ToString() + " B";
        }
        public static string ValueToString(float value)
        {
            if (value < 1e3f)
                return value.ToString("N3") + "  ";
            if (value < 1e6f)
                return (value / 1e3f).ToString("N3") + " K";
            if (value < 1e9f)
                return (value / 1e6f).ToString("N3") + " M";
            if (value < 1e12f)
                return (value / 1e9f).ToString("N3") + " B";
            return (value / 1e12f).ToString("N3") + " T";
        }
        public static string ValueToString(long value)
        {
            if (value < 1e3)
                return value.ToString() + "  ";
            if (value < 1e6)
                return (value / 1e3).ToString() + " K";
            if (value < 1e9)
                return (value / 1e6).ToString() + " M";
            if (value < 1e12)
                return (value / 1e9).ToString() + " B";
            return (value / 1e12).ToString() + " T";
        }
        public static string ValueToString(double value)
        {
            if (value < 1e3)
                return value.ToString("N3") + "  ";
            if (value < 1e6)
                return (value / 1e3).ToString("N3") + " K";
            if (value < 1e9)
                return (value / 1e6).ToString("N3") + " M";
            if (value < 1e12)
                return (value / 1e9).ToString("N3") + " B";
            return (value / 1e12).ToString("N3") + " T";
        }
    }
    public class PowerScore
    {
        public PowerScore(float value)
        {
            _score = value;
        }
        public float Score { get { return _score; } }
        public override string ToString()
        {
            if (Score < 1e3f)
                return Score.ToString("N3") + "  ";
            if (Score < 1e6f)
                return (Score / 1e3f).ToString("N3") + " K";
            if (Score < 1e9f)
                return (Score / 1e6f).ToString("N3") + " M";
            if (Score < 1e12f)
                return (Score / 1e9f).ToString("N3") + " B";
            return (Score / 1e12f).ToString("N3") + " T";
        }
        public static float operator +(PowerScore score1, PowerScore score2) { return score1.Score + score2.Score; }
        public static float operator -(PowerScore score1, PowerScore score2) { return score1.Score - score2.Score; }
        public static float operator +(PowerScore score1, float score2) { return score1.Score + score2; }
        public static float operator -(PowerScore score1, float score2) { return score1.Score - score2; }
        public static float operator *(PowerScore score1, float score2) { return score1.Score * score2; }
        public static float operator /(PowerScore score1, float score2) { return score1.Score / score2; }

        private float _score;
    }
	public static class Power
	{
        public static float ArmorDefenseScore(IShipStats Stats)
        {
            var ArmorPoints = Stats.ArmorPoints;

            var KineticResistancePercentage = Generic.DefaultForNaN(Stats.KineticResistancePercentage, 0);
            var EnergyResistancePercentage = Generic.DefaultForNaN(Stats.EnergyResistancePercentage, 0);
            var ThermalResistancePercentage = Generic.DefaultForNaN(Stats.ThermalResistancePercentage, 0);
            var QuantumResistancePercentage = Generic.DefaultForNaN(Stats.QuantumResistancePercentage, 0);

            var score = ArmorPoints * (1 + KineticResistancePercentage + EnergyResistancePercentage + ThermalResistancePercentage + QuantumResistancePercentage);
            return score / 100;
        }
        public static float ArmorEnduranceScore(IShipStats Stats)
        {
            var ArmorRepairRate = Stats.ArmorRepairRate;
            var ArmorRepairCooldown = Stats.ArmorRepairCooldown;
            var ArmorPointsAttenuatableRate = Stats.ArmorPointsAttenuatableRate;
            var ArmorRepairAttenuatableRate = Stats.ArmorRepairAttenuatableRate;

            var score = ArmorRepairRate * Mathf.Pow(1 - ArmorPointsAttenuatableRate, 30) * Mathf.Pow(1 - ArmorRepairAttenuatableRate, 30) / (ArmorRepairCooldown + 1) / (ArmorRepairCooldown + 1);
           
            return score / 100;
        }
        public static float ShieldDefenseScore(IShipStats Stats)
        {
            var ShieldPoints = Stats.ShieldPoints;

            var ShieldKineticResistancePercentage = Generic.DefaultForNaN(Stats.ShieldKineticResistancePercentage, 0);
            var ShieldEnergyResistancePercentage = Generic.DefaultForNaN(Stats.ShieldEnergyResistancePercentage, 0);
            var ShieldThermalResistancePercentage = Generic.DefaultForNaN(Stats.ShieldThermalResistancePercentage, 0);
            var ShieldQuantumResistancePercentage = Generic.DefaultForNaN(Stats.ShieldQuantumResistancePercentage, 0);

            var score = ShieldPoints * (1 + ShieldKineticResistancePercentage + ShieldEnergyResistancePercentage + ShieldThermalResistancePercentage + ShieldQuantumResistancePercentage);
            return score / 100;
        }
        public static float ShieldEnduranceScore(IShipStats Stats)
        {
            var ShieldRechargeCooldown = Stats.ShieldRechargeCooldown;
            var ShieldRechargeRate = Stats.ShieldRechargeRate;
            var ShieldPointsAttenuatableRate = Stats.ShieldPointsAttenuatableRate;
            var ShieldRechargeAttenuatableRate = Stats.ShieldRechargeAttenuatableRate;

            var score = ShieldRechargeRate * Mathf.Pow(1 - ShieldPointsAttenuatableRate, 30) * Mathf.Pow(1 - ShieldRechargeAttenuatableRate, 30) / (ShieldRechargeCooldown + 1) / (ShieldRechargeCooldown + 1);

            return score / 100;
        }
        public static float EnergyShieldDefenseScore(IShipStats Stats)
        {
            var EnergyShieldPoints = Stats.EnergyShieldPoints;

            var EnergyShieldKineticResistancePercentage = Generic.DefaultForNaN(Stats.EnergyShieldKineticResistancePercentage, 0);
            var EnergyShieldEnergyResistancePercentage = Generic.DefaultForNaN(Stats.EnergyShieldEnergyResistancePercentage, 0);
            var EnergyShieldThermalResistancePercentage = Generic.DefaultForNaN(Stats.EnergyShieldThermalResistancePercentage, 0);
            var EnergyShieldQuantumResistancePercentage = Generic.DefaultForNaN(Stats.EnergyShieldQuantumResistancePercentage, 0);

            var score = EnergyShieldPoints * (1 + EnergyShieldKineticResistancePercentage + EnergyShieldEnergyResistancePercentage + EnergyShieldThermalResistancePercentage + EnergyShieldQuantumResistancePercentage);
            return score / 100;
        }
        public static float EnergyShieldEnduranceScore(IShipStats Stats)
        {
            var EnergyShieldRechargeCooldown = Stats.EnergyShieldRechargeCooldown;
            var EnergyShieldRechargeRate = Stats.EnergyShieldRechargeRate;
            var EnergyShieldPointsAttenuatableRate = Stats.EnergyShieldPointsAttenuatableRate;
            var EnergyShieldRechargeAttenuatableRate = Stats.EnergyShieldRechargeAttenuatableRate;

            var score = EnergyShieldRechargeRate * Mathf.Pow(1 - EnergyShieldPointsAttenuatableRate, 30) * Mathf.Pow(1 - EnergyShieldRechargeAttenuatableRate, 30) / (EnergyShieldRechargeCooldown + 1) / (EnergyShieldRechargeCooldown + 1);

            return score / 100;
        }
        public static float EnergyScore(IShipStats Stats)
        {
            var EnergyPoints = Stats.EnergyPoints;

            var score = EnergyPoints;

            return score / 100;
        }
        public static float EnergyEnduranceScore(IShipStats Stats)
        {
            var EnergyRechargeRate = Stats.EnergyRechargeRate;
            var EnergyRechargeCooldown = Stats.EnergyRechargeCooldown;
            var EnergyPointsAttenuatableRate = Stats.EnergyPointsAttenuatableRate;
            var EnergyRechargeAttenuatableRate = Stats.EnergyRechargeAttenuatableRate;

            var score = EnergyRechargeRate * Mathf.Pow(1 - EnergyPointsAttenuatableRate, 30) * Mathf.Pow(1 - EnergyRechargeAttenuatableRate, 30) / (EnergyRechargeCooldown + 1) / (EnergyRechargeCooldown + 1);

            return score / 100;
        }
        public static float EngineScore(IShipStats Stats)
        {
            var EnginePower = Stats.EnginePower;
            var TurnRate = Stats.TurnRate;
            var EngineMAXPower = Stats.EngineMAXPower;
            var TurnMAXRate = Stats.TurnMAXRate;

            var EPscore = EnginePower * EnginePower + EngineMAXPower * EngineMAXPower * EngineMAXPower;
            var TRscore = TurnMAXRate * TurnMAXRate + TurnMAXRate * TurnMAXRate * TurnMAXRate;

            var score = Mathf.Sqrt(EPscore * TRscore);
            return score;
        }
        public static float DevicesScore(IShipSpecification Specification)
        {
            float score = 0;
            foreach (var device in Specification.Devices)
                score += device.DeviceScore();
            return score;
        }
        public static float DeviceScore(this IDeviceData data)
        {
            var DeviceClass = data.Device.DeviceClass;
            var EnergyConsumption = data.Device.EnergyConsumption;
            var PassiveEnergyConsumption = data.Device.PassiveEnergyConsumption * 0.05f;
            var Power = data.Device.Power;
            var Range = data.Device.Range;
            var Size = data.Device.Size;
            var Cooldown = data.Device.Cooldown;
            var Lifetime = data.Device.Lifetime;
            var Quantity = data.Device.Quantity;
            var EquipmentStats = data.Device.EquipmentStats;

            float score = 0;
            switch (DeviceClass)
            {
                case DeviceClass.ClonningCenter:
                    score = 50f;
                    break;
                case DeviceClass.TimeMachine:
                    score = Mathf.Pow(Lifetime, 2.5f);
                    break;
                case DeviceClass.Accelerator:
                    score = Mathf.Pow(Power, 1.2f);
                    break;
                case DeviceClass.Decoy:
                    score = Mathf.Pow(Lifetime, 1.5f) + Mathf.Pow(Quantity > 0 ? Quantity : Power, 1.8f) + Mathf.Pow(Range, 1.2f);
                    break;
                case DeviceClass.Ghost:
                    score = Mathf.Pow(Lifetime, 1.2f);
                    break;
                case DeviceClass.PointDefense:
                    score = Mathf.Pow(Quantity > 0 ? Quantity : Power, 1.5f) + Mathf.Pow(Size, 1.5f);
                    break;
                case DeviceClass.GravityGenerator:
                    score = Mathf.Pow(Power, 1.8f) + Mathf.Pow(Range, 1.2f);
                    break;
                case DeviceClass.EnergyShield:
                    score = (Mathf.Pow(Power, 1.2f) + Mathf.Pow(Size, 1.5f)) * 1.2f;
                    break;
                case DeviceClass.PartialShield:
                    score = Mathf.Pow(Power, 1.2f) + Mathf.Pow(Size, 1.5f);
                    break;
                case DeviceClass.Denseshield:
                    score = 12;
                    break;
                case DeviceClass.Fortification:
                    score = 8;
                    break;
                case DeviceClass.CombustionInhibition:
                    score = 8;
                    break;
                case DeviceClass.EnergyDiversion:
                    score = 8;
                    break;
                case DeviceClass.RepairBot:
                    score = Mathf.Pow(Power * Quantity, 0.9f);
                    break;
                case DeviceClass.Detonator:
                    score = Mathf.Pow(Power, 1.5f) + Mathf.Pow(Range, 1.2f);
                    break;
                case DeviceClass.Stealth:
                    score = 10;
                    break;
                case DeviceClass.Teleporter:
                    score =  Mathf.Pow(Range, 1.2f);
                    break;
                case DeviceClass.Brake:
                    score = Power;
                    break;
                case DeviceClass.SuperStealth:
                    score = 15;
                    break;
                case DeviceClass.FireAssault:
                    score = 15;
                    break;
                case DeviceClass.ToxicWaste:
                    score = Mathf.Pow(Lifetime, 1.2f) + Mathf.Pow(Power, 1.2f) + Mathf.Pow(Size, 1.2f);
                    break;
                case DeviceClass.WormTail:
                    score = (1 - Mathf.Pow(0.975f, Quantity > 0 ? Quantity : Size)) * Power * 39;
                    break;
                case DeviceClass.Equipment:
                    score = EquipmentStats.EquipmentScore();
                    break;
                default:
                    score = 0;
                    break;
            }

            return score * 10000 / (PassiveEnergyConsumption + 1) / (EnergyConsumption + 1) / (Cooldown + 1);
        }
        private static float EquipmentScore(this EquipmentStats data)
        {
            float score = 1;
            score *= data.WeaponAoeRadiusMultiplier + 1;
            score *= data.WeaponDamageMultiplier + 1;
            score *= data.WeaponEnergyCostMultiplier + 1;
            score *= data.WeaponFireRateMultiplier + 1;
            score *= data.WeaponLifetimeMultiplier + 1;
            score *= data.WeaponRangeMultiplier + 1;
            score *= data.WeaponSizeMultiplier + 1;
            score *= data.WeaponVelocityMultiplier + 1;
            score *= data.WeaponWeightMultiplier + 1;
/*
            score *= data.WeaponMagazineMultiplier + 1;
            score *= data.WeaponSpreadMultiplier + 1;
*/
            score *= data.KineticResistance + 1;
            score *= data.ThermalResistance + 1;
            score *= data.EnergyResistance + 1;
            score *= data.QuantumResistance + 1;
            score *= data.ShieldKineticResistance + 1;
            score *= data.ShieldThermalResistance + 1;
            score *= data.ShieldEnergyResistance + 1;
            score *= data.ShieldQuantumResistance + 1;
            score *= data.EnergyShieldKineticResistance + 1;
            score *= data.EnergyShieldThermalResistance + 1;
            score *= data.EnergyShieldEnergyResistance + 1;
            score *= data.EnergyShieldQuantumResistance + 1;
            return score;
        }
        public static float WeaponsScore(IShipSpecification Specification)
        {
            var platforms = Specification.Platforms;
            var weaponDPS = WeaponsDPS(Specification, false);
            var weaponMDPS = WeaponsDPS(Specification, true);
            var weaponEPS = WeaponsEPS(Specification, false);
            var weaponMEPS = WeaponsEPS(Specification, true);
            var weaponconcentration = WeaponsConcentration(Specification);
            var weapondispersion = WeaponsDispersion(Specification);

            var weaponefficiency = weaponDPS / (weaponEPS + 1);
            var weaponefficiency_M = weaponMDPS / (weaponMEPS + 1);
            float weaponscores = 0;
            foreach (var platform in platforms)
            {
                foreach (var weapon in platform.Weapons)
                    weaponscores += weapon.WeaponScore();
                foreach (var weapon in platform.WeaponsObsolete)
                    weaponscores += weapon.WeaponScore();
            }

            var score = weaponscores + Mathf.Max(weaponconcentration, weapondispersion) + Mathf.Sqrt(weaponefficiency * weaponefficiency + weaponefficiency_M * weaponefficiency_M) / 2;

            return score;
        }
        public static float WeaponsDPS(IShipSpecification Specification, bool M)
        {
            var platforms = Specification.Platforms;
            float DPS = 0;
            foreach (var platform in platforms)
            {
                foreach (var weapon in platform.Weapons)
                    DPS += weapon.WeaponDPS(M);
                foreach (var weapon in platform.WeaponsObsolete)
                    DPS += weapon.WeaponDPS(M);
            }
            return DPS;
        }
        public static float WeaponsEPS(IShipSpecification Specification, bool M)
        {
            var platforms = Specification.Platforms;
            float EPS = 0;
            foreach (var platform in platforms)
            {
                foreach (var weapon in platform.Weapons)
                {
                    EPS += weapon.WeaponEPS(M);
                }
                foreach (var weapon in platform.WeaponsObsolete)
                {
                    EPS += weapon.WeaponEPS(M);
                }
            }
            return EPS;
        }
        public static float WeaponsConcentration(IShipSpecification Specification)
        {
            var platforms = Specification.Platforms;
            float score = 0;
            Vector2 concentrationpoint = new Vector2(0, 0);
            foreach (var platform in platforms)
            {
                concentrationpoint += RotationHelpers.Direction(platform.Rotation);
            }
            concentrationpoint.Normalize();
            foreach (var platform in platforms)
            {
                float concentration = 0;
                foreach (var weapon in platform.Weapons)
                {
                    float spread = weapon.Weapon.Stats.Spread > 360 ? weapon.Weapon.Stats.Spread - 360 : weapon.Weapon.Stats.Spread;
                    concentration += weapon.WeaponDPS(false) * 360 / (spread + 360);
                }
                foreach (var weapon in platform.WeaponsObsolete)
                {
                    float spread = weapon.Weapon.Spread > 360 ? weapon.Weapon.Spread - 360 : weapon.Weapon.Spread;
                    concentration += weapon.WeaponDPS(false) * 360 / (spread + 360);
                }
                score += Mathf.Max(concentration * Vector2.Dot(RotationHelpers.Direction(platform.Rotation), concentrationpoint), 0);
            }
            return score;
        }
        public static float WeaponsDispersion(IShipSpecification Specification)
        {
            var platforms = Specification.Platforms;
            float score = 0;
            Vector2 concentrationpoint = new Vector2(0, 0);
            foreach (var platform in platforms)
            {
                concentrationpoint += RotationHelpers.Direction(platform.Rotation);
            }
            concentrationpoint.Normalize();
            foreach (var platform in platforms)
            {
                float Dispersion = 0;
                foreach (var weapon in platform.Weapons)
                {
                    float spread = weapon.Weapon.Stats.Spread > 360 ? weapon.Weapon.Stats.Spread - 360 : weapon.Weapon.Stats.Spread;
                    Dispersion += weapon.WeaponDPS(false) * spread / 360;
                }
                foreach (var weapon in platform.WeaponsObsolete)
                {
                    float spread = weapon.Weapon.Spread > 360 ? weapon.Weapon.Spread - 360 : weapon.Weapon.Spread;
                    Dispersion += weapon.WeaponDPS(false) * spread / 360;
                }
                score += Mathf.Max(Dispersion * Vector2.Dot(RotationHelpers.Direction(platform.Rotation), RotationHelpers.Direction(platform.Rotation) - concentrationpoint), 0);
            }
            return score;
        }
        public static float WeaponScore(this IWeaponData data)
        {
            var weaponDPS = WeaponDPS(data, false);
            var weaponMDPS = WeaponDPS(data, true);
            var weaponEPS = WeaponEPS(data, false);
            var weaponMEPS = WeaponEPS(data, true);
            var weaponefficiency = weaponDPS / (weaponEPS + 1);
            var weaponefficiency_M = weaponMDPS / (weaponMEPS + 1);

            var ammunitionscore = data.Ammunition.GetAmmunitionScore(1);
            
            var score = Mathf.Sqrt(weaponDPS * weaponDPS + weaponMDPS * weaponMDPS) / 2 + Mathf.Sqrt(weaponefficiency * weaponefficiency + weaponefficiency_M * weaponefficiency_M) / 2 + ammunitionscore / 2;
            return score;
        }
        public static float WeaponScore(this IWeaponDataObsolete data)
        {
            var weaponDPS = WeaponDPS(data, false);
            var weaponMDPS = WeaponDPS(data, true);
            var weaponEPS = WeaponEPS(data, false);
            var weaponMEPS = WeaponEPS(data, true);
            var weaponefficiency = weaponDPS / (weaponEPS + 1);
            var weaponefficiency_M = weaponMDPS / (weaponMEPS + 1);

            var ammunitionscore = data.Ammunition.GetAmmunitionScore(1);
            
            var score = Mathf.Sqrt(weaponDPS * weaponDPS + weaponMDPS * weaponMDPS) / 2 + Mathf.Sqrt(weaponefficiency * weaponefficiency + weaponefficiency_M * weaponefficiency_M) / 2 + ammunitionscore / 2;
            return score;
        }
        public static float WeaponScore(this WeaponData data)
        {
            var weaponDPS = WeaponDPS(data, false);
            var weaponMDPS = WeaponDPS(data, true);
            var weaponEPS = WeaponEPS(data, false);
            var weaponMEPS = WeaponEPS(data, true);
            var weaponefficiency = weaponDPS / (weaponEPS + 1);
            var weaponefficiency_M = weaponMDPS / (weaponMEPS + 1);

            var ammunitionscore = data.Ammunition.GetAmmunitionScore(1);
            
            var score = Mathf.Sqrt(weaponDPS * weaponDPS + weaponMDPS * weaponMDPS) / 2 + Mathf.Sqrt(weaponefficiency * weaponefficiency + weaponefficiency_M * weaponefficiency_M) / 2 + ammunitionscore / 2;
            return score;
        }
        public static float WeaponScore(WeaponStats data, AmmunitionObsoleteStats ammo)
        {
            var weaponDPS = WeaponDPS(data, ammo, false);
            var weaponMDPS = WeaponDPS(data, ammo, true);
            var weaponEPS = WeaponEPS(data, ammo, false);
            var weaponMEPS = WeaponEPS(data, ammo, true);
            var weaponefficiency = weaponDPS / (weaponEPS + 1);
            var weaponefficiency_M = weaponMDPS / (weaponMEPS + 1);

            var ammunitionscore = ammo.GetAmmunitionScore(1);
            
            var score = Mathf.Sqrt(weaponDPS * weaponDPS + weaponMDPS * weaponMDPS) / 2 + Mathf.Sqrt(weaponefficiency * weaponefficiency + weaponefficiency_M * weaponefficiency_M) / 2 + ammunitionscore / 2;
            return score;
        }
        public static float WeaponDPS(this IWeaponData data, bool M)
        {
            float damages = data.Ammunition.GetAmmunitionDamage();
            float firerate = data.Weapon.Stats.FireRate;
            float magazine = Mathf.Max(data.Weapon.Stats.Magazine, 1);
            if (data.Weapon.Stats.WeaponClass == WeaponClass.MashineGun || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avdemage = damages * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exdamage = exround * magazine * damages;

                if (data.Ammunition.Body.Type == BulletType.Continuous)
                    return (M ? exdamage : avdemage) * data.Ammunition.Body.Lifetime;
                return M ? exdamage : avdemage;
            }
            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous )
                return damages;
            if (data.Ammunition.Body.Type == BulletType.Continuous)
                return damages * firerate * magazine * data.Ammunition.Body.Lifetime;
            return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * damages;
        }
        public static float WeaponDPS(this IWeaponDataObsolete data, bool M)
        {
            float damages = data.Ammunition.GetObsoleteAmmunitionDamage();
            float firerate = data.Weapon.FireRate;
            float magazine = Mathf.Max(data.Weapon.Magazine, 1);
            if (data.Weapon.WeaponClass == WeaponClass.MashineGun || data.Weapon.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avdemage = damages * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exdamage = exround * magazine * damages;

                if (data.Ammunition.AmmunitionClass.IsBeam())
                    return (M ? exdamage : avdemage) * data.Ammunition.LifeTime;
                return M ? exdamage : avdemage;
            }
            if (data.Weapon.WeaponClass == WeaponClass.Continuous)
                return damages;
            if (data.Ammunition.AmmunitionClass.IsBeam())
                return damages * firerate * magazine * data.Ammunition.LifeTime;
            return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * damages;
        }
        public static float WeaponEPS(this IWeaponData data, bool M)
        {
            float energy = data.Ammunition.Body.EnergyCost;
            float firerate = data.Weapon.Stats.FireRate;
            float magazine = Mathf.Max(data.Weapon.Stats.Magazine, 1);
            if (data.Weapon.Stats.WeaponClass == WeaponClass.MashineGun || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avenergy = energy * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exenergy = exround * magazine * energy;

                if (data.Ammunition.Body.Type == BulletType.Continuous)
                    return M ? exenergy : avenergy;
                return M ? exenergy : avenergy;
            }
            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
                return energy;
            if (data.Ammunition.Body.Type == BulletType.Continuous)
                return energy * firerate * magazine;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * energy;
        }
        public static float WeaponEPS(this IWeaponDataObsolete data, bool M)
        {
            float energy = data.Ammunition.EnergyCost;
            float firerate = data.Weapon.FireRate;
            float magazine = Mathf.Max(data.Weapon.Magazine, 1);
            if (data.Weapon.WeaponClass == WeaponClass.MashineGun || data.Weapon.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avenergy = energy * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exenergy = exround * magazine * energy;

                if (data.Ammunition.AmmunitionClass.IsBeam())
                    return M ? exenergy : avenergy;
                return M ? exenergy : avenergy;
            }
            if (data.Weapon.WeaponClass == WeaponClass.Continuous)
                return energy;
            if (data.Ammunition.AmmunitionClass.IsBeam())
                return energy * firerate * magazine;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * energy;
        }
        public static float WeaponDPS(this WeaponData data, bool M)
        {
            float damages = data.Ammunition.GetAmmunitionDamage();
            float firerate = data.Weapon.Stats.FireRate;
            float magazine = Mathf.Max(data.Weapon.Stats.Magazine, 1);
            if (data.Weapon.Stats.WeaponClass == WeaponClass.MashineGun || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avdemage = damages * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exdamage = exround * magazine * damages;

                if (data.Ammunition.Body.Type == BulletType.Continuous)
                    return (M ? exdamage : avdemage) * data.Ammunition.Body.Lifetime;
                return M ? exdamage : avdemage;
            }
            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
                return damages;
            if (data.Ammunition.Body.Type == BulletType.Continuous)
                return damages * firerate * magazine * data.Ammunition.Body.Lifetime;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * damages;
        }
        public static float WeaponDPS(WeaponStats data, AmmunitionObsoleteStats ammo, bool M)
        {
            float damages = ammo.GetObsoleteAmmunitionDamage();
            float firerate = data.FireRate;
            float magazine = Mathf.Max(data.Magazine, 1);
            if (data.WeaponClass == WeaponClass.MashineGun || data.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avdemage = damages * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exdamage = exround * magazine * damages;

                if (ammo.AmmunitionClass.IsBeam())
                    return (M ? exdamage : avdemage) * ammo.LifeTime;
                return M ? exdamage : avdemage;
            }
            if (data.WeaponClass == WeaponClass.Continuous)
                return damages;
            if (ammo.AmmunitionClass.IsBeam())
                return damages * firerate * magazine * ammo.LifeTime;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * damages;
        }
        public static float WeaponEPS(this WeaponData data, bool M)
        {
            float energy = data.Ammunition.Body.EnergyCost;
            float firerate = data.Weapon.Stats.FireRate;
            float magazine = Mathf.Max(data.Weapon.Stats.Magazine, 1);
            if (data.Weapon.Stats.WeaponClass == WeaponClass.MashineGun || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.Weapon.Stats.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avenergy = energy * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exenergy = exround * magazine * energy;

                if (data.Ammunition.Body.Type == BulletType.Continuous)
                    return M ? exenergy : avenergy;
                return M ? exenergy : avenergy;
            }
            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
                return energy;
            if (data.Ammunition.Body.Type == BulletType.Continuous)
                return energy * firerate * magazine;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * energy;
        }
        public static float WeaponEPS(WeaponStats data, AmmunitionObsoleteStats ammo, bool M)
        {
            float energy = ammo.EnergyCost;
            float firerate = data.FireRate;
            float magazine = Mathf.Max(data.Magazine, 1);
            if (data.WeaponClass == WeaponClass.MashineGun || data.WeaponClass == WeaponClass.BarrageMachineGun_Swing || data.WeaponClass == WeaponClass.BarrageMachineGun_SwingUnilateral)
            {
                var firetime = magazine / 50;
                var coldtime = 1 / firerate;
                var alltime = firetime + coldtime;
                var deltafirerate = magazine / alltime;
                var avenergy = energy * deltafirerate;

                int round = Mathf.FloorToInt(1 / alltime);
                var exround = round > 0 ? round + (1 - alltime * round) / firetime : 1;
                var exenergy = exround * magazine * energy;

                if (ammo.AmmunitionClass.IsBeam())
                    return M ? exenergy : avenergy;
                return M ? exenergy : avenergy;
            }
            if (data.WeaponClass == WeaponClass.Continuous)
                return energy;
            if (ammo.AmmunitionClass.IsBeam())
                return energy * firerate * magazine;
            else
                return (M ? Mathf.Max(firerate, 1) : firerate) * magazine * energy;
        }
        private static float GetAmmunitionScore(this Ammunition data,float mulitplier, int iteration = 0)
        {
            float damages = 0;
            foreach (var damage in data.Effects.Where(item => item.Type == ImpactEffectType.Damage))
                damages += damage.Power;
            damages *= mulitplier;

            var Velocity = data.Body.Velocity;
            var Range = data.Body.Range;
            var Lifetime = data.Body.Lifetime > 0 ? data.Body.Lifetime : Velocity > 0 ? Range / Velocity : 0;
            var Weight = data.Body.Weight;
            var HitPoints = data.Body.HitPoints;

            float score = 0;
            switch (data.Body.Type)
            {
                case BulletType.Homing:
                    score = damages + Mathf.Pow(Lifetime, 1.8f) + Mathf.Pow(Velocity, 1.8f) + Mathf.Pow(Velocity / (Weight + 0.1f) + 1, 1.1f);
                    break;
                case BulletType.IntelligentHoming:
                    score = damages * 1.05f + Mathf.Pow(Lifetime, 1.8f) + Mathf.Pow(Velocity, 1.8f) + Mathf.Pow(Velocity / (Weight + 0.1f) + 1, 1.1f);
                    break;
                case BulletType.RandomSteering:
                    score = damages + Mathf.Pow(Lifetime, 2f) + Mathf.Pow(Velocity, 1.5f);
                    break;
                case BulletType.Continuous:
                    score = damages + Mathf.Pow(Range, 1.5f) + Mathf.Pow(Lifetime, 2.5f);
                    break;
                case BulletType.Static:
                    score = damages + Mathf.Pow(Lifetime, 2f);
                    break;
                default:
                    score = damages + Mathf.Pow(Range, 1.2f) + Mathf.Pow(Velocity, 1.5f);
                    break;
            }
            var triggers = data.Triggers.Where(item => item.EffectType == BulletEffectType.SpawnBullet);
            if (!triggers.IsEmpty())
            {
                foreach (var trigger in triggers.OfType<BulletTrigger_SpawnBullet>())
                {
                    if (trigger.Ammunition != null)
                    {
                        var maxNestingLevel = trigger.MaxNestingLevel > 0 ? trigger.MaxNestingLevel : 100;
                        if (iteration >= maxNestingLevel)
                            return 0;
                        var spawmbullet = trigger.Ammunition;
                        float mul = trigger.PowerMultiplier > 0 ? trigger.PowerMultiplier : 1;
                        score += trigger.Quantity * spawmbullet.GetAmmunitionScore(mul, iteration + 1);
                    }
                }
            }
            return score;
        }
        private static float GetAmmunitionScore(this AmmunitionObsoleteStats data,float mulitplier)
        {
            var damage = data.Damage * mulitplier;

            var Impulse = data.Impulse;
            var Recoil = data.Recoil;
            var Velocity = data.Velocity;
            var Range = data.Range;
            var Lifetime = data.LifeTime > 0 ? data.LifeTime : Velocity > 0 ? Range / Velocity : 0;
            var HitPoints = data.HitPoints;
            var AreaOfEffect = data.AreaOfEffect;

            float score = 0;
            switch (data.AmmunitionClass)
            {
                case AmmunitionClassObsolete.Explosion:
                case AmmunitionClassObsolete.Bomb:
                    score = damage + Mathf.Pow(AreaOfEffect, 3f);
                    break;
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.PlasmaWeb:
                    score = damage + Mathf.Pow(Lifetime, 1.8f) + Mathf.Pow(AreaOfEffect, 3f);
                    break;
                case AmmunitionClassObsolete.Emp:
                case AmmunitionClassObsolete.Fireworks:
                case AmmunitionClassObsolete.FragBomb:
                case AmmunitionClassObsolete.Fragment:
                case AmmunitionClassObsolete.UnguidedRocket:
                    score = damage + Mathf.Pow(Range, 1.2f) + Mathf.Pow(Velocity, 1.5f) + Mathf.Pow(AreaOfEffect, 2.5f);
                    break;
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.HomingImmobilizer:
                case AmmunitionClassObsolete.HomingTorpedo:
                case AmmunitionClassObsolete.HomingCarrier:
                case AmmunitionClassObsolete.ClusterMissile:
                case AmmunitionClassObsolete.AcidRocket:
                    score = damage + Mathf.Pow(Lifetime, 1.8f) + Mathf.Pow(Velocity, 1.8f) + Mathf.Pow(AreaOfEffect, 2.5f);
                    break;
                case AmmunitionClassObsolete.IntelligentRocket:
                    score = damage * 1.05f + Mathf.Pow(Lifetime, 1.8f) + Mathf.Pow(Velocity, 1.8f);
                    break;
                case AmmunitionClassObsolete.RandomRocket:
                    score = damage + Mathf.Pow(Lifetime, 2f) + Mathf.Pow(Velocity, 1.5f);
                    break;
                case AmmunitionClassObsolete.Acid:
                case AmmunitionClassObsolete.DamageOverTime:
                case AmmunitionClassObsolete.EnergyBeam:
                case AmmunitionClassObsolete.EnergySiphon:
                case AmmunitionClassObsolete.LaserBeam:
                case AmmunitionClassObsolete.TractorBeam:
                case AmmunitionClassObsolete.VampiricRay:
                case AmmunitionClassObsolete.SmallVampiricRay:
                case AmmunitionClassObsolete.RepairRay:
                    score = damage + Mathf.Pow(Range, 1.5f) + Mathf.Pow(Lifetime, 2.5f);
                    break;
                case AmmunitionClassObsolete.BlackHole:
                    score = damage + Mathf.Pow(Lifetime, 2f);
                    break;
                case AmmunitionClassObsolete.Common:
                case AmmunitionClassObsolete.Carrier:
                case AmmunitionClassObsolete.DroneControl:
                case AmmunitionClassObsolete.Immobilizer:
                case AmmunitionClassObsolete.Singularity:
                case AmmunitionClassObsolete.IonBeam:
                default:
                    score = damage + Mathf.Pow(Range, 1.2f) + Mathf.Pow(Velocity, 1.5f);
                    break;
            }
            if(data.CoupledAmmunition!=null)
            {
                score += data.CoupledAmmunition.Stats.GetAmmunitionScore(1);
            }
            return score;
        }
        private static float GetAmmunitionDamage(this Ammunition data, int iteration = 0)
        {
            float damages = 0;
            foreach (var damage in data.Effects.Where(item => item.Type == ImpactEffectType.Damage))
                damages += damage.Power;

            //Debug.Log("damages : " + damages);

            var triggers = data.Triggers.Where(item => item.EffectType == GameDatabase.Enums.BulletEffectType.SpawnBullet);
            if (triggers.IsEmpty())
                return damages;
            else
            {
                foreach (var trigger in triggers.OfType<BulletTrigger_SpawnBullet>())
                {
                    if (trigger.Ammunition != null)
                    {
                        var maxNestingLevel = trigger.MaxNestingLevel > 0 ? trigger.MaxNestingLevel : 100;
                        if (iteration >= maxNestingLevel)
                            return 0;
                        var spawmbullet = trigger.Ammunition;
                        float mul = trigger.PowerMultiplier > 0 ? trigger.PowerMultiplier : 1;
                        damages += trigger.Quantity * mul * spawmbullet.GetAmmunitionDamage(iteration + 1);
                    }
                }
                return damages;
            }
        }
        private static float GetObsoleteAmmunitionDamage(this AmmunitionObsoleteStats data)
        {
            if (data.CoupledAmmunition == null)
                return data.Damage;
            else
                return data.Damage + data.CoupledAmmunition.Stats.GetObsoleteAmmunitionDamage();
        }
        public static float DronesScore(IShipSpecification Specification, ShipSettings setting)
        {
            float score = 0;
            foreach (var drone in Specification.DroneBays)
                score += drone.Drone.DroneScore(setting);
            return score;
        }
        public static float DroneScore(this ShipBuild data, ShipSettings setting)
        {
            var spec = new ShipBuilder(data).Build(setting);
            var stats = spec.Stats;
            var armordefensescore = ArmorDefenseScore(stats);
            var armorendurancescore = ArmorEnduranceScore(stats);
            var shielddefensescore = ShieldDefenseScore(stats);
            var shieldendurancescore = ShieldEnduranceScore(stats);
            var energyshielddefensescore = EnergyShieldDefenseScore(stats);
            var energyshieldendurancescore = EnergyShieldEnduranceScore(stats);
            var energyscore = EnergyScore(stats);
            var energyendurancescore = EnergyEnduranceScore(stats);
            var enginescore = EngineScore(stats);
            var devicesscore = DevicesScore(spec);
            var weaponsscore = WeaponsScore(spec);

            var DEFscore = armordefensescore * 2 + shielddefensescore + energyshielddefensescore;
            var DEFendurancescore = armorendurancescore * 2 + shieldendurancescore + energyshieldendurancescore;
            var ENEscore = energyscore + energyendurancescore;
            var ENGscore = enginescore;
            var DEVscore = devicesscore;
            var ATTscore = weaponsscore * 2;

            var score = DEFscore + DEFendurancescore + ENEscore + ENGscore + DEVscore + ATTscore;
            return score / 5;
        }
        public static float DroneBaysScore(IShipSpecification Specification, ShipSettings setting)
        {
            float score = 0;
            foreach (var bay in Specification.DroneBays)
                score += bay.DroneBayScore(setting);
            return score;
        }
        public static float DroneBayScore(this IDroneBayData data, ShipSettings setting)
        {
            var dronescore = data.Drone.DroneScore(setting);

            var EnergyConsumption = data.DroneBay.EnergyConsumption;
            var PassiveEnergyConsumption = data.DroneBay.PassiveEnergyConsumption;
            var Range = data.DroneBay.Range;
            var DamageMultiplier = data.DroneBay.DamageMultiplier;
            var DefenseMultiplier = data.DroneBay.DefenseMultiplier;
            var SpeedMultiplier = data.DroneBay.SpeedMultiplier;
            var ImprovedAi = data.DroneBay.ImprovedAi ? 1.2f : 1;
            var Capacity = data.DroneBay.Capacity;
            var Squadron = data.DroneBay.Squadron > 0 ? 1.5f : 1;

            var score = dronescore * Range / (PassiveEnergyConsumption + 1) / (EnergyConsumption + 1) * Mathf.Sqrt(DamageMultiplier * DefenseMultiplier * SpeedMultiplier) * Capacity * Squadron * ImprovedAi;
            return score;
        }
        public static float DroneBayScore(DroneBayStats data,ShipBuild uav, ShipSettings setting)
        {
            var dronescore = uav.DroneScore(setting);

            var EnergyConsumption = data.EnergyConsumption;
            var PassiveEnergyConsumption = data.PassiveEnergyConsumption;
            var Range = data.Range;
            var DamageMultiplier = data.DamageMultiplier;
            var DefenseMultiplier = data.DefenseMultiplier;
            var SpeedMultiplier = data.SpeedMultiplier;
            var ImprovedAi = data.ImprovedAi ? 1.2f : 1;
            var Capacity = data.Capacity;
            var Squadron = data.Squadron > 0 ? 1.5f : 1;

            var score = dronescore * Range / (PassiveEnergyConsumption + 1) / (EnergyConsumption + 1) * Mathf.Sqrt(DamageMultiplier * DefenseMultiplier * SpeedMultiplier) * Capacity * Squadron * ImprovedAi;
            return score;
        }
        public static float UAVplatformsScore(IShipSpecification Specification, ShipSettings setting)
        {
            var dronescore = new float[Specification.DroneBays.Count()];
            var count = 0;
            foreach (var bay in Specification.DroneBays)
                dronescore[count++] = bay.DroneBayScore(setting);

            var platformcount = Mathf.Max(Specification.UAVPlatforms.Count(), 1);
            if (Specification.DroneBays.Count() <= platformcount)
                return dronescore.Sum();
            else
            {
                var score = Generic.ArrayMaxValue(dronescore, platformcount);
                return score.Sum();
            }
        }
        public static float GetShipComprehensivePower(IShip ship, ShipSettings setting)
        {
            var spec = ship.CreateBuilder().Build(setting);
            var stats = spec.Stats;
            var armordefensescore = ArmorDefenseScore(stats);
            var armorendurancescore = ArmorEnduranceScore(stats);
            var shielddefensescore = ShieldDefenseScore(stats);
            var shieldendurancescore = ShieldEnduranceScore(stats);
            var energyshielddefensescore = EnergyShieldDefenseScore(stats);
            var energyshieldendurancescore = EnergyShieldEnduranceScore(stats);
            var energyscore = EnergyScore(stats);
            var energyendurancescore = EnergyEnduranceScore(stats);
            var enginescore = EngineScore(stats);
            var devicesscore = DevicesScore(spec);
            var weaponsscore = WeaponsScore(spec);
            //var dronesscore = DronesScore(spec, setting);
            var dronebaysscore = DroneBaysScore(spec, setting);
            var uavplatformsscore = UAVplatformsScore(spec, setting);

            var DEFscore = armordefensescore * 2 + shielddefensescore + energyshielddefensescore;
            var DEFendurancescore = armorendurancescore * 2 + shieldendurancescore + energyshieldendurancescore;
            var ENEscore = energyscore + energyendurancescore;
            var ENGscore = enginescore;
            var DEVscore = devicesscore;
            var ATTscore = weaponsscore * 2;
            var UAVscore = dronebaysscore + uavplatformsscore;

            var score = DEFscore + DEFendurancescore + ENEscore + ENGscore + DEVscore + ATTscore + UAVscore;
            if (ShipDebugLogSetting.ShipPowerDebugLog)
            {
                Debug.Log("Ship Power:  " + ship.Name + "    armordefensescore:  " + armordefensescore);
                Debug.Log("Ship Power:  " + ship.Name + "    armorendurancescore:  " + armorendurancescore);
                Debug.Log("Ship Power:  " + ship.Name + "    shielddefensescore:  " + shielddefensescore);
                Debug.Log("Ship Power:  " + ship.Name + "    shieldendurancescore:  " + shieldendurancescore);
                Debug.Log("Ship Power:  " + ship.Name + "    energyshielddefensescore:  " + energyshielddefensescore);
                Debug.Log("Ship Power:  " + ship.Name + "    energyshieldendurancescore:  " + energyshieldendurancescore);
                Debug.Log("Ship Power:  " + ship.Name + "    energyscore:  " + energyscore);
                Debug.Log("Ship Power:  " + ship.Name + "    energyendurancescore:  " + energyendurancescore);
                Debug.Log("Ship Power:  " + ship.Name + "    enginescore:  " + enginescore);
                Debug.Log("Ship Power:  " + ship.Name + "    devicesscore:  " + devicesscore);
                Debug.Log("Ship Power:  " + ship.Name + "    weaponsscore:  " + weaponsscore);
                Debug.Log("Ship Power:  " + ship.Name + "    dronebaysscore:  " + dronebaysscore);
                Debug.Log("Ship Power:  " + ship.Name + "    uavplatformsscore:  " + uavplatformsscore);
            }
            Debug.Log("Ship Power:  " + ship.Name + "    Score:  " + score);
            return score * (1f + 0.5f * (int)ship.ExtraThreatLevel) * (1 + (int)ship.ExtraEnhanceLevel * 0.05f) / 6;
        }
        public static float GetShipsComprehensivePower(IEnumerable<IShip> ships, ShipSettings setting)
        {
            var power = 0f;
            foreach (var ship in ships)
                power += GetShipComprehensivePower(ship, setting);

            return power;
        }
    }
}