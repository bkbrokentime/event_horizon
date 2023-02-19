using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Constructor;
using Constructor.Model;
using Constructor.Modification;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Player;
using Gui.ComponentList;
using Model;
using Services.Localization;
using Services.Reources;
using Zenject;
using GameDatabase;
using Maths;
using UnityEngine.SocialPlatforms.Impl;

namespace ViewModel
{
	public class ComponentViewModel : ComponentListItemBase
	{
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IResourceLocator _resourceLocator;
	    [Inject] private readonly PlayerResources _playerResources;

        [SerializeField] public ConstructorViewModel ConstructorViewModel;
        [SerializeField] public Button Button;
        [SerializeField] public Image Icon;
		[SerializeField] public ComponentGIFIconViewModel ComponentGIFIconViewModel;
        [SerializeField] public Text Name;
        [SerializeField] public Text Modification;
        [SerializeField] public Text Count;
        [SerializeField] public Sprite EmptyIcon;
        [SerializeField] public LayoutGroup DescriptionLayout;
        [SerializeField] public GameObject ComponentDescription;
        [SerializeField] public GameObject ComponentDescriptionLabel;

        public override void Initialize(ComponentInfo data, int count)
		{
		    if (Count != null)
		    {
                Count.gameObject.SetActive(count > 0);
		        Count.text = count.ToString();
		    }

            if (_component == data)
				return;

			_component = data;
            var model = _component.CreateComponent(ConstructorViewModel.ShipSize);

            if (Button)
                Button.interactable = model.IsSuitable(ConstructorViewModel.Ship.Model);

			UpdateDescription(model);
		}

		public void Clear()
		{
			Icon.sprite = EmptyIcon;
			Icon.color = Color.white;
			Name.text = "-";
			_component = new ComponentInfo();
		}

		public void OnClicked()
		{
			ConstructorViewModel.ShowComponent(_component);
		}

        public override ComponentInfo Component { get { return _component; } }

		private void UpdateDescription(Constructor.Component.IComponent component)
		{
		    if (Name != null)
		    {
				Name.text = _component.GetName(_localization, true);
		        Name.color = ColorTable.QualityColor(_component.ItemQuality);
		    }
		    if (Icon != null)
			{
				if(_component.Data.GIFIcon)
				{
					_sprites = _resourceLocator.GetGIFSprite(_component.Data.Icon);
					gif = true;

                    //UnityEngine.Debug.Log("_sprites = " + _sprites.Length);

					if (ComponentGIFIconViewModel != null)
					{
						ComponentGIFIconViewModel.gif = true;
						ComponentGIFIconViewModel.icons = _sprites;
					}
					Icon.sprite = _sprites[0];
                }
                else
					Icon.sprite = _resourceLocator.GetSprite(_component.Data.Icon);
				Icon.color = _component.Data.Color;
			}

		    if (Modification != null)
		    {
		        var modification = component.Modification ?? EmptyModification.Instance;
		        Modification.gameObject.SetActive(!string.IsNullOrEmpty(Modification.text = modification.GetDescription(_localization)));
		        Modification.color = ColorTable.QualityColor(_component.ItemQuality);
		    }

			if (DescriptionLayout)
				DescriptionLayout.InitializeElements<TextFieldViewModel, KeyValuePair<string, string>>(
					GetDescription(component, _localization, ConstructorViewModel == null ? null : ConstructorViewModel.Database), UpdateTextField);

			if (ComponentDescription != null && ComponentDescriptionLabel != null)
			{
				if (component.GetComponent().Description != null)
				{
					ComponentDescription.SetActive(true);
					ComponentDescriptionLabel.GetComponent<Text>().text = component.GetComponent().Description;
				}
				else
					ComponentDescription.SetActive(false);
			}
        }

		private void UpdateTextField(TextFieldViewModel viewModel, KeyValuePair<string, string> data)
		{
			viewModel.Label.text = _localization.GetString(data.Key);
			viewModel.Value.text = data.Value;
		}

		public static IEnumerable<KeyValuePair<string, string>> GetDescription(Constructor.Component.IComponent component, ILocalization localization, IDatabase database = null)
		{
			var stats = new ShipEquipmentStats();
			component.UpdateStats(ref stats);

			if (stats.ArmorPoints != 0)
				yield return new KeyValuePair<string, string>("$HitPoints", FormatFloat(stats.ArmorPoints));
			if (stats.ArmorRepairRate != 0)
				yield return new KeyValuePair<string, string>("$RepairRate", FormatFloat(stats.ArmorRepairRate));
			if (stats.ArmorRepairCooldownMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$ArmorRepairCooldown", (stats.ArmorRepairCooldownMultiplier * 100).ToString() + " %");

            if (stats.ArmorPointsAttenuatableRate != 0)
				yield return new KeyValuePair<string, string>("$ArmorPointsAttenuatableRate", FormatFloat(stats.ArmorPointsAttenuatableRate));
			if (stats.ArmorRepairAttenuatableRate != 0)
				yield return new KeyValuePair<string, string>("$ArmorRepairAttenuatableRate", FormatFloat(stats.ArmorRepairAttenuatableRate));

            if (!Mathf.Approximately(stats.EnergyPoints, 0))
				yield return new KeyValuePair<string, string>("$Energy", FormatFloat(stats.EnergyPoints));

			if (stats.EnergyRechargeRate < 0)
				yield return new KeyValuePair<string, string>("$EnergyConsumption", FormatFloat(-stats.EnergyRechargeRate));
			if (stats.EnergyRechargeRate > 0)
				yield return new KeyValuePair<string, string>("$RechargeRate", FormatFloat(stats.EnergyRechargeRate));
            if (stats.EnergyRechargeCooldownMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$EnergyRechargeCooldown", (stats.EnergyRechargeCooldownMultiplier * 100).ToString() + " %");

            if (stats.EnergyPointsAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$EnergyPointsAttenuatableRate", FormatFloat(stats.EnergyPointsAttenuatableRate));
            if (stats.EnergyRechargeAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$EnergyRechargeAttenuatableRate", FormatFloat(stats.EnergyRechargeAttenuatableRate));
            
			if (!Mathf.Approximately(stats.ShieldPoints, 0))
		        yield return new KeyValuePair<string, string>("$ShieldPoints", FormatFloat(stats.ShieldPoints));
            if (!Mathf.Approximately(stats.ShieldRechargeRate, 0))
		        yield return new KeyValuePair<string, string>("$ShieldRechargeRate", FormatFloat(stats.ShieldRechargeRate));
            if (stats.ShieldRechargeCooldownMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$ShieldRechargeCooldown", (stats.ShieldRechargeCooldownMultiplier * 100).ToString() + " %");

            if (stats.ShieldPointsAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$ShieldPointsAttenuatableRate", FormatFloat(stats.ShieldPointsAttenuatableRate));
            if (stats.ShieldRechargeAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$ShieldRechargeAttenuatableRate", FormatFloat(stats.ShieldRechargeAttenuatableRate));

			if (!Mathf.Approximately(stats.EnergyShieldPoints, 0))
		        yield return new KeyValuePair<string, string>("$EnergyShieldPoints", FormatFloat(stats.EnergyShieldPoints));
            if (!Mathf.Approximately(stats.EnergyShieldRechargeRate, 0))
		        yield return new KeyValuePair<string, string>("$EnergyShieldRechargeRate", FormatFloat(stats.EnergyShieldRechargeRate));
            if (stats.EnergyShieldRechargeCooldownMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$EnergyShieldRechargeCooldown", (stats.EnergyShieldRechargeCooldownMultiplier * 100).ToString() + " %");

            if (stats.EnergyShieldPointsAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$EnergyShieldPointsAttenuatableRate", FormatFloat(stats.EnergyShieldPointsAttenuatableRate));
            if (stats.EnergyShieldRechargeAttenuatableRate != 0)
                yield return new KeyValuePair<string, string>("$EnergyShieldRechargeAttenuatableRate", FormatFloat(stats.EnergyShieldRechargeAttenuatableRate));

            if (stats.EnginePower != 0)
				yield return new KeyValuePair<string, string>("$Velocity", FormatFloat(stats.EnginePower));
			if (stats.TurnRate != 0)
				yield return new KeyValuePair<string, string>("$TurnRate", FormatFloat(stats.TurnRate));

			if (stats.WeaponDamageMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DamageModifier", stats.WeaponDamageMultiplier.ToString());
			if (stats.WeaponFireRateMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$FireRateModifier", stats.WeaponFireRateMultiplier.ToString());
			if (stats.WeaponRangeMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$RangeModifier", stats.WeaponRangeMultiplier.ToString());
			if (stats.WeaponEnergyCostMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$EnergyModifier", stats.WeaponEnergyCostMultiplier.ToString());

			if (stats.RammingDamage != 0)
				yield return new KeyValuePair<string, string>("$RamDamage", FormatFloat(stats.RammingDamage));
			if (stats.EnergyAbsorption != 0)
				yield return new KeyValuePair<string, string>("$DamageAbsorption", FormatFloat(stats.EnergyAbsorption));

			if (stats.KineticResistance != 0)
				yield return new KeyValuePair<string, string>("$KineticDamageResistance", FormatFloat(stats.KineticResistance));
			if (stats.ThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$ThermalDamageResistance", FormatFloat(stats.ThermalResistance));
			if (stats.EnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyDamageResistance", FormatFloat(stats.EnergyResistance));
			if (stats.QuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$QuantumDamageResistance", FormatFloat(stats.QuantumResistance));

			if (stats.ShieldKineticResistance != 0)
				yield return new KeyValuePair<string, string>("$ShieldKineticDamageResistance", FormatFloat(stats.ShieldKineticResistance));
			if (stats.ShieldThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$ShieldThermalDamageResistance", FormatFloat(stats.ShieldThermalResistance));
			if (stats.ShieldEnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$ShieldEnergyDamageResistance", FormatFloat(stats.ShieldEnergyResistance));
			if (stats.ShieldQuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$ShieldQuantumDamageResistance", FormatFloat(stats.ShieldQuantumResistance));

			if (stats.EnergyShieldKineticResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyShieldKineticDamageResistance", FormatFloat(stats.EnergyShieldKineticResistance));
			if (stats.EnergyShieldThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyShieldThermalDamageResistance", FormatFloat(stats.EnergyShieldThermalResistance));
			if (stats.EnergyShieldEnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyShieldEnergyDamageResistance", FormatFloat(stats.EnergyShieldEnergyResistance));
			if (stats.EnergyShieldQuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$EnergyShieldQuantumDamageResistance", FormatFloat(stats.EnergyShieldQuantumResistance));

			if (stats.DroneDamageMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneDamageModifier", stats.DroneDamageMultiplier.ToString());
            if (stats.DroneDefenseMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$DroneDefenseModifier", stats.DroneDefenseMultiplier.ToString());
            if (stats.DroneRangeMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneRangeModifier", stats.DroneRangeMultiplier.ToString());
			if (stats.DroneSpeedMultiplier.HasValue)
				yield return new KeyValuePair<string, string>("$DroneSpeedModifier", stats.DroneSpeedMultiplier.ToString());
			if (stats.DroneReconstructionSpeed > 0)
				yield return new KeyValuePair<string, string>("$DroneReconstructionTime", (1f/stats.DroneReconstructionSpeed).ToString("N1"));
            if (stats.DroneReconstructionTimeMultiplier.HasValue)
                yield return new KeyValuePair<string, string>("$DroneReconstructionTime", stats.DroneReconstructionTimeMultiplier.ToString());

            // TODO: display component type
            //DeviceInfo.SetActive(component.Devices.Any());
            //DroneBayInfo.SetActive(component.DroneBays.Any());

            if (component.Weapons.Any())
                foreach (var item in GetWeaponDescription(component.Weapons.First(), localization))
                    yield return item;

            if (component.WeaponsObsolete.Any())
				foreach (var item in GetWeaponDescription(component.WeaponsObsolete.First(), localization))
					yield return item;

            if (component.DroneBays.Any())
                foreach (var item in GetDroneBayDescription(component.DroneBays.First(), localization, database))
                    yield return item;

			if(component.Devices.Any())
                foreach (var item in GetDeviceDescription(component.Devices.First(), localization))
                    yield return item;

            yield return new KeyValuePair<string, string>(division, division2);
            if (!Mathf.Approximately(stats.Weight, 0))
				yield return new KeyValuePair<string, string>("$Weight", Mathf.RoundToInt(stats.Weight).ToString());
		}

        public override bool Selected { get; set; }

		private static IEnumerable<KeyValuePair<string, string>> GetWeaponDescription(KeyValuePair<WeaponStats,AmmunitionObsoleteStats> weapon, ILocalization localization)
		{
            yield return new KeyValuePair<string, string>(division, division2);
            yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));
			//yield return new KeyValuePair<string, string>("$DamageTypeDescription", localization.GetString(weapon.Value.DamageType.Description()));

			var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
			if (weapon.Key.FireRate > 0)
			{
				yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
				yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
				yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f/weapon.Key.FireRate).ToString(_floatFormat));
			}
			else
			{
				yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
				yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
			}

			if (weapon.Value.Range > 0)
				yield return new KeyValuePair<string, string>("$WeaponRange", weapon.Value.Range.ToString(_floatFormat));
			if (weapon.Value.Velocity > 0)
				yield return new KeyValuePair<string, string>("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
			if (weapon.Value.Impulse > 0)
				yield return new KeyValuePair<string, string>("$WeaponImpulse", (weapon.Value.Impulse*1000).ToString(_floatFormat));
			if (weapon.Value.AreaOfEffect > 0)
				yield return new KeyValuePair<string, string>("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));

			yield return new KeyValuePair<string, string>("$WeaponDPS2", Maths.Power.WeaponDPS(weapon.Key, weapon.Value, false).ToString(_floatFormat));
            yield return new KeyValuePair<string, string>("$WeaponMDPS", Maths.Power.WeaponDPS(weapon.Key, weapon.Value, true).ToString(_floatFormat));
            yield return new KeyValuePair<string, string>("$WeaponEPS2", Maths.Power.WeaponEPS(weapon.Key, weapon.Value, false).ToString(_floatFormat));
            yield return new KeyValuePair<string, string>("$WeaponMEPS", Maths.Power.WeaponEPS(weapon.Key, weapon.Value, true).ToString(_floatFormat));
            yield return new KeyValuePair<string, string>("$WeaponScore", Maths.Power.WeaponScore(weapon.Key, weapon.Value).ToString(_floatFormat));

        }

        private static IEnumerable<KeyValuePair<string, string>> GetWeaponDescription(Constructor.Component.WeaponData data, ILocalization localization)
	    {
            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (data.Weapon.Stats.WeaponClass == )
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}
            yield return new KeyValuePair<string, string>(division, division2);

            if (data.Weapon.Stats.WeaponClass == WeaponClass.Continuous)
	        {
	            yield return new KeyValuePair<string, string>("$WeaponEPS", data.Ammunition.Body.EnergyCost.ToString(_floatFormat));
	        }
	        else
	        {
				//var damageSuffix = data.Weapon.Stats.Magazine <= 1 ? string.Empty : "х" + data.Weapon.Stats.Magazine;
                //yield return new KeyValuePair<string, string>("$WeaponDamage", data.Ammunition.Effects.ToString(_floatFormat) + damageSuffix);

                yield return new KeyValuePair<string, string>("$WeaponEnergy", data.Ammunition.Body.EnergyCost.ToString(_floatFormat));
	            yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / (data.Weapon.Stats.FireRate * data.StatModifier.FireRateMultiplier.Value)).ToString(_floatFormat));
            }


	        foreach (var item in GetWeaponDamageText(data.Ammunition, data.StatModifier, localization))
	            yield return item;

			yield return new KeyValuePair<string, string>("$WeaponDPS2", Maths.Power.WeaponDPS(data, false).ToString(_floatFormat));
			yield return new KeyValuePair<string, string>("$WeaponMDPS", Maths.Power.WeaponDPS(data, true).ToString(_floatFormat));
			yield return new KeyValuePair<string, string>("$WeaponEPS2", Maths.Power.WeaponEPS(data, false).ToString(_floatFormat));
			yield return new KeyValuePair<string, string>("$WeaponMEPS", Maths.Power.WeaponEPS(data, true).ToString(_floatFormat));
			yield return new KeyValuePair<string, string>("$WeaponScore", Maths.Power.WeaponScore(data).ToString(_floatFormat));

            //yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(weapon.Value.DamageType.Name()));

            //var damageSuffix = weapon.Key.Magazine <= 1 ? string.Empty : "х" + weapon.Key.Magazine;
            //if (weapon.Key.FireRate > 0)
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDamage", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEnergy", weapon.Value.EnergyCost.ToString(_floatFormat));
            //    yield return new KeyValuePair<string, string>("$WeaponCooldown", (1.0f / weapon.Key.FireRate).ToString(_floatFormat));
            //}
            //else
            //{
            //    yield return new KeyValuePair<string, string>("$WeaponDPS", weapon.Value.Damage.ToString(_floatFormat) + damageSuffix);
            //    yield return new KeyValuePair<string, string>("$WeaponEPS", weapon.Value.EnergyCost.ToString(_floatFormat));
            //}

	        //if (data.Ammunition.Body.Range > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponRange", data.Ammunition.Body.Range.ToString(_floatFormat));
	        //if (data.Ammunition.Body.Velocity > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponRange", data.Ammunition.Body.Velocity.ToString(_floatFormat));
	        //if (weapon.Value.Velocity > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponVelocity", weapon.Value.Velocity.ToString(_floatFormat));
	        //if (weapon.Value.Impulse > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponImpulse", (weapon.Value.Impulse * 1000).ToString(_floatFormat));
	        //if (weapon.Value.AreaOfEffect > 0)
	        //    yield return new KeyValuePair<string, string>("$WeaponArea", weapon.Value.AreaOfEffect.ToString(_floatFormat));
	    }

	    private static IEnumerable<KeyValuePair<string, string>> GetWeaponDamageText(Ammunition ammunition, WeaponStatModifier statModifier, ILocalization localization)
	    {
            var effect = ammunition.Effects.FirstOrDefault(item => item.Type == ImpactEffectType.Damage || item.Type == ImpactEffectType.SiphonHitPoints);
            if (effect.Power > 0)
	        {
	            yield return new KeyValuePair<string, string>("$DamageType", localization.GetString(effect.DamageType.Name()));
	            var damage = effect.Power * statModifier.DamageMultiplier.Value;
                yield return new KeyValuePair<string, string>(ammunition.ImpactType == BulletImpactType.DamageOverTime ? "$WeaponDPS" : "$WeaponDamage",  damage.ToString(_floatFormat));
	            yield break;
	        }

	        var trigger = ammunition.Triggers.OfType<BulletTrigger_SpawnBullet>().FirstOrDefault();
            if (trigger?.Ammunition != null)
                foreach (var item in GetWeaponDamageText(trigger.Ammunition, statModifier, localization))
                    yield return item;
	    }

        private static IEnumerable<KeyValuePair<string, string>> GetDroneBayDescription(KeyValuePair<DroneBayStats,ShipBuild> droneBay, ILocalization localization,IDatabase database=null)
        {
            yield return new KeyValuePair<string, string>(division, division2);
            yield return new KeyValuePair<string, string>("$DroneBayCapacity", droneBay.Key.Capacity.ToString());
			if (droneBay.Key.Squadron > 0)
				yield return new KeyValuePair<string, string>("$DroneBaySquadron", droneBay.Key.Squadron.ToString());
            if (!Mathf.Approximately(droneBay.Key.DamageMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneDamageModifier", FormatPercent(droneBay.Key.DamageMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.DefenseMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneDefenseModifier", FormatPercent(droneBay.Key.DefenseMultiplier - 1f));
            if (!Mathf.Approximately(droneBay.Key.SpeedMultiplier, 1f))
                yield return new KeyValuePair<string, string>("$DroneSpeedModifier", FormatPercent(droneBay.Key.SpeedMultiplier - 1f));

            yield return new KeyValuePair<string, string>("$DroneRangeModifier", droneBay.Key.Range.ToString("N"));
            /*
                        var weapon = droneBay.Value.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase).FirstOrDefault(item => item.Info.Data.Weapon != null);
                        if (weapon != null)
                            yield return new KeyValuePair<string, string>("$WeaponType", localization.GetString(weapon.Info.Data.Name));
             */
            var weapon = droneBay.Value.Components.Select<InstalledComponent, IntegratedComponent>(ComponentExtensions.FromDatabase).Where(item => item.Info.Data.Weapon != null);
			if (weapon != null)
				foreach (var item in weapon)
					yield return new KeyValuePair<string, string>("$WeaponType", localization.GetString(item.Info.Data.Name));
			if (database != null)
				yield return new KeyValuePair<string, string>("$DroneBaysScore", Power.DroneBayScore(droneBay.Key, droneBay.Value, database.ShipSettings).ToString("N3"));
        }
		private static IEnumerable<KeyValuePair<string, string>> GetDeviceDescription(DeviceStats device, ILocalization localization)
		{
			yield return new KeyValuePair<string, string>(division, division2);
            var DeviceClass = device.DeviceClass;
			var EnergyConsumption = device.EnergyConsumption;
			//var PassiveEnergyConsumption = device.PassiveEnergyConsumption * 0.05f;
			var Power = device.Power;
			var Range = device.Range;
			var Size = device.Size;
			var Cooldown = device.Cooldown;
			var Lifetime = device.Lifetime;
			var Quantity = device.Quantity;
			var EquipmentStats = device.EquipmentStats;
			yield return new KeyValuePair<string, string>("$DeviceClass", localization.GetString(DeviceClass.Name()));
			yield return new KeyValuePair<string, string>("$DeviceEnergyConsumption", EnergyConsumption.ToString());
			yield return new KeyValuePair<string, string>("$DeviceCooldown", Cooldown.ToString());
			switch (DeviceClass)
			{
				case DeviceClass.ClonningCenter:
					break;
				case DeviceClass.TimeMachine:
					yield return new KeyValuePair<string, string>("$DeviceLifetime", Lifetime.ToString());
					break;
				case DeviceClass.Accelerator:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					break;
				case DeviceClass.Decoy:
					if (Quantity > 0)
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Quantity.ToString());
					else
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceRange", Range.ToString());
					yield return new KeyValuePair<string, string>("$DeviceLifetime", Lifetime.ToString());
					break;
				case DeviceClass.Ghost:
					yield return new KeyValuePair<string, string>("$DeviceLifetime", Lifetime.ToString());
					break;
				case DeviceClass.PointDefense:
					if (Quantity > 0)
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Quantity.ToString());
					else
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceSize", Size.ToString());
					break;
				case DeviceClass.GravityGenerator:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceRange", Range.ToString());
					break;
				case DeviceClass.EnergyShield:
				case DeviceClass.PartialShield:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceSize", Size.ToString());
					break;
				case DeviceClass.Denseshield:
					yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldKineticResistance", FormatPercent(0.3f));
					yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldEnergyResistance", FormatPercent(0.3f));
					yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldThermalResistance", FormatPercent(0.3f));
					yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldQuantumResistance", FormatPercent(0.3f));
					break;
				case DeviceClass.Fortification:
					yield return new KeyValuePair<string, string>("$EquipmentKineticResistance", FormatPercent(0.8f));
					break;
				case DeviceClass.CombustionInhibition:
					yield return new KeyValuePair<string, string>("$EquipmentThermalResistance", FormatPercent(0.8f));
					break;
				case DeviceClass.EnergyDiversion:
					yield return new KeyValuePair<string, string>("$EquipmentEnergyResistance", FormatPercent(0.8f));
					break;
				case DeviceClass.RepairBot:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					if (Quantity > 0)
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Quantity.ToString());
					break;
				case DeviceClass.Detonator:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceRange", Range.ToString());
					break;
				case DeviceClass.Stealth:
				case DeviceClass.SuperStealth:
					break;
				case DeviceClass.Teleporter:
					yield return new KeyValuePair<string, string>("$DeviceRange", Range.ToString());
					break;
				case DeviceClass.Brake:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					break;
				case DeviceClass.FireAssault:
					yield return new KeyValuePair<string, string>("$EquipmentWeaponDamageMultiplier", FormatPercent(1f));
					yield return new KeyValuePair<string, string>("$EquipmentWeaponFireRateMultiplier", FormatPercent(1f));
					break;
				case DeviceClass.ToxicWaste:
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					yield return new KeyValuePair<string, string>("$DeviceSize", Size.ToString());
					yield return new KeyValuePair<string, string>("$DeviceLifetime", Lifetime.ToString());
					break;
				case DeviceClass.WormTail:
					if (Quantity > 0)
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Quantity.ToString());
					else
						yield return new KeyValuePair<string, string>("$DeviceQuantity", Size.ToString());
					yield return new KeyValuePair<string, string>("$DevicePower", Power.ToString());
					break;
				case DeviceClass.Equipment:
					foreach (var item in GetEquipmentDescription(EquipmentStats, localization))
						yield return item;
					break;
				default:
					break;
			}
		}
        private static IEnumerable<KeyValuePair<string, string>> GetEquipmentDescription(EquipmentStats equipment, ILocalization localization)
		{
			if (equipment.KineticResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentKineticResistance", FormatPercent(equipment.KineticResistance));
			if (equipment.ThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentThermalResistance", FormatPercent(equipment.ThermalResistance));
			if (equipment.EnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentEnergyResistance", FormatPercent(equipment.EnergyResistance));
			if (equipment.QuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentQuantumResistance", FormatPercent(equipment.QuantumResistance));
			if (equipment.ShieldKineticResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentShieldKineticResistance", FormatPercent(equipment.ShieldKineticResistance));
			if (equipment.ShieldThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentShieldThermalResistance", FormatPercent(equipment.ShieldThermalResistance));
			if (equipment.ShieldEnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentShieldEnergyResistance", FormatPercent(equipment.ShieldEnergyResistance));
			if (equipment.ShieldQuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentShieldQuantumResistance", FormatPercent(equipment.ShieldQuantumResistance));
			if (equipment.EnergyShieldKineticResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldKineticResistance", FormatPercent(equipment.EnergyShieldKineticResistance));
			if (equipment.EnergyShieldThermalResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldThermalResistance", FormatPercent(equipment.EnergyShieldThermalResistance));
			if (equipment.EnergyShieldEnergyResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldEnergyResistance", FormatPercent(equipment.EnergyShieldEnergyResistance));
			if (equipment.EnergyShieldQuantumResistance != 0)
				yield return new KeyValuePair<string, string>("$EquipmentEnergyShieldQuantumResistance", FormatPercent(equipment.EnergyShieldQuantumResistance));

			if (equipment.WeaponAoeRadiusMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponAoeRadiusMultiplier", FormatPercent(equipment.WeaponAoeRadiusMultiplier));
			if (equipment.WeaponDamageMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponDamageMultiplier", FormatPercent(equipment.WeaponDamageMultiplier));
			if (equipment.WeaponEnergyCostMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponEnergyCostMultiplier", FormatPercent(equipment.WeaponEnergyCostMultiplier));
			if (equipment.WeaponFireRateMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponFireRateMultiplier", FormatPercent(equipment.WeaponFireRateMultiplier));
			if (equipment.WeaponLifetimeMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponLifetimeMultiplier", FormatPercent(equipment.WeaponLifetimeMultiplier));
			if (equipment.WeaponRangeMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponRangeMultiplier", FormatPercent(equipment.WeaponRangeMultiplier));
			if (equipment.WeaponSizeMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponSizeMultiplier", FormatPercent(equipment.WeaponSizeMultiplier));
			if (equipment.WeaponVelocityMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponVelocityMultiplier", FormatPercent(equipment.WeaponVelocityMultiplier));
			if (equipment.WeaponWeightMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponWeightMultiplier", FormatPercent(equipment.WeaponWeightMultiplier));
			if (equipment.WeaponMagazineMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponMagazineMultiplier", FormatPercent(equipment.WeaponMagazineMultiplier));
			if (equipment.WeaponSpreadMultiplier != 0)
				yield return new KeyValuePair<string, string>("$EquipmentWeaponSpreadMultiplier", FormatPercent(equipment.WeaponSpreadMultiplier));
        }

        private static string FormatInt(int value)
		{
			return (value >= 0 ? "+" : "") + value;
		}

		private static string FormatFloat(float value)
		{
			return (value >= 0 ? "+" : "") + value.ToString(_floatFormat);
		}

		private static string FormatPercent(float value)
		{
			return (value >= 0 ? "+" : "") + Mathf.RoundToInt(100*value) + "%";
		}

		private ComponentInfo _component;
	    private const string _floatFormat = "0.##";

        [SerializeField] private Sprite[] _sprites = new Sprite[0];
        [SerializeField] private bool gif = false;
        private float _time = 0.2f;
        private float _lasttime = 0.2f;
        private int _count = 0;

		private const string division = "<-=-=-=-=-=->";
		private const string division2 = " ";
    }
}
