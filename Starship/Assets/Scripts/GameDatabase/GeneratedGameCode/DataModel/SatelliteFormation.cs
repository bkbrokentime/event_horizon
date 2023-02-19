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
	public partial class SatelliteFormation
	{
		partial void OnDataDeserialized(SatelliteFormationSerializable serializable, Database.Loader loader);

		public static SatelliteFormation Create(SatelliteFormationSerializable serializable, Database.Loader loader)
		{
			return new SatelliteFormation(serializable, loader);
		}

		private SatelliteFormation(SatelliteFormationSerializable serializable, Database.Loader loader)
		{
			Position = serializable.Position;
			Rotation = UnityEngine.Mathf.Clamp(serializable.Rotation, -360f, 360f);

			OnDataDeserialized(serializable, loader);
		}

		public UnityEngine.Vector2 Position { get; private set; }
		public float Rotation { get; private set; }

		public static SatelliteFormation DefaultValue { get; private set; }
	}
}
