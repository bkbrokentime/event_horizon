using Constructor.Component;
using Domain.Quests;
using GameDatabase.Enums;

namespace Constructor
{
	public static class ComponentExtension
	{
	    public static IComponent Create(this GameDatabase.DataModel.Component component, int shipSize)
	    {
	        return new CommonComponent(component, shipSize);
	    }

        public static string[] GetUniqueKey(this GameDatabase.DataModel.Component component)
        {
            var Keys = new string[0];
            if (component.Restrictions.UniqueComponentTag.Count > 0)
                foreach (var key in component.Restrictions.UniqueComponentTag)
                    Keys.Add(key.Tag);

            if (component.Device != null)
            {
                switch (component.Device.Stats.DeviceClass)
                {
                    case DeviceClass.Teleporter:
                    case DeviceClass.Fortification:
                    case DeviceClass.Equipment:
                    case DeviceClass.CombustionInhibition:
                    case DeviceClass.EnergyDiversion:
                    case DeviceClass.Denseshield:
                    case DeviceClass.Brake:
                    case DeviceClass.RepairBot:
                    case DeviceClass.PointDefense:
                    case DeviceClass.GravityGenerator:
                    case DeviceClass.Ghost:
                    case DeviceClass.Decoy:
                    case DeviceClass.Detonator:
                    case DeviceClass.Accelerator:
                    case DeviceClass.ToxicWaste:
                    case DeviceClass.FireAssault:
                        Keys.Add(component.Device.Stats.DeviceClass.ToString());
                        break;
                    case DeviceClass.Stealth:
                    case DeviceClass.SuperStealth:
                        Keys.Add(DeviceClass.Stealth.ToString());
                        break;
                    case DeviceClass.EnergyShield:
                    case DeviceClass.PartialShield:
                        Keys.Add(DeviceClass.EnergyShield.ToString());
                        break;
                    case DeviceClass.WormTail:
                        Keys.Add(DeviceClass.WormTail.ToString());
                        break;
                    default:
                        break;
                }
            }

            return Keys;
        }
    }
}
