//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;

namespace GameDatabase.DataModel
{
	public partial class ComponentStats
	{
		partial void OnDataDeserialized(ComponentStatsSerializable serializable, Database.Loader loader);

		public static ComponentStats Create(ComponentStatsSerializable serializable, Database.Loader loader)
		{
			return new ComponentStats(serializable, loader);
		}

		private ComponentStats(ComponentStatsSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<ComponentStats>(serializable.Id);
			loader.AddComponentStats(serializable.Id, this);

			Type = serializable.Type;
			ArmorPoints = UnityEngine.Mathf.Clamp(serializable.ArmorPoints, -1000000f, 1000000f);
			ArmorRepairRate = UnityEngine.Mathf.Clamp(serializable.ArmorRepairRate, -1000000f, 1000000f);
			ArmorRepairCooldownModifier = UnityEngine.Mathf.Clamp(serializable.ArmorRepairCooldownModifier, -1f, 1f);
			ArmorPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ArmorPointsAttenuatableRate, -1f, 1f);
			ArmorRepairAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ArmorRepairAttenuatableRate, -1f, 1f);
			EnergyPoints = UnityEngine.Mathf.Clamp(serializable.EnergyPoints, -1000000f, 1000000f);
			EnergyRechargeRate = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeRate, -1000000f, 1000000f);
			EnergyRechargeCooldownModifier = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeCooldownModifier, -5f, 5f);
			EnergyPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyPointsAttenuatableRate, -1f, 1f);
			EnergyRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeAttenuatableRate, -1f, 1f);
			ShieldPoints = UnityEngine.Mathf.Clamp(serializable.ShieldPoints, -1000000f, 1000000f);
			ShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeRate, -1000000f, 1000000f);
			ShieldRechargeCooldownModifier = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeCooldownModifier, -5f, 5f);
			ShieldPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ShieldPointsAttenuatableRate, -1f, 1f);
			ShieldRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeAttenuatableRate, -1f, 1f);
			EnergyShieldPoints = UnityEngine.Mathf.Clamp(serializable.EnergyShieldPoints, -1000000f, 1000000f);
			EnergyShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.EnergyShieldRechargeRate, -1000000f, 1000000f);
			EnergyShieldRechargeCooldownModifier = UnityEngine.Mathf.Clamp(serializable.EnergyShieldRechargeCooldownModifier, -5f, 5f);
			EnergyShieldPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyShieldPointsAttenuatableRate, -1f, 1f);
			EnergyShieldRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyShieldRechargeAttenuatableRate, -1f, 1f);
			Weight = UnityEngine.Mathf.Clamp(serializable.Weight, -1000000f, 1000000f);
			RammingDamage = UnityEngine.Mathf.Clamp(serializable.RammingDamage, -1000000f, 1000000f);
			EnergyAbsorption = UnityEngine.Mathf.Clamp(serializable.EnergyAbsorption, -1000000f, 1000000f);
			KineticResistance = UnityEngine.Mathf.Clamp(serializable.KineticResistance, -1000000f, 1000000f);
			EnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyResistance, -1000000f, 1000000f);
			ThermalResistance = UnityEngine.Mathf.Clamp(serializable.ThermalResistance, -1000000f, 1000000f);
			QuantumResistance = UnityEngine.Mathf.Clamp(serializable.QuantumResistance, -1000000f, 1000000f);
			ShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.ShieldKineticResistance, -1000000f, 1000000f);
			ShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.ShieldEnergyResistance, -1000000f, 1000000f);
			ShieldThermalResistance = UnityEngine.Mathf.Clamp(serializable.ShieldThermalResistance, -1000000f, 1000000f);
			ShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.ShieldQuantumResistance, -1000000f, 1000000f);
			EnergyShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldKineticResistance, -1000000f, 1000000f);
			EnergyShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldEnergyResistance, -1000000f, 1000000f);
			EnergyShieldThermalResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldThermalResistance, -1000000f, 1000000f);
			EnergyShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldQuantumResistance, -1000000f, 1000000f);
			EnginePower = UnityEngine.Mathf.Clamp(serializable.EnginePower, 0f, 2000f);
			TurnRate = UnityEngine.Mathf.Clamp(serializable.TurnRate, 0f, 2000f);
			Autopilot = serializable.Autopilot;
			DroneRangeModifier = UnityEngine.Mathf.Clamp(serializable.DroneRangeModifier, -50f, 50f);
			DroneDamageModifier = UnityEngine.Mathf.Clamp(serializable.DroneDamageModifier, -50f, 50f);
			DroneDefenseModifier = UnityEngine.Mathf.Clamp(serializable.DroneDefenseModifier, -50f, 50f);
			DroneSpeedModifier = UnityEngine.Mathf.Clamp(serializable.DroneSpeedModifier, -50f, 50f);
			DronesBuiltPerSecond = UnityEngine.Mathf.Clamp(serializable.DronesBuiltPerSecond, 0f, 100f);
			DroneBuildTimeModifier = UnityEngine.Mathf.Clamp(serializable.DroneBuildTimeModifier, 0f, 100f);
			WeaponFireRateModifier = UnityEngine.Mathf.Clamp(serializable.WeaponFireRateModifier, -100f, 100f);
			WeaponDamageModifier = UnityEngine.Mathf.Clamp(serializable.WeaponDamageModifier, -100f, 100f);
			WeaponRangeModifier = UnityEngine.Mathf.Clamp(serializable.WeaponRangeModifier, -100f, 100f);
			WeaponEnergyCostModifier = UnityEngine.Mathf.Clamp(serializable.WeaponEnergyCostModifier, -100f, 100f);
			AlterWeaponPlatform = serializable.AlterWeaponPlatform;

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<ComponentStats> Id;

		public ComponentStatsType Type { get; private set; }
		public float ArmorPoints { get; private set; }
		public float ArmorRepairRate { get; private set; }
		public float ArmorRepairCooldownModifier { get; private set; }
		public float ArmorPointsAttenuatableRate { get; private set; }
		public float ArmorRepairAttenuatableRate { get; private set; }
		public float EnergyPoints { get; private set; }
		public float EnergyRechargeRate { get; private set; }
		public float EnergyRechargeCooldownModifier { get; private set; }
		public float EnergyPointsAttenuatableRate { get; private set; }
		public float EnergyRechargeAttenuatableRate { get; private set; }
		public float ShieldPoints { get; private set; }
		public float ShieldRechargeRate { get; private set; }
		public float ShieldRechargeCooldownModifier { get; private set; }
		public float ShieldPointsAttenuatableRate { get; private set; }
		public float ShieldRechargeAttenuatableRate { get; private set; }
		public float EnergyShieldPoints { get; private set; }
		public float EnergyShieldRechargeRate { get; private set; }
		public float EnergyShieldRechargeCooldownModifier { get; private set; }
		public float EnergyShieldPointsAttenuatableRate { get; private set; }
		public float EnergyShieldRechargeAttenuatableRate { get; private set; }
		public float Weight { get; private set; }
		public float RammingDamage { get; private set; }
		public float EnergyAbsorption { get; private set; }
		public float KineticResistance { get; private set; }
		public float EnergyResistance { get; private set; }
		public float ThermalResistance { get; private set; }
		public float QuantumResistance { get; private set; }
		public float ShieldKineticResistance { get; private set; }
		public float ShieldEnergyResistance { get; private set; }
		public float ShieldThermalResistance { get; private set; }
		public float ShieldQuantumResistance { get; private set; }
		public float EnergyShieldKineticResistance { get; private set; }
		public float EnergyShieldEnergyResistance { get; private set; }
		public float EnergyShieldThermalResistance { get; private set; }
		public float EnergyShieldQuantumResistance { get; private set; }
		public float EnginePower { get; private set; }
		public float TurnRate { get; private set; }
		public bool Autopilot { get; private set; }
		public float DroneRangeModifier { get; private set; }
		public float DroneDamageModifier { get; private set; }
		public float DroneDefenseModifier { get; private set; }
		public float DroneSpeedModifier { get; private set; }
		public float DronesBuiltPerSecond { get; private set; }
		public float DroneBuildTimeModifier { get; private set; }
		public float WeaponFireRateModifier { get; private set; }
		public float WeaponDamageModifier { get; private set; }
		public float WeaponRangeModifier { get; private set; }
		public float WeaponEnergyCostModifier { get; private set; }
		public PlatformType AlterWeaponPlatform { get; private set; }

		public static ComponentStats DefaultValue { get; private set; }
	}
}
