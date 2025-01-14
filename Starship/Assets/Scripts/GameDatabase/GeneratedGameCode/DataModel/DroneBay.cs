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
	public partial class DroneBay
	{
		public static DroneBay Create(DroneBaySerializable serializable, Database.Loader loader)
		{
			return new DroneBay(serializable, loader);
		}

		private DroneBay(DroneBaySerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<DroneBay>(serializable.Id);
			loader.AddDroneBay(serializable.Id, this);
			Stats = new DroneBayStats(serializable, loader);
		}

		public readonly ItemId<DroneBay> Id;
		public readonly DroneBayStats Stats;

		public static DroneBay DefaultValue { get; private set; }
	}

	public partial struct DroneBayStats 
	{
		partial void OnDataDeserialized(DroneBaySerializable serializable, Database.Loader loader);

		public DroneBayStats(DroneBaySerializable serializable, Database.Loader loader)
		{
			EnergyConsumption = UnityEngine.Mathf.Clamp(serializable.EnergyConsumption, 0f, 1E+09f);
			PassiveEnergyConsumption = UnityEngine.Mathf.Clamp(serializable.PassiveEnergyConsumption, 0f, 1E+09f);
			Range = UnityEngine.Mathf.Clamp(serializable.Range, 1f, 1000f);
			DamageMultiplier = UnityEngine.Mathf.Clamp(serializable.DamageMultiplier, 0.0001f, 1000f);
			DefenseMultiplier = UnityEngine.Mathf.Clamp(serializable.DefenseMultiplier, 0.0001f, 1000f);
			SpeedMultiplier = UnityEngine.Mathf.Clamp(serializable.SpeedMultiplier, 0.0001f, 1000f);
			ImprovedAi = serializable.ImprovedAi;
			Capacity = UnityEngine.Mathf.Clamp(serializable.Capacity, 1, 1000);
			Squadron = UnityEngine.Mathf.Clamp(serializable.Squadron, 0, 1000);
			ActivationType = serializable.ActivationType;
			LaunchSound = new AudioClipId(serializable.LaunchSound);
			LaunchEffectPrefab = new PrefabId(serializable.LaunchEffectPrefab, PrefabId.Type.Effect);
			ControlButtonIcon = new SpriteId(serializable.ControlButtonIcon, SpriteId.Type.ActionButton);

			OnDataDeserialized(serializable, loader);
		}

		public float EnergyConsumption;
		public float PassiveEnergyConsumption;
		public float Range;
		public float DamageMultiplier;
		public float DefenseMultiplier;
		public float SpeedMultiplier;
		public bool ImprovedAi;
		public int Capacity;
		public int Squadron;
		public ActivationType ActivationType;
		public AudioClipId LaunchSound;
		public PrefabId LaunchEffectPrefab;
		public SpriteId ControlButtonIcon;
	}
}
