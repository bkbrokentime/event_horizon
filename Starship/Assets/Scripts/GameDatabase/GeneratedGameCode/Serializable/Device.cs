//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace GameDatabase.Serializable
{
	[Serializable]
	public class DeviceSerializable : SerializableItem
	{
		public DeviceClass DeviceClass;
		public float EnergyConsumption;
		public float PassiveEnergyConsumption;
		public float Power;
		public int Quantity;
		public float Range;
		public float Size;
		public float Cooldown;
		public float Lifetime;
		public UnityEngine.Vector2 Offset;
		public ActivationType ActivationType;
		public string Color;
		public string Sound;
		public string EffectPrefab;
		public string ObjectPrefab;
		public bool UseMyIcon;
		public string ObjectIconImage;
		public UnityEngine.Vector2 ObjectOffset;
		public string SecondObjectIconImage;
		public UnityEngine.Vector2 SecondObjectOffset;
		public string ControlButtonIcon;
		public int EquipmentStatsId;
	}
}
