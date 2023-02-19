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
	public partial class TagList
	{
		partial void OnDataDeserialized(TagListSerializable serializable, Database.Loader loader);

		public static TagList Create(TagListSerializable serializable, Database.Loader loader)
		{
			return new TagList(serializable, loader);
		}

		private TagList(TagListSerializable serializable, Database.Loader loader)
		{
			Tag = serializable.Tag;

			OnDataDeserialized(serializable, loader);
		}

		public string Tag { get; private set; }

		public static TagList DefaultValue { get; private set; }
	}
}
