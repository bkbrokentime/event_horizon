using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Constructor.Model
{
    public interface IShipStats
    {
        ColorScheme ShipColor { get; }
        ColorScheme TurretColor { get; }

        StatMultiplier DamageMultiplier { get; }
        StatMultiplier ArmorMultiplier { get; }
        StatMultiplier ShieldMultiplier { get; }
        StatMultiplier EnergyShieldMultiplier { get; }

        float ArmorPoints { get; }
        float EnergyPoints { get; }
        float ShieldPoints { get; }

        float EnergyShieldPoints { get; }

        float EnergyRechargeRate { get; }
        float ShieldRechargeRate { get; }
        float EnergyShieldRechargeRate { get; }
        float ArmorRepairRate { get; }

        Layout Layout { get; }
        Layout SecondLayout { get; }
        float Weight { get; }
        float EnginePower { get; }
        float TurnRate { get; }
        float EngineMAXPower { get; }
        float TurnMAXRate { get; }
        float NOLIMITEnginePower { get; }
        float NOLIMITTurnRate { get; }
        float NOLIMITEngineMAXPower { get; }
        float NOLIMITTurnMAXRate { get; }

        StatMultiplier WeaponDamageMultiplier { get; }
        StatMultiplier WeaponFireRateMultiplier { get; }
        StatMultiplier WeaponEnergyCostMultiplier { get; }
        StatMultiplier WeaponRangeMultiplier { get; }

        StatMultiplier DroneDamageMultiplier { get; }
        StatMultiplier DroneDefenseMultiplier { get; }
        StatMultiplier DroneSpeedMultiplier { get; }
        StatMultiplier DroneRangeMultiplier { get; }

        float EnergyAbsorption { get; }
        float RammingDamage { get; }
        float RammingDamageMultiplier { get; }
        float ExpectRammingDamage { get; }

        float ArmorRepairCooldown { get; }
        float EnergyRechargeCooldown { get; }
        float ShieldRechargeCooldown { get; }
        float EnergyShieldRechargeCooldown { get; }

        float ArmorPointsAttenuatableRate { get; }
        float ArmorRepairAttenuatableRate { get; }
        float EnergyPointsAttenuatableRate { get; }
        float EnergyRechargeAttenuatableRate { get; }
        float ShieldPointsAttenuatableRate { get; }
        float ShieldRechargeAttenuatableRate { get; }
        float EnergyShieldPointsAttenuatableRate { get; }
        float EnergyShieldRechargeAttenuatableRate { get; }

        float EnergyResistance { get; }
        float KineticResistance { get; }
        float ThermalResistance { get; }
        float QuantumResistance { get; }

        float ShieldEnergyResistance { get; }
        float ShieldKineticResistance { get; }
        float ShieldThermalResistance { get; }
        float ShieldQuantumResistance { get; }

        float EnergyShieldEnergyResistance { get; }
        float EnergyShieldKineticResistance { get; }
        float EnergyShieldThermalResistance { get; }
        float EnergyShieldQuantumResistance { get; }

        float EnergyAbsorptionPercentage { get; }
        float KineticResistancePercentage { get; }
        float EnergyResistancePercentage { get; }
        float ThermalResistancePercentage { get; }
        float QuantumResistancePercentage { get; }

        float ShieldKineticResistancePercentage { get; }
        float ShieldEnergyResistancePercentage { get; }
        float ShieldThermalResistancePercentage { get; }
        float ShieldQuantumResistancePercentage { get; }

        float EnergyShieldKineticResistancePercentage { get; }
        float EnergyShieldEnergyResistancePercentage { get; }
        float EnergyShieldThermalResistancePercentage { get; }
        float EnergyShieldQuantumResistancePercentage { get; }

        bool Autopilot { get; }
        bool TargetingSystem { get; }

        float DroneBuildSpeed { get; }
        float DroneBuildTime { get; }

        SpriteId IconImage { get; }
        SpriteId ModelImage { get; }
        float ModelScale { get; }
        Color EngineColor { get; }
        ShipCategory ShipCategory { get; }
        IEnumerable<Engine> Engines { get; }
    }

    public class ShipStatsCalculator : IShipStats
    {
        public ShipStatsCalculator(Ship ship, ShipSettings settings)
        {
            _ship = ship;
            ShipSettings = settings;
        }

        public ShipSettings ShipSettings { get; }

        public ShipBaseStats BaseStats;
        public ShipEquipmentStats EquipmentStats;
        public ShipBonuses Bonuses;

        public ColorScheme ShipColor { get; set; }
        public ColorScheme TurretColor { get; set; }

        public StatMultiplier DamageMultiplier => Bonuses.DamageMultiplier;
        public StatMultiplier ArmorMultiplier => Bonuses.ArmorPointsMultiplier;
        public StatMultiplier ShieldMultiplier => Bonuses.ShieldPointsMultiplier;
        public StatMultiplier EnergyShieldMultiplier => Bonuses.EnergyShieldPointsMultiplier;

        public Layout Layout => BaseStats.Layout;
        public Layout SecondLayout => BaseStats.SecondLayout;

        public int CellCount => BaseStats.Layout.CellCount;
        public int SecondLayoutCellCount => BaseStats.SecondLayout.CellCount;

        public ImmutableCollection<Device> BuiltinDevices => BaseStats.BuiltinDevices;

        public float ArmorPoints => ArmorPointsWithoutBonuses * Bonuses.ArmorPointsMultiplier.Value;

        private float ArmorPointsWithoutBonuses
        {
            get
            {
                var basePoints = (ShipSettings.BaseArmorPoints + ShipSettings.ArmorPointsPerCell * CellCount) * BaseStats.BaseArmorMultiplier.Value;
                var totalPoints = basePoints + EquipmentStats.ArmorPoints;
                return totalPoints >= 1 ? totalPoints : 0;
            }
        }

        public float EnergyPoints => (ShipSettings.BaseEnergyPoints + EquipmentStats.EnergyPoints) * Bonuses.EnergyMultiplier.Value;
        public float ShieldPoints => EquipmentStats.ShieldPoints * Bonuses.ShieldPointsMultiplier.Value;
        public float EnergyShieldPoints => EquipmentStats.EnergyShieldPoints * Bonuses.EnergyShieldPointsMultiplier.Value;

        public float EnergyRechargeRate => EquipmentStats.EnergyRechargeRate + ShipSettings.BaseEnergyRechargeRate;
        public float ShieldRechargeRate => (EquipmentStats.ShieldRechargeRate + ShipSettings.BaseShieldRechargeRate) * Bonuses.ShieldPointsMultiplier.Value * Bonuses.ShieldRechargeMultiplier.Value;
        public float EnergyShieldRechargeRate => (EquipmentStats.EnergyShieldRechargeRate + ShipSettings.BaseEnergyShieldRechargeRate) * Bonuses.EnergyShieldPointsMultiplier.Value * Bonuses.EnergyShieldRechargeMultiplier.Value;

        public float ArmorRepairRate
        {
            get
            {
                var regeneration = _ship.Regeneration ? 0.01f + BaseStats.RegenerationRate : BaseStats.RegenerationRate;
                return EquipmentStats.ArmorRepairRate * Bonuses.ArmorPointsMultiplier.Value/* + Mathf.Max(0, ArmorPoints * regeneration)*/ + ArmorPoints * regeneration;
            }
        }

        public float EnergyResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyResistance + (_ship.EnergyResistance + Bonuses.ExtraEnergyResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.EnergyResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.EnergyResistanceMultiplier.Value;
            }
        }

        public float KineticResistance
        {
            get
            {
                var resistance = EquipmentStats.KineticResistance + (_ship.KineticResistance + Bonuses.ExtraKineticResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.KineticResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.KineticResistanceMultiplier.Value;
            }
        }

        public float ThermalResistance
        {
            get
            {
                var resistance = EquipmentStats.ThermalResistance + (_ship.HeatResistance + Bonuses.ExtraHeatResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.ThermalResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.HeatResistanceMultiplier.Value;
            }
        }
        public float QuantumResistance
        {
            get
            {
                var resistance = EquipmentStats.QuantumResistance + (_ship.QuantumResistance + Bonuses.ExtraQuantumResistance) * (ArmorPointsWithoutBonuses + EquipmentStats.QuantumResistance);
                return resistance * Bonuses.ArmorPointsMultiplier.Value * BaseStats.QuantumResistanceMultiplier.Value;
            }
        }

        public float ShieldEnergyResistance
        {
            get
            {
                var resistance = EquipmentStats.ShieldEnergyResistance + (_ship.ShieldEnergyResistance + Bonuses.ExtraShieldEnergyResistance) * (0 + EquipmentStats.ShieldEnergyResistance);
                return resistance * Bonuses.ShieldPointsMultiplier.Value * BaseStats.ShieldEnergyResistanceMultiplier.Value;
            }
        }

        public float ShieldKineticResistance
        {
            get
            {
                var resistance = EquipmentStats.ShieldKineticResistance + (_ship.ShieldKineticResistance + Bonuses.ExtraShieldKineticResistance) * (0 + EquipmentStats.ShieldKineticResistance);
                return resistance * Bonuses.ShieldPointsMultiplier.Value * BaseStats.ShieldKineticResistanceMultiplier.Value;
            }
        }

        public float ShieldThermalResistance
        {
            get
            {
                var resistance = EquipmentStats.ShieldThermalResistance + (_ship.ShieldHeatResistance + Bonuses.ExtraShieldHeatResistance) * (0 + EquipmentStats.ShieldThermalResistance);
                return resistance * Bonuses.ShieldPointsMultiplier.Value * BaseStats.ShieldHeatResistanceMultiplier.Value;
            }
        }
        public float ShieldQuantumResistance
        {
            get
            {
                var resistance = EquipmentStats.ShieldQuantumResistance + (_ship.ShieldQuantumResistance + Bonuses.ExtraShieldQuantumResistance) * (0 + EquipmentStats.ShieldQuantumResistance);
                return resistance * Bonuses.ShieldPointsMultiplier.Value * BaseStats.ShieldQuantumResistanceMultiplier.Value;
            }
        }

        public float EnergyShieldEnergyResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyShieldEnergyResistance + (_ship.EnergyShieldEnergyResistance + Bonuses.ExtraEnergyShieldEnergyResistance) * (0 + EquipmentStats.EnergyShieldEnergyResistance);
                return resistance * Bonuses.EnergyShieldPointsMultiplier.Value * BaseStats.EnergyShieldEnergyResistanceMultiplier.Value;
            }
        }

        public float EnergyShieldKineticResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyShieldKineticResistance + (_ship.EnergyShieldKineticResistance + Bonuses.ExtraEnergyShieldKineticResistance) * (0 + EquipmentStats.EnergyShieldKineticResistance);
                return resistance * Bonuses.EnergyShieldPointsMultiplier.Value * BaseStats.EnergyShieldKineticResistanceMultiplier.Value;
            }
        }

        public float EnergyShieldThermalResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyShieldThermalResistance + (_ship.EnergyShieldHeatResistance + Bonuses.ExtraEnergyShieldHeatResistance) * (0 + EquipmentStats.EnergyShieldThermalResistance);
                return resistance * Bonuses.EnergyShieldPointsMultiplier.Value * BaseStats.EnergyShieldHeatResistanceMultiplier.Value;
            }
        }
        public float EnergyShieldQuantumResistance
        {
            get
            {
                var resistance = EquipmentStats.EnergyShieldQuantumResistance + (_ship.EnergyShieldQuantumResistance + Bonuses.ExtraEnergyShieldQuantumResistance) * (0 + EquipmentStats.EnergyShieldQuantumResistance);
                return resistance * Bonuses.EnergyShieldPointsMultiplier.Value * BaseStats.EnergyShieldQuantumResistanceMultiplier.Value;
            }
        }

        public float ArmorRepairCooldown => (EquipmentStats.ArmorRepairBaseCooldown + ShipSettings.ArmorRepairCooldown) * Mathf.Max(EquipmentStats.ArmorRepairCooldownMultiplier.Value, 0);
        public float EnergyRechargeCooldown => (EquipmentStats.EnergyRechargeBaseCooldown + ShipSettings.EnergyRechargeCooldown) * Mathf.Max(EquipmentStats.EnergyRechargeCooldownMultiplier.Value, 0);
        public float ShieldRechargeCooldown => (EquipmentStats.ShieldRechargeBaseCooldown + ShipSettings.ShieldRechargeCooldown) * Mathf.Max(EquipmentStats.ShieldRechargeCooldownMultiplier.Value, 0);
        public float EnergyShieldRechargeCooldown => (EquipmentStats.EnergyShieldRechargeBaseCooldown + ShipSettings.EnergyShieldRechargeCooldown) * Mathf.Max(EquipmentStats.EnergyShieldRechargeCooldownMultiplier.Value, 0);
        public float ArmorPointsAttenuatableRate => EquipmentStats.ArmorPointsAttenuatableRate + _ship.ArmorPointsAttenuatableRate;
        public float ArmorRepairAttenuatableRate => EquipmentStats.ArmorRepairAttenuatableRate + _ship.ArmorRepairAttenuatableRate;
        public float EnergyPointsAttenuatableRate => EquipmentStats.EnergyPointsAttenuatableRate + _ship.EnergyPointsAttenuatableRate;
        public float EnergyRechargeAttenuatableRate => EquipmentStats.EnergyRechargeAttenuatableRate + _ship.EnergyRechargeAttenuatableRate;
        public float ShieldPointsAttenuatableRate => EquipmentStats.ShieldPointsAttenuatableRate + _ship.ShieldPointsAttenuatableRate;
        public float ShieldRechargeAttenuatableRate => EquipmentStats.ShieldRechargeAttenuatableRate + _ship.ShieldRechargeAttenuatableRate;
        public float EnergyShieldPointsAttenuatableRate => EquipmentStats.EnergyShieldPointsAttenuatableRate + _ship.EnergyShieldPointsAttenuatableRate;
        public float EnergyShieldRechargeAttenuatableRate => EquipmentStats.EnergyShieldRechargeAttenuatableRate + _ship.EnergyShieldRechargeAttenuatableRate;

        public float EnergyAbsorptionPercentage => EnergyAbsorption * (1 + EnergyResistancePercentage) / (ArmorPoints + EnergyAbsorption);
        public float KineticResistancePercentage => Mathf.Clamp(KineticResistance / (ArmorPoints + Mathf.Max(0, KineticResistance)), -100, 1);
        public float EnergyResistancePercentage => Mathf.Clamp(EnergyResistance / (ArmorPoints + Mathf.Max(0, EnergyResistance)), -100, 1);
        public float ThermalResistancePercentage => Mathf.Clamp(ThermalResistance / (ArmorPoints + Mathf.Max(0, ThermalResistance)), -100, 1);
        public float QuantumResistancePercentage => Mathf.Clamp(QuantumResistance / (ArmorPoints + Mathf.Max(0, QuantumResistance)), -100, 1);
        public float ShieldKineticResistancePercentage => Mathf.Clamp(ShieldKineticResistance / (ShieldPoints + Mathf.Max(0, ShieldKineticResistance)), -100, 1);
        public float ShieldEnergyResistancePercentage => Mathf.Clamp(ShieldEnergyResistance / (ShieldPoints + Mathf.Max(0, ShieldEnergyResistance)), -100, 1);
        public float ShieldThermalResistancePercentage => Mathf.Clamp(ShieldThermalResistance / (ShieldPoints + Mathf.Max(0, ShieldThermalResistance)), -100, 1);
        public float ShieldQuantumResistancePercentage => Mathf.Clamp(ShieldQuantumResistance / (ShieldPoints + Mathf.Max(0, ShieldQuantumResistance)), -100, 1);
        public float EnergyShieldKineticResistancePercentage => Mathf.Clamp(EnergyShieldKineticResistance / (EnergyShieldPoints + Mathf.Max(0, EnergyShieldKineticResistance)), -100, 1);
        public float EnergyShieldEnergyResistancePercentage => Mathf.Clamp(EnergyShieldEnergyResistance / (EnergyShieldPoints + Mathf.Max(0, EnergyShieldEnergyResistance)), -100, 1);
        public float EnergyShieldThermalResistancePercentage => Mathf.Clamp(EnergyShieldThermalResistance / (EnergyShieldPoints + Mathf.Max(0, EnergyShieldThermalResistance)), -100, 1);
        public float EnergyShieldQuantumResistancePercentage => Mathf.Clamp(EnergyShieldQuantumResistance / (EnergyShieldPoints + Mathf.Max(0, EnergyShieldQuantumResistance)), -100, 1);

        public float Weight
        {
            get
            {
                var multiplier = BaseStats.BaseWeightMultiplier.Value;
                var minWeight = multiplier * ShipSettings.MinimumWeightPerCell * CellCount;
                return Mathf.Max(minWeight, EquipmentStats.Weight + ShipSettings.DefaultWeightPerCell * CellCount * multiplier);
            }
        }

        public float EnginePower
        {
            get
            {
                var enginePower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower * 1000f;
                return Mathf.Clamp(enginePower / Weight, 0, ShipSettings.MaxVelocity / 2);
            }
        }

        public float TurnRate
        {
            get
            {
                var turnRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate * 1000f;
                return Mathf.Clamp(turnRate / Weight / (1 + _ship.ModelScale), 0, ShipSettings.MaxTurnRate * Mathf.PI / 10);
            }
        }
        public float NOLIMITEnginePower
        {
            get
            {
                var enginePower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower * 1000f;
                return Mathf.Clamp(enginePower / Weight, 0, float.MaxValue);
            }
        }

        public float NOLIMITTurnRate
        {
            get
            {
                var turnRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate * 1000f;
                return Mathf.Clamp(turnRate / Weight / (1 + _ship.ModelScale), 0, float.MaxValue);
            }
        }
        public float EngineMAXPower
        {
            get
            {
                var engineMAXPower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower * 5000f;
                return Mathf.Clamp(Mathf.Sqrt(engineMAXPower * 2f / Weight), 0, ShipSettings.MaxVelocity);
            }
        }

        public float TurnMAXRate
        {
            get
            {
                var turnMAXRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate * 5000f;
                return Mathf.Clamp(Mathf.Sqrt(turnMAXRate * 2f / Weight / (1 + _ship.ModelScale)), 0, ShipSettings.MaxTurnRate * Mathf.PI / 5);
            }
        }
        public float NOLIMITEngineMAXPower
        {
            get
            {
                var engineMAXPower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower * 5000f;
                return Mathf.Clamp(Mathf.Sqrt(engineMAXPower * 2f / Weight), 0, float.MaxValue);
            }
        }

        public float NOLIMITTurnMAXRate
        {
            get
            {
                var turnMAXRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate * 5000f;
                return Mathf.Clamp(Mathf.Sqrt(turnMAXRate * 2f / Weight / _ship.ModelScale / _ship.ModelScale), 0, float.MaxValue);
            }
        }

        /*
        public float Weight
        {
            get
            {
                var multiplier = BaseStats.BaseWeightMultiplier.Value;
                var minWeight = multiplier * ShipSettings.MinimumWeightPerCell * CellCount;
                return Mathf.Max(minWeight, EquipmentStats.Weight + ShipSettings.DefaultWeightPerCell * CellCount * multiplier) / 1000f;
            }
        }

        public float EnginePower
        {
            get
            {
                var enginePower = Bonuses.VelocityMultiplier.Value * EquipmentStats.EnginePower;
                return Mathf.Clamp(enginePower * 50f / (Mathf.Sqrt(Weight) * CellCount), 0, ShipSettings.MaxVelocity);
            }
        }

        public float TurnRate
        {
            get
            {
                var turnRate = Bonuses.VelocityMultiplier.Value * EquipmentStats.TurnRate;
                return Mathf.Clamp(turnRate * 50f / (Mathf.Sqrt(Weight) * CellCount), 0, ShipSettings.MaxTurnRate);
            }
        }

        */

        public bool Autopilot => EquipmentStats.Autopilot;
        public bool TargetingSystem => BaseStats.AutoTargeting;

        public StatMultiplier WeaponDamageMultiplier => EquipmentStats.WeaponDamageMultiplier * Bonuses.DamageMultiplier.Value;
        public StatMultiplier WeaponFireRateMultiplier => EquipmentStats.WeaponFireRateMultiplier;
        public StatMultiplier WeaponEnergyCostMultiplier => EquipmentStats.WeaponEnergyCostMultiplier;
        public StatMultiplier WeaponRangeMultiplier => EquipmentStats.WeaponRangeMultiplier;

        public StatMultiplier DroneDamageMultiplier => EquipmentStats.DroneDamageMultiplier;
        public StatMultiplier DroneDefenseMultiplier => EquipmentStats.DroneDefenseMultiplier;
        public StatMultiplier DroneSpeedMultiplier => EquipmentStats.DroneSpeedMultiplier;
        public StatMultiplier DroneRangeMultiplier => EquipmentStats.DroneRangeMultiplier;

        public float RammingDamage => EquipmentStats.RammingDamage;
        public float EnergyAbsorption => Mathf.Max(0, EquipmentStats.EnergyAbsorption * Bonuses.ArmorPointsMultiplier.Value);

        public float RammingDamageMultiplier
        {
            get
            {
                var armorPoints = ArmorPoints;
                var rammingDamage = EquipmentStats.RammingDamage;
                return Mathf.Max(0, EquipmentStats.RammingDamageMultiplier.Value * Bonuses.RammingDamageMultiplier.Value * (1.0f + Mathf.Pow(rammingDamage * (1 + KineticResistancePercentage) / armorPoints, 0.8f)));
            }
        }
        public float ExpectRammingDamage => Mathf.Pow(0.2f * Mathf.Pow(Weight, 0.5f) * RammingDamageMultiplier * EngineMAXPower, 0.5f);

        public float DroneBuildTime
        {
            get
            {
                var speed = ShipSettings.BaseDroneReconstructionSpeed + EquipmentStats.DroneReconstructionSpeed;
                return speed > 0 ? EquipmentStats.DroneReconstructionTimeMultiplier.Value / speed : 0f;
            }
        }

        public float DroneBuildSpeed
        {
            get
            {
                var speed = ShipSettings.BaseDroneReconstructionSpeed + EquipmentStats.DroneReconstructionSpeed;
                return speed / EquipmentStats.DroneReconstructionTimeMultiplier.Value;
            }
        }

        public float ModelScale => _ship.ModelScale * Bonuses.ScaleMultiplier.Value;
        public SpriteId ModelImage => _ship.ModelImage;
        public SpriteId IconImage => _ship.IconImage;
        public Color EngineColor => _ship.EngineColor;
        public ShipCategory ShipCategory => _ship.ShipCategory;
        public IEnumerable<Engine> Engines => _ship.Engines;

        private readonly Ship _ship;
    }
}
