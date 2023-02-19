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
	public partial class ExplorationResourceItem
	{
		partial void OnDataDeserialized(ExplorationResourceItemSerializable serializable, Database.Loader loader);

		public static ExplorationResourceItem Create(ExplorationResourceItemSerializable serializable, Database.Loader loader)
		{
			return new ExplorationResourceItem(serializable, loader);
		}

		private ExplorationResourceItem(ExplorationResourceItemSerializable serializable, Database.Loader loader)
		{
			Type = serializable.Type;
			ExplorationLoot = new ImmutableCollection<ResourceItem>(serializable.ExplorationLoot?.Select(item => ResourceItem.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public ExplorationType Type { get; private set; }
		public ImmutableCollection<ResourceItem> ExplorationLoot { get; private set; }

		public static ExplorationResourceItem DefaultValue { get; private set; }
	}
}
