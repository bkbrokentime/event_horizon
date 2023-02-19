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
	public partial class Ship
	{
		partial void OnDataDeserialized(ShipSerializable serializable, Database.Loader loader);

		public static Ship Create(ShipSerializable serializable, Database.Loader loader)
		{
			return new Ship(serializable, loader);
		}

		private Ship(ShipSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Ship>(serializable.Id);
			loader.AddShip(serializable.Id, this);

			ShipCategory = serializable.ShipCategory;
			Name = serializable.Name;
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));
			SizeClass = serializable.SizeClass;
			IconImage = new SpriteId(serializable.IconImage, SpriteId.Type.ShipIcon);
			IconScale = UnityEngine.Mathf.Clamp(serializable.IconScale, 0.1f, 100f);
			ModelImage = new SpriteId(serializable.ModelImage, SpriteId.Type.Ship);
			ModelScale = UnityEngine.Mathf.Clamp(serializable.ModelScale, 0.1f, 100f);
			_enginePosition = serializable.EnginePosition;
			EngineColor = new ColorData(serializable.EngineColor);
			_engineSize = UnityEngine.Mathf.Clamp(serializable.EngineSize, 0f, 1f);
			Engines = new ImmutableCollection<Engine>(serializable.Engines?.Select(item => Engine.Create(item, loader)));
			EnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyResistance, -100f, 100f);
			KineticResistance = UnityEngine.Mathf.Clamp(serializable.KineticResistance, -100f, 100f);
			HeatResistance = UnityEngine.Mathf.Clamp(serializable.HeatResistance, -100f, 100f);
			QuantumResistance = UnityEngine.Mathf.Clamp(serializable.QuantumResistance, -100f, 100f);
			ShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.ShieldEnergyResistance, -100f, 100f);
			ShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.ShieldKineticResistance, -100f, 100f);
			ShieldHeatResistance = UnityEngine.Mathf.Clamp(serializable.ShieldHeatResistance, -100f, 100f);
			ShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.ShieldQuantumResistance, -100f, 100f);
			EnergyShieldEnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldEnergyResistance, -100f, 100f);
			EnergyShieldKineticResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldKineticResistance, -100f, 100f);
			EnergyShieldHeatResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldHeatResistance, -100f, 100f);
			EnergyShieldQuantumResistance = UnityEngine.Mathf.Clamp(serializable.EnergyShieldQuantumResistance, -100f, 100f);
			ArmorPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ArmorPointsAttenuatableRate, -1f, 1f);
			ArmorRepairAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ArmorRepairAttenuatableRate, -1f, 1f);
			EnergyPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyPointsAttenuatableRate, -1f, 1f);
			EnergyRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyRechargeAttenuatableRate, -1f, 1f);
			ShieldPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ShieldPointsAttenuatableRate, -1f, 1f);
			ShieldRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.ShieldRechargeAttenuatableRate, -1f, 1f);
			EnergyShieldPointsAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyShieldPointsAttenuatableRate, -1f, 1f);
			EnergyShieldRechargeAttenuatableRate = UnityEngine.Mathf.Clamp(serializable.EnergyShieldRechargeAttenuatableRate, -1f, 1f);
			Regeneration = serializable.Regeneration;
			BaseWeightModifier = UnityEngine.Mathf.Clamp(serializable.BaseWeightModifier, -0.9f, 1000f);
			BuiltinDevices = new ImmutableCollection<Device>(serializable.BuiltinDevices?.Select(item => loader.GetDevice(new ItemId<Device>(item), true)));
			Layout = new Layout(serializable.Layout);
			SecondLayout = new Layout(serializable.SecondLayout);
			UAVLaunchPlatforms = new ImmutableCollection<UAVLaunchPlatform>(serializable.UAVLaunchPlatforms?.Select(item => UAVLaunchPlatform.Create(item, loader)));
			Barrels = new ImmutableCollection<Barrel>(serializable.Barrels?.Select(item => Barrel.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Ship> Id;

		public ShipCategory ShipCategory { get; private set; }
		public string Name { get; private set; }
		public Faction Faction { get; private set; }
		public SizeClass SizeClass { get; private set; }
		public SpriteId IconImage { get; private set; }
		public float IconScale { get; private set; }
		public SpriteId ModelImage { get; private set; }
		public float ModelScale { get; private set; }
		private readonly UnityEngine.Vector2 _enginePosition;
		public ColorData EngineColor { get; private set; }
		private readonly float _engineSize;
		public ImmutableCollection<Engine> Engines { get; private set; }
		public float EnergyResistance { get; private set; }
		public float KineticResistance { get; private set; }
		public float HeatResistance { get; private set; }
		public float QuantumResistance { get; private set; }
		public float ShieldEnergyResistance { get; private set; }
		public float ShieldKineticResistance { get; private set; }
		public float ShieldHeatResistance { get; private set; }
		public float ShieldQuantumResistance { get; private set; }
		public float EnergyShieldEnergyResistance { get; private set; }
		public float EnergyShieldKineticResistance { get; private set; }
		public float EnergyShieldHeatResistance { get; private set; }
		public float EnergyShieldQuantumResistance { get; private set; }
		public float ArmorPointsAttenuatableRate { get; private set; }
		public float ArmorRepairAttenuatableRate { get; private set; }
		public float EnergyPointsAttenuatableRate { get; private set; }
		public float EnergyRechargeAttenuatableRate { get; private set; }
		public float ShieldPointsAttenuatableRate { get; private set; }
		public float ShieldRechargeAttenuatableRate { get; private set; }
		public float EnergyShieldPointsAttenuatableRate { get; private set; }
		public float EnergyShieldRechargeAttenuatableRate { get; private set; }
		public bool Regeneration { get; private set; }
		public float BaseWeightModifier { get; private set; }
		public ImmutableCollection<Device> BuiltinDevices { get; private set; }
		public Layout Layout { get; private set; }
		public Layout SecondLayout { get; private set; }
		public ImmutableCollection<UAVLaunchPlatform> UAVLaunchPlatforms { get; private set; }
		public ImmutableCollection<Barrel> Barrels { get; private set; }

		public static Ship DefaultValue { get; private set; }
	}
}
