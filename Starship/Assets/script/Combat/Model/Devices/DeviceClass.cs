using System.Drawing;
using System.Linq;
using Combat.Component.Bullet.Lifetime;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace Model 
{
	//public enum DeviceClass
	//{
	//	Accelerator,
	//	Decoy,
	//	EnergyShield,
	//	Ghost,
	//	GravityGenerator,
	//	PartialShield,
	//	PointDefense,
	//	RepairBot,
	//	Detonator,
	//	Stealth,
	//	Teleporter,
	//	Brake,
	//	SuperStealth,
	//	Fortification,
 //       ToxicWaste,
	//}
	
	public static class DeviceClassExtension
	{
	    public static bool IsSuitable(this DeviceClass type, Constructor.Ships.IShipModel ship)
	    {
	        switch (type)
	        {
                case DeviceClass.RepairBot:
	                return !ship.IsBionic;
	        }

            return ship.BuiltinDevices.All(item => item.Stats.DeviceClass != type);
	    }

        //public static void CreateDeviceEffect(this DeviceClass type, IDevice model, DeviceStats deviceStats, IBindingManager bindingManager, BindingFactoryDeprecated bindingFactory)
        //{
        //       if (!deviceStats.EffectPrefab)
        //           return;

        //    switch (type)
        //    {
        //        case DeviceClass.Accelerator:
        //           case DeviceClass.Detonator:
        //           case DeviceClass.EnergyShield:
        //           case DeviceClass.GravityGenerator:
        //           case DeviceClass.RepairBot:
        //           case DeviceClass.PointDefense:
        //           case DeviceClass.Teleporter:
        //               bindingManager.AddBinding(bindingFactory.CreateDeviceEffectBinding(model, deviceStats.EffectPrefab, deviceStats.Color));
        //               break;
        //       }
        //   }

        //      public static IDevice Create(this DeviceClass type, IShipCombatModel ship, DeviceStats deviceSpec, ShipStats shipSpec, ISoundPlayer soundPlayer, ISceneObsolete scene, IBindingManager bindingManager, IObjectPool objectPool, Settings settings)
        //{
        //          var effectFactory = new EffectFactoryObsolete(scene, bindingManager, objectPool);
        //          var objectFactory = new SpaceObjectFactoryObsolete(scene, bindingManager, objectPool);

        //	switch (type)
        //	{
        //	case DeviceClass.Accelerator:
        //		return new AcceleratorDevice(ship, deviceSpec, shipSpec, soundPlayer);
        //	case DeviceClass.Decoy:
        //		return new DecoyDevice(ship, deviceSpec, shipSpec, soundPlayer, objectFactory);
        //	case DeviceClass.EnergyShield:
        //		return new EnergyShieldDevice(ship, deviceSpec, shipSpec);
        //	case DeviceClass.Ghost:
        //		return new GhostDevice(ship, deviceSpec, shipSpec, soundPlayer, effectFactory);
        //	case DeviceClass.GravityGenerator:
        //		return new GravityGenerator(ship, deviceSpec, shipSpec, scene, soundPlayer);
        //	case DeviceClass.PartialShield:
        //		return new PartialShieldDevice(ship, deviceSpec, shipSpec, scene, objectFactory);
        //	case DeviceClass.PointDefense:
        //		return new PointDefenseSystem(ship, deviceSpec, shipSpec, scene);
        //	case DeviceClass.RepairBot:
        //		return new RepairSystem(ship, deviceSpec, shipSpec, scene, soundPlayer, objectFactory, effectFactory, settings);
        //	case DeviceClass.Detonator:
        //		return new SelfDestructDevice(ship, deviceSpec, shipSpec, soundPlayer, objectFactory, settings);
        //	case DeviceClass.Stealth:
        //		return new StealthDevice(ship, deviceSpec, shipSpec, false);
        //	case DeviceClass.Teleporter:
        //		return new TeleporterDevice(ship, deviceSpec, shipSpec, soundPlayer, effectFactory);
        //	case DeviceClass.Brake:
        //		return new BrakeDevice(ship, deviceSpec, shipSpec);
        //	case DeviceClass.SuperStealth:
        //		return new StealthDevice(ship, deviceSpec, shipSpec, true);
        //	case DeviceClass.Fortification:
        //		return new FortificationDevice(ship, deviceSpec, shipSpec, soundPlayer);
        //          case DeviceClass.ToxicWaste:
        //              return new ToxicWaste(ship, deviceSpec, shipSpec, objectFactory);
        //	}

        //	ExceptionHandler.HandleException("Bad device class: " + type);
        //	return null;
        //}
        public static string Name(this DeviceClass type)
        {
            switch (type)
            {
                case DeviceClass.ClonningCenter:
                    return "$ClonningCenter";
                case DeviceClass.TimeMachine:
                    return "$TimeMachine";
                case DeviceClass.Accelerator:
                    return "$Accelerator";
                case DeviceClass.Decoy:
                    return "$Decoy";
                case DeviceClass.Ghost:
                    return "$Ghost";
                case DeviceClass.PointDefense:
                    return "$PointDefense";
                case DeviceClass.GravityGenerator:
                    return "$GravityGenerator";
                case DeviceClass.EnergyShield:
                    return "$EnergyShield";
                case DeviceClass.PartialShield:
                    return "$PartialShield";
                case DeviceClass.Denseshield:
                    return "$Denseshield";
                case DeviceClass.Fortification:
                    return "$Fortification";
                case DeviceClass.CombustionInhibition:
                    return "$CombustionInhibition";
                case DeviceClass.EnergyDiversion:
                    return "$EnergyDiversion";
                case DeviceClass.RepairBot:
                    return "$RepairBot";
                case DeviceClass.Detonator:
                    return "$Detonator";
                case DeviceClass.Stealth:
                    return "$Stealth";
                case DeviceClass.Teleporter:
                    return "$Teleporter";
                case DeviceClass.Brake:
                    return "$Brake";
                case DeviceClass.SuperStealth:
                    return "$SuperStealth";
                case DeviceClass.FireAssault:
                    return "$FireAssault";
                case DeviceClass.ToxicWaste:
                    return "$ToxicWaste";
                case DeviceClass.WormTail:
                    return "$WormTail";
                case DeviceClass.Equipment:
                    return "$Equipment";
                default:
                    return " ";
            }
        }
    }
}