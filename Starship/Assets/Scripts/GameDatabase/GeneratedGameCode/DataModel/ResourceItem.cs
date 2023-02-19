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
	public partial class ResourceItem
	{
		partial void OnDataDeserialized(ResourceItemSerializable serializable, Database.Loader loader);

		public static ResourceItem Create(ResourceItemSerializable serializable, Database.Loader loader)
		{
			return new ResourceItem(serializable, loader);
		}

		private ResourceItem(ResourceItemSerializable serializable, Database.Loader loader)
		{
			QuestItem = loader.GetQuestItem(new ItemId<QuestItem>(serializable.ItemId));
			if (QuestItem == null)
			    throw new DatabaseException(this.GetType().Name + ".QuestItem cannot be null - " + serializable.ItemId);
			Chance = UnityEngine.Mathf.Clamp(serializable.Chance, 0, 100);
			MaxAmount = UnityEngine.Mathf.Clamp(serializable.MaxAmount, 0, 999999999);

			OnDataDeserialized(serializable, loader);
		}

		public QuestItem QuestItem { get; private set; }
		public int Chance { get; private set; }
		public int MaxAmount { get; private set; }

		public static ResourceItem DefaultValue { get; private set; }
	}
}
