using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Constructor;

namespace ViewModel
{
	public class StatsPanelViewModel : MonoBehaviour
	{
		public TextFieldViewModel ArmorPoints;
		public TextFieldViewModel RepairRate;
		public TextFieldViewModel ArmorRepairCooldown;
		public TextFieldViewModel ArmorPointsAttenuatableRate;
		public TextFieldViewModel ArmorRepairAttenuatableRate;

		public TextFieldViewModel Energy;
		public TextFieldViewModel RechargeRate;
		public TextFieldViewModel EnergyRechargeCooldown;
		public TextFieldViewModel EnergyPointsAttenuatableRate;
		public TextFieldViewModel EnergyRechargeAttenuatableRate;

        public TextFieldViewModel Shield;
        public TextFieldViewModel ShieldRechargeRate;
        public TextFieldViewModel ShieldRechargeCooldown;
        public TextFieldViewModel ShieldPointsAttenuatableRate;
        public TextFieldViewModel ShieldRechargeAttenuatableRate;

        public TextFieldViewModel EnergyShield;
        public TextFieldViewModel EnergyShieldRechargeRate;
        public TextFieldViewModel EnergyShieldRechargeCooldown;
        public TextFieldViewModel EnergyShieldPointsAttenuatableRate;
        public TextFieldViewModel EnergyShieldRechargeAttenuatableRate;

        public TextFieldViewModel Weight;
		public TextFieldViewModel RamDamage;
		public TextFieldViewModel ExpectRamDamage;
		public TextFieldViewModel DamageAbsorption;
		public TextFieldViewModel Velocity;
		public TextFieldViewModel TurnRate;
		public TextFieldViewModel MAXVelocity;
		public TextFieldViewModel MAXTurnRate;
		public TextFieldViewModel WeaponDamage;
		public TextFieldViewModel WeaponFireRate;
		public TextFieldViewModel WeaponRange;
		public TextFieldViewModel WeaponEnergyConsumption;
		public TextFieldViewModel DroneDamageModifier;
	    public TextFieldViewModel DroneDefenseModifier;
        public TextFieldViewModel DroneRangeModifier;
		public TextFieldViewModel DroneSpeedModifier;
		public TextFieldViewModel DroneTimeModifier;

		public TextFieldViewModel EnergyDamageResistance;
		public TextFieldViewModel KineticDamageResistance;
		public TextFieldViewModel HeatDamageResistance;
		public TextFieldViewModel QuantumDamageResistance;

		public TextFieldViewModel ShieldEnergyDamageResistance;
		public TextFieldViewModel ShieldKineticDamageResistance;
		public TextFieldViewModel ShieldHeatDamageResistance;
		public TextFieldViewModel ShieldQuantumDamageResistance;

		public TextFieldViewModel EnergyShieldEnergyDamageResistance;
		public TextFieldViewModel EnergyShieldKineticDamageResistance;
		public TextFieldViewModel EnergyShieldHeatDamageResistance;
		public TextFieldViewModel EnergyShieldQuantumDamageResistance;

		public GameObject WeaponsBlock;
		public GameObject DronesBlock;
		public GameObject ResistanceBlock;

		public Color NormalColor;
		public Color ErrorColor;

		public Text HitPointsSummaryText;
		public Text EnergySummaryText;
		public Text VelocitySummaryText;
		public Text MAXVelocitySummaryText;

		public CanvasGroup CanvasGroup;

		public void OnMoreInfoButtonClicked(bool isOn)
		{
			CanvasGroup.alpha = isOn ? 1 : 0;
		}

		public void UpdateStats(Constructor.IShipSpecification spec)
		{
			HitPointsSummaryText.text = RoundToInt(spec.Stats.ArmorPoints);
			HitPointsSummaryText.color = spec.Stats.ArmorPoints > 0 ? NormalColor : ErrorColor;

			EnergySummaryText.text = RoundToInt(spec.Stats.EnergyPoints) + " [" + RoundToInt(spec.Stats.EnergyRechargeRate, true) + "]";
			EnergySummaryText.color = spec.Stats.EnergyRechargeRate > 0 ? NormalColor : ErrorColor;
#if UNITY_EDITOR
			VelocitySummaryText.text = spec.Stats.EnginePower.ToString("N1") + " / " + (spec.Stats.TurnRate * Mathf.Rad2Deg).ToString("N1") + " ( " + spec.Stats.NOLIMITEnginePower.ToString("N1") + " / " + (spec.Stats.NOLIMITTurnRate * Mathf.Rad2Deg).ToString("N1") + " ) ";
#else
			VelocitySummaryText.text = spec.Stats.EnginePower.ToString("N1") + " / " + (spec.Stats.TurnRate * Mathf.Rad2Deg).ToString("N1");
#endif
			VelocitySummaryText.color = spec.Stats.EnginePower > 0 ? NormalColor : ErrorColor;
#if UNITY_EDITOR
            MAXVelocitySummaryText.text = spec.Stats.EngineMAXPower.ToString("N1") + " / " + (spec.Stats.TurnMAXRate * Mathf.Rad2Deg).ToString("N1") + " ( " + spec.Stats.NOLIMITEngineMAXPower.ToString("N1") + " / " + (spec.Stats.NOLIMITTurnMAXRate * Mathf.Rad2Deg).ToString("N1") + " ) ";
#else
            MAXVelocitySummaryText.text = spec.Stats.EngineMAXPower.ToString("N1") + " / " + (spec.Stats.TurnMAXRate * Mathf.Rad2Deg).ToString("N1");
#endif
            MAXVelocitySummaryText.color = spec.Stats.EngineMAXPower > 0 ? NormalColor : ErrorColor;

			ArmorPoints.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ArmorPoints, 0));
			ArmorPoints.Value.text = spec.Stats.ArmorPoints.ToString("N1");
			ArmorPoints.Color = spec.Stats.ArmorPoints > 0 ? NormalColor : ErrorColor;

			RepairRate.gameObject.SetActive(spec.Stats.ArmorRepairRate != 0);
			RepairRate.Value.text = spec.Stats.ArmorRepairRate.ToString("N1");

			ArmorRepairCooldown.gameObject.SetActive(spec.Stats.ArmorRepairCooldown > 0 && spec.Stats.ArmorRepairRate != 0);
			ArmorRepairCooldown.Value.text = spec.Stats.ArmorRepairCooldown.ToString("N1");
			ArmorPointsAttenuatableRate.gameObject.SetActive(spec.Stats.ArmorPointsAttenuatableRate > 0 && spec.Stats.ArmorPoints != 0);
			ArmorPointsAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.ArmorPointsAttenuatableRate * 100) + "%";
			ArmorRepairAttenuatableRate.gameObject.SetActive(spec.Stats.ArmorRepairAttenuatableRate > 0 && spec.Stats.ArmorRepairRate != 0);
			ArmorRepairAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.ArmorRepairAttenuatableRate * 100) + "%";

			Shield.gameObject.SetActive(spec.Stats.ShieldPoints > 0);
			Shield.Value.text = spec.Stats.ShieldPoints.ToString("N1");
			ShieldRechargeRate.gameObject.SetActive(spec.Stats.ShieldPoints != 0);
			ShieldRechargeRate.Value.text = spec.Stats.ShieldRechargeRate.ToString("N");

			ShieldRechargeCooldown.gameObject.SetActive(spec.Stats.ShieldRechargeCooldown > 0 && spec.Stats.ShieldRechargeRate != 0);
			ShieldRechargeCooldown.Value.text = spec.Stats.ShieldRechargeCooldown.ToString("N1");
			ShieldPointsAttenuatableRate.gameObject.SetActive(spec.Stats.ShieldPointsAttenuatableRate > 0 && spec.Stats.ShieldPoints != 0);
			ShieldPointsAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.ShieldPointsAttenuatableRate * 100) + "%";
			ShieldRechargeAttenuatableRate.gameObject.SetActive(spec.Stats.ShieldRechargeAttenuatableRate > 0 && spec.Stats.ShieldRechargeRate != 0);
			ShieldRechargeAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.ShieldRechargeAttenuatableRate * 100) + "%";

			EnergyShield.gameObject.SetActive(spec.Stats.EnergyShieldPoints > 0);
			EnergyShield.Value.text = spec.Stats.EnergyShieldPoints.ToString("N1");
			EnergyShieldRechargeRate.gameObject.SetActive(spec.Stats.EnergyShieldPoints != 0);
			EnergyShieldRechargeRate.Value.text = spec.Stats.EnergyShieldRechargeRate.ToString("N");

			EnergyShieldRechargeCooldown.gameObject.SetActive(spec.Stats.EnergyShieldRechargeCooldown > 0 && spec.Stats.EnergyShieldRechargeRate != 0);
			EnergyShieldRechargeCooldown.Value.text = spec.Stats.EnergyShieldRechargeCooldown.ToString("N1");
			EnergyShieldPointsAttenuatableRate.gameObject.SetActive(spec.Stats.EnergyShieldPointsAttenuatableRate > 0 && spec.Stats.EnergyShieldPoints != 0);
			EnergyShieldPointsAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.EnergyShieldPointsAttenuatableRate * 100) + "%";
			EnergyShieldRechargeAttenuatableRate.gameObject.SetActive(spec.Stats.EnergyShieldRechargeAttenuatableRate > 0 && spec.Stats.EnergyShieldRechargeRate != 0);
			EnergyShieldRechargeAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.EnergyShieldRechargeAttenuatableRate * 100) + "%";

			Energy.Value.text = spec.Stats.EnergyPoints.ToString("N");
			Weight.Value.text = Mathf.RoundToInt(spec.Stats.Weight/**1000*/).ToString();
			RechargeRate.Value.text = spec.Stats.EnergyRechargeRate.ToString("N");
			RechargeRate.Color = spec.Stats.EnergyRechargeRate > 0 ? NormalColor : ErrorColor;

			EnergyRechargeCooldown.gameObject.SetActive(spec.Stats.EnergyRechargeCooldown > 0 && spec.Stats.EnergyRechargeRate != 0);
			EnergyRechargeCooldown.Value.text = spec.Stats.EnergyRechargeCooldown.ToString("N1");
			EnergyPointsAttenuatableRate.gameObject.SetActive(spec.Stats.EnergyPointsAttenuatableRate > 0 && spec.Stats.EnergyPoints != 0);
			EnergyPointsAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.EnergyPointsAttenuatableRate * 100) + "%";
			EnergyRechargeAttenuatableRate.gameObject.SetActive(spec.Stats.EnergyRechargeAttenuatableRate > 0 && spec.Stats.EnergyRechargeRate != 0);
			EnergyRechargeAttenuatableRate.Value.text = Mathf.RoundToInt(spec.Stats.EnergyRechargeAttenuatableRate * 100) + "%";

			Velocity.Color = spec.Stats.EnginePower > 0 ? NormalColor : ErrorColor;
			Velocity.Value.text = spec.Stats.EnginePower.ToString("N");
			TurnRate.Color = spec.Stats.TurnRate > 0 ? NormalColor : ErrorColor;
			TurnRate.Value.text = (spec.Stats.TurnRate * Mathf.Rad2Deg).ToString("N");

			MAXVelocity.Color = spec.Stats.EngineMAXPower > 0 ? NormalColor : ErrorColor;
			MAXVelocity.Value.text = spec.Stats.EngineMAXPower.ToString("N");
			MAXTurnRate.Color = spec.Stats.TurnMAXRate > 0 ? NormalColor : ErrorColor;
			MAXTurnRate.Value.text = (spec.Stats.TurnMAXRate * Mathf.Rad2Deg).ToString("N");

			WeaponDamage.gameObject.SetActive(spec.Stats.WeaponDamageMultiplier.HasValue);
			WeaponDamage.Value.text = spec.Stats.WeaponDamageMultiplier.ToString();
			WeaponFireRate.gameObject.SetActive(spec.Stats.WeaponFireRateMultiplier.HasValue);
			WeaponFireRate.Value.text = spec.Stats.WeaponFireRateMultiplier.ToString();
			WeaponRange.gameObject.SetActive(spec.Stats.WeaponRangeMultiplier.HasValue);
			WeaponRange.Value.text = spec.Stats.WeaponRangeMultiplier.ToString();
			WeaponEnergyConsumption.gameObject.SetActive(spec.Stats.WeaponEnergyCostMultiplier.HasValue);
			WeaponEnergyConsumption.Value.text = spec.Stats.WeaponEnergyCostMultiplier.ToString();
			WeaponsBlock.gameObject.SetActive(WeaponsBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);

			DroneDamageModifier.gameObject.SetActive(spec.Stats.DroneDamageMultiplier.HasValue);
			DroneDamageModifier.Value.text = spec.Stats.DroneDamageMultiplier.ToString();
			DroneDefenseModifier.gameObject.SetActive(spec.Stats.DroneDefenseMultiplier.HasValue);
			DroneDefenseModifier.Value.text = spec.Stats.DroneDefenseMultiplier.ToString();
			DroneRangeModifier.gameObject.SetActive(spec.Stats.DroneRangeMultiplier.HasValue);
			DroneRangeModifier.Value.text = spec.Stats.DroneRangeMultiplier.ToString();
			DroneSpeedModifier.gameObject.SetActive(spec.Stats.DroneSpeedMultiplier.HasValue);
			DroneSpeedModifier.Value.text = spec.Stats.DroneSpeedMultiplier.ToString();
			DroneTimeModifier.gameObject.SetActive(spec.Stats.DroneBuildSpeed > 0);
			DroneTimeModifier.Value.text = spec.Stats.DroneBuildTime.ToString("N1");
			DronesBlock.gameObject.SetActive(DronesBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);

			RamDamage.gameObject.SetActive(!Mathf.Approximately(spec.Stats.RammingDamage, 0));
			RamDamage.Value.text = FormatPercent(spec.Stats.RammingDamageMultiplier);
			ExpectRamDamage.gameObject.SetActive(spec.Stats.ExpectRammingDamage > 0);
			ExpectRamDamage.Value.text = spec.Stats.ExpectRammingDamage.ToString("N1");
			DamageAbsorption.gameObject.SetActive(spec.Stats.EnergyAbsorption > 0);
			DamageAbsorption.Value.text = Mathf.RoundToInt(spec.Stats.EnergyAbsorptionPercentage * 100) + "%";

			KineticDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.KineticResistance, 0));
			KineticDamageResistance.Value.text = FloatToString(spec.Stats.KineticResistance) + " ( " + Mathf.RoundToInt(spec.Stats.KineticResistancePercentage * 100) + "% )";
			HeatDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ThermalResistance, 0));
			HeatDamageResistance.Value.text = FloatToString(spec.Stats.ThermalResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ThermalResistancePercentage * 100) + "% )";
			EnergyDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyResistance, 0));
			EnergyDamageResistance.Value.text = FloatToString(spec.Stats.EnergyResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyResistancePercentage * 100) + "% )";
			QuantumDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.QuantumResistance, 0));
			QuantumDamageResistance.Value.text = FloatToString(spec.Stats.QuantumResistance) + " ( " + Mathf.RoundToInt(spec.Stats.QuantumResistancePercentage * 100) + "% )";

			ShieldKineticDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ShieldKineticResistance, 0));
			ShieldKineticDamageResistance.Value.text = FloatToString(spec.Stats.ShieldKineticResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ShieldKineticResistancePercentage * 100) + "% )";
			ShieldHeatDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ShieldThermalResistance, 0));
			ShieldHeatDamageResistance.Value.text = FloatToString(spec.Stats.ShieldThermalResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ShieldThermalResistancePercentage * 100) + "% )";
			ShieldEnergyDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ShieldEnergyResistance, 0));
			ShieldEnergyDamageResistance.Value.text = FloatToString(spec.Stats.ShieldEnergyResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ShieldEnergyResistancePercentage * 100) + "% )";
			ShieldQuantumDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.ShieldQuantumResistance, 0));
			ShieldQuantumDamageResistance.Value.text = FloatToString(spec.Stats.ShieldQuantumResistance) + " ( " + Mathf.RoundToInt(spec.Stats.ShieldQuantumResistancePercentage * 100) + "% )";

			EnergyShieldKineticDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyShieldKineticResistance, 0));
			EnergyShieldKineticDamageResistance.Value.text = FloatToString(spec.Stats.EnergyShieldKineticResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyShieldKineticResistancePercentage * 100) + "% )";
			EnergyShieldHeatDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyShieldThermalResistance, 0));
			EnergyShieldHeatDamageResistance.Value.text = FloatToString(spec.Stats.EnergyShieldThermalResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyShieldThermalResistancePercentage * 100) + "% )";
			EnergyShieldEnergyDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyShieldEnergyResistance, 0));
			EnergyShieldEnergyDamageResistance.Value.text = FloatToString(spec.Stats.EnergyShieldEnergyResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyShieldEnergyResistancePercentage * 100) + "% )";
			EnergyShieldQuantumDamageResistance.gameObject.SetActive(!Mathf.Approximately(spec.Stats.EnergyShieldQuantumResistance, 0));
			EnergyShieldQuantumDamageResistance.Value.text = FloatToString(spec.Stats.EnergyShieldQuantumResistance) + " ( " + Mathf.RoundToInt(spec.Stats.EnergyShieldQuantumResistancePercentage * 100) + "% )";

			ResistanceBlock.gameObject.SetActive(ResistanceBlock.transform.Cast<Transform>().Count(item => item.gameObject.activeSelf) > 1);
		}

		private string FormatPercent(float value)
		{
			if (value >= 1)
				return "+" + Mathf.RoundToInt(100*(value - 1)) + "%";
			else
				return "-" + Mathf.RoundToInt(100*(1 - value)) + "%";
		}

		private static string RoundToInt(float value, bool showSign = false)
		{
			var intValue = Mathf.RoundToInt(value);
			if (showSign && intValue >= 0)
				return "+" + intValue;
			else
				return intValue.ToString();
		}

		private static string FloatToString(float value, bool showSign = false)
		{
			if (showSign && value >= 0)
				return "+" + value.ToString("N");
			else
				return value.ToString("N");
		}
    }
}
