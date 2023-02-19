using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Constructor.Model
{
    public struct ShipEquipmentStats
    {
        public float ArmorPoints;
        public float ArmorRepairRate;
        public StatMultiplier ArmorRepairCooldownMultiplier;
        public float ArmorPointsAttenuatableRate;
        public float ArmorRepairAttenuatableRate;

        public float EnergyPoints;
        public float EnergyRechargeRate;
        public StatMultiplier EnergyRechargeCooldownMultiplier;
        public float EnergyPointsAttenuatableRate;
        public float EnergyRechargeAttenuatableRate;

        public float ShieldPoints;
        public float ShieldRechargeRate;
        public StatMultiplier ShieldRechargeCooldownMultiplier;
        public float ShieldPointsAttenuatableRate;
        public float ShieldRechargeAttenuatableRate;

        public float EnergyShieldPoints;
        public float EnergyShieldRechargeRate;
        public StatMultiplier EnergyShieldRechargeCooldownMultiplier;
        public float EnergyShieldPointsAttenuatableRate;
        public float EnergyShieldRechargeAttenuatableRate;

        public float ArmorRepairBaseCooldown;
        public float HullRepairBaseCooldown;
        public float EnergyRechargeBaseCooldown;
        public float ShieldRechargeBaseCooldown;
        public float EnergyShieldRechargeBaseCooldown;

        public float Weight;

        public float EnergyAbsorption;
        public float RammingDamage;
        public StatMultiplier RammingDamageMultiplier;

        public float KineticResistance;
        public float EnergyResistance;
        public float ThermalResistance;
        public float QuantumResistance;

        public float ShieldKineticResistance;
        public float ShieldEnergyResistance;
        public float ShieldThermalResistance;
        public float ShieldQuantumResistance;

        public float EnergyShieldKineticResistance;
        public float EnergyShieldEnergyResistance;
        public float EnergyShieldThermalResistance;
        public float EnergyShieldQuantumResistance;

        public float EnginePower;
        public float TurnRate;

        public bool Autopilot;

        public StatMultiplier DroneRangeMultiplier;
        public StatMultiplier DroneDamageMultiplier;
        public StatMultiplier DroneDefenseMultiplier;
        public StatMultiplier DroneSpeedMultiplier;
        public float DroneReconstructionSpeed;
        public StatMultiplier DroneReconstructionTimeMultiplier;

        public StatMultiplier WeaponFireRateMultiplier;
        public StatMultiplier WeaponDamageMultiplier;
        public StatMultiplier WeaponRangeMultiplier;
        public StatMultiplier WeaponEnergyCostMultiplier;

        public static ShipEquipmentStats FromComponent(ComponentStats component, int cellCount)
        {
            var stats = new ShipEquipmentStats();

            var multiplier = component.Type == ComponentStatsType.PerOneCell ? cellCount : 1.0f;

            stats.ArmorPoints = component.ArmorPoints * multiplier;
            stats.ArmorRepairRate = component.ArmorRepairRate * multiplier;
            stats.ArmorRepairCooldownMultiplier = new StatMultiplier(component.ArmorRepairCooldownModifier * multiplier);
            stats.ArmorPointsAttenuatableRate = component.ArmorPointsAttenuatableRate * multiplier;
            stats.ArmorRepairAttenuatableRate = component.ArmorRepairAttenuatableRate * multiplier;

            stats.EnergyPoints = component.EnergyPoints * multiplier;
            stats.EnergyRechargeRate = component.EnergyRechargeRate * multiplier;
            stats.EnergyRechargeCooldownMultiplier = new StatMultiplier(component.EnergyRechargeCooldownModifier * multiplier);
            stats.EnergyPointsAttenuatableRate = component.EnergyPointsAttenuatableRate * multiplier;
            stats.EnergyRechargeAttenuatableRate = component.EnergyRechargeAttenuatableRate * multiplier;

            stats.ShieldPoints = component.ShieldPoints * multiplier;
            stats.ShieldRechargeRate = component.ShieldRechargeRate * multiplier;
            stats.ShieldRechargeCooldownMultiplier = new StatMultiplier(component.ShieldRechargeCooldownModifier * multiplier);
            stats.ShieldPointsAttenuatableRate = component.ShieldPointsAttenuatableRate * multiplier;
            stats.ShieldRechargeAttenuatableRate = component.ShieldRechargeAttenuatableRate * multiplier;

            stats.EnergyShieldPoints = component.EnergyShieldPoints * multiplier;
            stats.EnergyShieldRechargeRate = component.EnergyShieldRechargeRate * multiplier;
            stats.EnergyShieldRechargeCooldownMultiplier = new StatMultiplier(component.EnergyShieldRechargeCooldownModifier * multiplier);
            stats.EnergyShieldPointsAttenuatableRate = component.EnergyShieldPointsAttenuatableRate * multiplier;
            stats.EnergyShieldRechargeAttenuatableRate = component.EnergyShieldRechargeAttenuatableRate * multiplier;

            stats.Weight = multiplier * component.Weight;

            stats.RammingDamage = component.RammingDamage * multiplier;
            stats.EnergyAbsorption = component.EnergyAbsorption * multiplier;

            stats.KineticResistance = component.KineticResistance * multiplier;
            stats.EnergyResistance = component.EnergyResistance * multiplier;
            stats.ThermalResistance = component.ThermalResistance * multiplier;
            stats.QuantumResistance = component.QuantumResistance * multiplier;

            stats.ShieldKineticResistance = component.ShieldKineticResistance * multiplier;
            stats.ShieldEnergyResistance = component.ShieldEnergyResistance * multiplier;
            stats.ShieldThermalResistance = component.ShieldThermalResistance * multiplier;
            stats.ShieldQuantumResistance = component.ShieldQuantumResistance * multiplier;

            stats.EnergyShieldKineticResistance = component.EnergyShieldKineticResistance * multiplier;
            stats.EnergyShieldEnergyResistance = component.EnergyShieldEnergyResistance * multiplier;
            stats.EnergyShieldThermalResistance = component.EnergyShieldThermalResistance * multiplier;
            stats.EnergyShieldQuantumResistance = component.EnergyShieldQuantumResistance * multiplier;

            stats.EnginePower = component.EnginePower * multiplier;
            stats.TurnRate = component.TurnRate * multiplier;

            stats.Autopilot = component.Autopilot;

            stats.DroneRangeMultiplier = new StatMultiplier(component.DroneRangeModifier * multiplier);
            stats.DroneDamageMultiplier = new StatMultiplier(component.DroneDamageModifier * multiplier);
            stats.DroneDefenseMultiplier = new StatMultiplier(component.DroneDefenseModifier * multiplier);
            stats.DroneSpeedMultiplier = new StatMultiplier(component.DroneSpeedModifier * multiplier);
            stats.DroneReconstructionTimeMultiplier = new StatMultiplier(component.DroneBuildTimeModifier * multiplier);
            stats.DroneReconstructionSpeed = component.DronesBuiltPerSecond * multiplier;

            stats.WeaponFireRateMultiplier = new StatMultiplier(component.WeaponFireRateModifier * multiplier);
            stats.WeaponDamageMultiplier = new StatMultiplier(component.WeaponDamageModifier * multiplier);
            stats.WeaponRangeMultiplier = new StatMultiplier(component.WeaponRangeModifier * multiplier);
            stats.WeaponEnergyCostMultiplier = new StatMultiplier(component.WeaponEnergyCostModifier * multiplier);

            return stats;
        }

        public static ShipEquipmentStats operator +(ShipEquipmentStats first, ShipEquipmentStats second)
        {
            first.ArmorPoints += second.ArmorPoints;
            first.ArmorRepairRate += second.ArmorRepairRate;
            first.ArmorRepairCooldownMultiplier += second.ArmorRepairCooldownMultiplier;
            first.ArmorPointsAttenuatableRate += second.ArmorPointsAttenuatableRate;
            first.ArmorRepairAttenuatableRate += second.ArmorRepairAttenuatableRate;

            first.EnergyPoints += second.EnergyPoints;
            first.EnergyRechargeRate += second.EnergyRechargeRate;
            first.EnergyRechargeCooldownMultiplier += second.EnergyRechargeCooldownMultiplier;
            first.EnergyPointsAttenuatableRate += second.EnergyPointsAttenuatableRate;
            first.EnergyRechargeAttenuatableRate += second.EnergyRechargeAttenuatableRate;

            first.ShieldPoints += second.ShieldPoints;
            first.ShieldRechargeRate += second.ShieldRechargeRate;
            first.ShieldRechargeCooldownMultiplier += second.ShieldRechargeCooldownMultiplier;
            first.ShieldPointsAttenuatableRate += second.ShieldPointsAttenuatableRate;
            first.ShieldRechargeAttenuatableRate += second.ShieldRechargeAttenuatableRate;

            first.EnergyShieldPoints += second.EnergyShieldPoints;
            first.EnergyShieldRechargeRate += second.EnergyShieldRechargeRate;
            first.EnergyShieldRechargeCooldownMultiplier += second.EnergyShieldRechargeCooldownMultiplier;
            first.EnergyShieldPointsAttenuatableRate += second.EnergyShieldPointsAttenuatableRate;
            first.EnergyShieldRechargeAttenuatableRate += second.EnergyShieldRechargeAttenuatableRate;

            first.Weight += second.Weight;

            first.EnergyAbsorption += second.EnergyAbsorption;
            first.RammingDamage += second.RammingDamage;
            first.RammingDamageMultiplier += second.RammingDamageMultiplier;

            first.KineticResistance += second.KineticResistance;
            first.EnergyResistance += second.EnergyResistance;
            first.ThermalResistance += second.ThermalResistance;
            first.QuantumResistance += second.QuantumResistance;

            first.ShieldKineticResistance += second.ShieldKineticResistance;
            first.ShieldEnergyResistance += second.ShieldEnergyResistance;
            first.ShieldThermalResistance += second.ShieldThermalResistance;
            first.ShieldQuantumResistance += second.ShieldQuantumResistance;

            first.EnergyShieldKineticResistance += second.EnergyShieldKineticResistance;
            first.EnergyShieldEnergyResistance += second.EnergyShieldEnergyResistance;
            first.EnergyShieldThermalResistance += second.EnergyShieldThermalResistance;
            first.EnergyShieldQuantumResistance += second.EnergyShieldQuantumResistance;

            first.EnginePower += second.EnginePower;
            first.TurnRate += second.TurnRate;

            first.Autopilot |= second.Autopilot;

            first.DroneRangeMultiplier += second.DroneRangeMultiplier;
            first.DroneDamageMultiplier += second.DroneDamageMultiplier;
            first.DroneDefenseMultiplier += second.DroneDefenseMultiplier;
            first.DroneSpeedMultiplier += second.DroneSpeedMultiplier;
            first.DroneReconstructionSpeed += second.DroneReconstructionSpeed;
            first.DroneReconstructionTimeMultiplier += second.DroneReconstructionTimeMultiplier;

            first.WeaponFireRateMultiplier += second.WeaponFireRateMultiplier;
            first.WeaponDamageMultiplier += second.WeaponDamageMultiplier;
            first.WeaponRangeMultiplier += second.WeaponRangeMultiplier;
            first.WeaponEnergyCostMultiplier += second.WeaponEnergyCostMultiplier;

            return first;
        }
    }
}
