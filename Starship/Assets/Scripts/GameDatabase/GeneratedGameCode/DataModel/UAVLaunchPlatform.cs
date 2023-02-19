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
	public partial class UAVLaunchPlatform
	{
		partial void OnDataDeserialized(UAVLaunchPlatformSerializable serializable, Database.Loader loader);

		public static UAVLaunchPlatform Create(UAVLaunchPlatformSerializable serializable, Database.Loader loader)
		{
			return new UAVLaunchPlatform(serializable, loader);
		}

		private UAVLaunchPlatform(UAVLaunchPlatformSerializable serializable, Database.Loader loader)
		{
			Position = serializable.Position;
			Rotation = UnityEngine.Mathf.Clamp(serializable.Rotation, -360f, 360f);
			Spread = UnityEngine.Mathf.Clamp(serializable.Spread, 0f, 360f);

			OnDataDeserialized(serializable, loader);
		}

		public UnityEngine.Vector2 Position { get; private set; }
		public float Rotation { get; private set; }
		public float Spread { get; private set; }

		public static UAVLaunchPlatform DefaultValue { get; private set; }
	}
}
