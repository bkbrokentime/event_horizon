using GameDatabase.Enums;

namespace Constructor
{
	public static class ComponentTypeExtension
	{
		public static bool CompatibleWith(this CellType component, CellType target)
		{
			if (target == CellType.Empty || target == CellType.Custom)
				return false;
			if (component == CellType.Empty || component == target)
				return true;


			if (target == CellType.InnerOuter && (component == CellType.Inner || component == CellType.Outer))
				return true;
			if (component == CellType.InnerOuter && (target == CellType.Inner || target == CellType.Outer))
				return true;
			//
			if (component == CellType.InnerSpecial && (target == CellType.Inner || target == CellType.Special))
				return true;
			if (target == CellType.InnerSpecial && (component == CellType.Inner || component == CellType.Special))
				return true;
			if (component == CellType.OuterUAVPlatform && (target == CellType.Outer || target == CellType.UAVPlatform))
				return true;
			if (target == CellType.OuterUAVPlatform && (component == CellType.Outer || component == CellType.UAVPlatform))
				return true;

			if ((component.CellId() & target.CellId()) != 0)
				return true;

			return false;
		}

		public static int CellId(this CellType type)
		{
			switch(type)
			{
				case CellType.Weapon:
					return 0x0001;
				case CellType.Engine:
                    return 0x0002;
                case CellType.Inner:
                    return 0x0004;
                case CellType.Outer:
                    return 0x0008;
                case CellType.Special:
                    return 0x0010;
                case CellType.UAVPlatform:
                    return 0x0020;

                case CellType.InnerOuter:
					return CellId(CellType.Inner) | CellId(CellType.Outer);
                case CellType.InnerSpecial:
                    return CellId(CellType.Inner) | CellId(CellType.Special);
                case CellType.OuterUAVPlatform:
                    return CellId(CellType.Outer) | CellId(CellType.UAVPlatform);
                default:
                    return 0;
            }
        }
	}
}
