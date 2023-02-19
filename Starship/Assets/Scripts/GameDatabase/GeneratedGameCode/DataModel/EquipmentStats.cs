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
	public partial class EquipmentStats
	{
		partial void OnDataDeserialized(EquipmentStatsSerializable serializable, Database.Loader loader);

		public static EquipmentStats Create(EquipmentStatsSerializable serializable, Database.Loader loader)
		{
			return new EquipmentStats(serializable, loader);
		}

		private EquipmentStats(EquipmentStatsSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<EquipmentStats>(serializable.Id);
			loader.AddEquipmentStats(serializable.Id, this);

			KineticResistance = UnityEngine.Mathf.Clamp(serializable.KineticResistance, -1f, 1f);
			EnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyResistance, -1f, 1f);
			ThermalResistance = UnityEngine.Mathf.Clamp(serializable.ThermalResistance, -1f, 1f);
			QuantumResistance = UnityEngine.Mathf.Clamp(serializable.QuantumResistance, -1f, 1f);
			ShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.ShieldKineticResistance, -1f, 1f);
			ShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.ShieldEnergyResistance, -1f, 1f);
			ShieldThermalResistance = UnityEngine.Mathf.Clamp(serializable.ShieldThermalResistance, -1f, 1f);
			ShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.ShieldQuantumResistance, -1f, 1f);
			EnergyShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldKineticResistance, -1f, 1f);
			EnergyShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldEnergyResistance, -1f, 1f);
			EnergyShieldThermalResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldThermalResistance, -1f, 1f);
			EnergyShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldQuantumResistance, -1f, 1f);
			WeaponDamageMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponDamageMultiplier, -1f, 100f);
			WeaponFireRateMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponFireRateMultiplier, -1f, 100f);
			WeaponRangeMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponRangeMultiplier, -1f, 100f);
			WeaponEnergyCostMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponEnergyCostMultiplier, -1f, 100f);
			WeaponSizeMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponSizeMultiplier, -1f, 100f);
			WeaponVelocityMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponVelocityMultiplier, -1f, 100f);
			WeaponWeightMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponWeightMultiplier, -1f, 100f);
			WeaponLifetimeMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponLifetimeMultiplier, -1f, 100f);
			WeaponAoeRadiusMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponAoeRadiusMultiplier, -1f, 100f);
			WeaponSpreadMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponSpreadMultiplier, -1f, 100f);
			WeaponMagazineMultiplier = UnityEngine.Mathf.Clamp(serializable.WeaponMagazineMultiplier, -1f, 100f);

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<EquipmentStats> Id;

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
		public float WeaponDamageMultiplier { get; private set; }
		public float WeaponFireRateMultiplier { get; private set; }
		public float WeaponRangeMultiplier { get; private set; }
		public float WeaponEnergyCostMultiplier { get; private set; }
		public float WeaponSizeMultiplier { get; private set; }
		public float WeaponVelocityMultiplier { get; private set; }
		public float WeaponWeightMultiplier { get; private set; }
		public float WeaponLifetimeMultiplier { get; private set; }
		public float WeaponAoeRadiusMultiplier { get; private set; }
		public float WeaponSpreadMultiplier { get; private set; }
		public float WeaponMagazineMultiplier { get; private set; }

		public static EquipmentStats DefaultValue { get; private set; }
	}
}
