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
	public partial class GalaxySettings
	{
		partial void OnDataDeserialized(GalaxySettingsSerializable serializable, Database.Loader loader);

		public static GalaxySettings Create(GalaxySettingsSerializable serializable, Database.Loader loader)
		{
			return new GalaxySettings(serializable, loader);
		}

		private GalaxySettings(GalaxySettingsSerializable serializable, Database.Loader loader)
		{
			AbandonedStarbaseFaction = loader.GetFaction(new ItemId<Faction>(serializable.AbandonedStarbaseFaction));
			InitialStarbaseFaction = loader.GetFaction(new ItemId<Faction>(serializable.InitialStarbaseFaction));
			MotherShip = loader.GetShipBuild(new ItemId<ShipBuild>(serializable.MotherShip));
			StartingShipBuilds = new ImmutableCollection<ShipBuild>(serializable.StartingShipBuilds?.Select(item => loader.GetShipBuild(new ItemId<ShipBuild>(item), true)));

			OnDataDeserialized(serializable, loader);
		}

		public Faction AbandonedStarbaseFaction { get; private set; }
		public Faction InitialStarbaseFaction { get; private set; }
		public ShipBuild MotherShip { get; private set; }
		public ImmutableCollection<ShipBuild> StartingShipBuilds { get; private set; }

		public static GalaxySettings DefaultValue { get; private set; }
	}
}
