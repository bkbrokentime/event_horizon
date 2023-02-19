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
	public partial class ComponentRestrictions
	{
		partial void OnDataDeserialized(ComponentRestrictionsSerializable serializable, Database.Loader loader);

		public static ComponentRestrictions Create(ComponentRestrictionsSerializable serializable, Database.Loader loader)
		{
			return new ComponentRestrictions(serializable, loader);
		}

		private ComponentRestrictions(ComponentRestrictionsSerializable serializable, Database.Loader loader)
		{
			ShipSizes = new ImmutableSet<SizeClass>(serializable.ShipSizes);
			NotForOrganicShips = serializable.NotForOrganicShips;
			NotForMechanicShips = serializable.NotForMechanicShips;
			MinCellAmount = UnityEngine.Mathf.Clamp(serializable.MinCellAmount, 0, 999999999);
			MaxCellAmount = UnityEngine.Mathf.Clamp(serializable.MaxCellAmount, 0, 999999999);
			OnlyForShipId = new ImmutableCollection<Ship>(serializable.OnlyForShipId?.Select(item => loader.GetShip(new ItemId<Ship>(item), true)));
			NotForShipId = new ImmutableCollection<Ship>(serializable.NotForShipId?.Select(item => loader.GetShip(new ItemId<Ship>(item), true)));
			UniqueComponentTag = new ImmutableCollection<TagList>(serializable.UniqueComponentTag?.Select(item => TagList.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public ImmutableSet<SizeClass> ShipSizes { get; private set; }
		public bool NotForOrganicShips { get; private set; }
		public bool NotForMechanicShips { get; private set; }
		public int MinCellAmount { get; private set; }
		public int MaxCellAmount { get; private set; }
		public ImmutableCollection<Ship> OnlyForShipId { get; private set; }
		public ImmutableCollection<Ship> NotForShipId { get; private set; }
		public ImmutableCollection<TagList> UniqueComponentTag { get; private set; }

		public static ComponentRestrictions DefaultValue { get; private set; }
	}
}
