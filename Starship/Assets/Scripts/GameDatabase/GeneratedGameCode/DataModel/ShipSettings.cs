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
	public partial class ShipSettings
	{
		partial void OnDataDeserialized(ShipSettingsSerializable serializable, Database.Loader loader);

		public static ShipSettings Create(ShipSettingsSerializable serializable, Database.Loader loader)
		{
			return new ShipSettings(serializable, loader);
		}

		private ShipSettings(ShipSettingsSerializable serializable, Database.Loader loader)
		{
			DefaultWeightPerCell = UnityEngine.Mathf.Clamp(serializable.DefaultWeightPerCell, 1f, 1000000f);
			MinimumWeightPerCell = UnityEngine.Mathf.Clamp(serializable.MinimumWeightPerCell, 1f, 1000000f);
			BaseArmorPoints = UnityEngine.Mathf.Clamp(serializable.BaseArmorPoints, 0f, 1000000f);
			ArmorPointsPerCell = UnityEngine.Mathf.Clamp(serializable.ArmorPointsPerCell, 0f, 1000000f);
			ArmorRepairCooldown = UnityEngine.Mathf.Clamp(serializable.ArmorRepairCooldown, 0f, 60f);
			BaseEnergyPoints = UnityEngine.Mathf.Clamp(serializable.BaseEnergyPoints, 0f, 1000000f);
			BaseEnergyRechargeRate = UnityEngine.Mathf.Clamp(serializable.BaseEnergyRechargeRate, 0f, 1000000f);
			EnergyRechargeCooldown = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeCooldown, 0f, 60f);
			BaseShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.BaseShieldRechargeRate, 0f, 1000000f);
			ShieldRechargeCooldown = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeCooldown, 0f, 60f);
			BaseEnergyShieldRechargeRate = UnityEngine.Mathf.Clamp(serializable.BaseEnergyShieldRechargeRate, 0f, 1000000f);
			EnergyShieldRechargeCooldown = UnityEngine.Mathf.Clamp(serializable.EnergyShieldRechargeCooldown, 0f, 60f);
			BaseDroneReconstructionSpeed = UnityEngine.Mathf.Clamp(serializable.BaseDroneReconstructionSpeed, 0f, 100f);
			MaxVelocity = UnityEngine.Mathf.Clamp(serializable.MaxVelocity, 5f, 50f);
			MaxTurnRate = UnityEngine.Mathf.Clamp(serializable.MaxTurnRate, 5f, 50f);

			OnDataDeserialized(serializable, loader);
		}

		public float DefaultWeightPerCell { get; private set; }
		public float MinimumWeightPerCell { get; private set; }
		public float BaseArmorPoints { get; private set; }
		public float ArmorPointsPerCell { get; private set; }
		public float ArmorRepairCooldown { get; private set; }
		public float BaseEnergyPoints { get; private set; }
		public float BaseEnergyRechargeRate { get; private set; }
		public float EnergyRechargeCooldown { get; private set; }
		public float BaseShieldRechargeRate { get; private set; }
		public float ShieldRechargeCooldown { get; private set; }
		public float BaseEnergyShieldRechargeRate { get; private set; }
		public float EnergyShieldRechargeCooldown { get; private set; }
		public float BaseDroneReconstructionSpeed { get; private set; }
		public float MaxVelocity { get; private set; }
		public float MaxTurnRate { get; private set; }

		public static ShipSettings DefaultValue { get; private set; }
	}
}
