using System.Linq;
using GameDatabase.DataModel;
using GameServices.Player;
using Services.Localization;
using Session;

namespace GameModel.Skills
{
    public enum SkillType
    {
        Undefined = 0,

        MainFuelCapacity = 1,
        MainEnginePower = 2,
        MainRescueUnit = 3,

        ShipAttack = 4,
        ShipDefense = 5,
        ShipExperience = 6,

        HeatDefense = 7,
        EnergyDefense = 8,
        KineticDefense = 9,

        Trading = 10,
        MasterTrader = 11,

        PlanetaryScanner = 12,
        SpaceScanner = 13,

        RequierementMaxLevel = 14,

        HangarSlot1 = 15,
        HangarSlot2 = 16,
        HangarSlot3 = 17,
        HangarSlot4 = 18,
        HangarSlot5 = 19,

        CraftingPrice = 20,
        CraftingLevel = 21,

        ShieldStrength = 22,
        ShieldRecharge = 23,

        RequierementBeatAllEnemies = 24,

        ExceedTheLimits = 25,

        HangarSlot6,
        RequierementMaxLevel2,
        RequierementLevel_80,
        RequierementLevel_100,
        RequierementLevel_120,

        EnergyShieldStrength,
        EnergyShieldRecharge,

        ShieldHeatDefense,
        ShieldEnergyDefense,
        ShieldKineticDefense,

        EnergyShieldHeatDefense,
        EnergyShieldEnergyDefense,
        EnergyShieldKineticDefense,

        QuantumDefense,
        ShieldQuantumDefense,
        EnergyShieldQuantumDefense,

        RequierementLevel_30,//42

		MoreEmery,
		MoreAlly,

        StealthRadar,
        JammingRadar,
        LargeShipExploration,
    }

    //   public enum SkillClass
    //{
    //	Common,
    //	Free,
    //	Locked,
    //}

    public static class SkillTypeExtensions
	{
	    public static bool IsCommonSkill(this SkillType type)
	    {
	        switch (type)
	        {
                case SkillType.Undefined:
                case SkillType.RequierementBeatAllEnemies:
                case SkillType.RequierementMaxLevel:
                case SkillType.RequierementMaxLevel2:
                case SkillType.RequierementLevel_80:
                case SkillType.RequierementLevel_100:
                case SkillType.RequierementLevel_120:
                case SkillType.RequierementLevel_30:
                    return false;
                default:
                    return true;
	        }
	    }

		//public static SkillClass Class(this SkillType type, PlayerFleet fleet = null, ISessionData session = null)
		//{
		//	switch (type)
		//	{
		//	case SkillType.Undefined:
		//		return SkillClass.Free;
		//	case SkillType.RequierementMaxLevel:
		//		return fleet != null && fleet.Ships.Any(item => item.Experience.Level == Maths.Experience.MaxPlayerRank) ? SkillClass.Free : SkillClass.Locked;
		//	case SkillType.RequierementBeatAllEnemies:
		//	    {
                    
		//	        session.Regions.GetCapturedFactions()

		//	    }

  //                  return SkillClass.Locked;
		//	default:
		//		return SkillClass.Common;
		//	}
		//}

		public static string GetName(this SkillType type, ILocalization localization)
		{
			switch (type) 
			{
			case SkillType.MainFuelCapacity:
				return localization.GetString("$FuelTankCapacity");
			case SkillType.MainEnginePower:
				return localization.GetString("$MainEnginePower");
			case SkillType.MainRescueUnit:
				return localization.GetString("$MainRescueUnit");
			case SkillType.ShipAttack:
				return localization.GetString("$AttackBooster");
			case SkillType.ShipDefense:
				return localization.GetString("$DefenseBooster");
			case SkillType.ShipExperience:
				return localization.GetString("$ExperienceBooster");
			case SkillType.RequierementMaxLevel:
            case SkillType.RequierementMaxLevel2:
            case SkillType.RequierementLevel_80:
            case SkillType.RequierementLevel_100:
            case SkillType.RequierementLevel_120:
            case SkillType.RequierementLevel_30:
			case SkillType.RequierementBeatAllEnemies:
				return localization.GetString("$ConditionNode");
            case SkillType.HangarSlot1:
                return localization.GetString("$HangarSlotNode");
            case SkillType.HangarSlot2:
            case SkillType.HangarSlot3:
            case SkillType.HangarSlot4:
            case SkillType.HangarSlot5:
            case SkillType.HangarSlot6:
                return localization.GetString("$UpgradeHangarSlotNode");
            case SkillType.PlanetaryScanner:
                return localization.GetString("$PlanetaryScannerNode");
            case SkillType.HeatDefense:
                return localization.GetString("$HeatDefenseNode");
            case SkillType.EnergyDefense:
                return localization.GetString("$EnergyDefenseNode");
            case SkillType.KineticDefense:
                return localization.GetString("$KineticDefenseNode");
            case SkillType.Trading:
                return localization.GetString("$TradingNode");
            case SkillType.MasterTrader:
                return localization.GetString("$MasterTraderNode");
            case SkillType.CraftingPrice:
                return localization.GetString("$CraftingPriceNode");
            case SkillType.CraftingLevel:
                return localization.GetString("$CraftingLevelNode");
			case SkillType.SpaceScanner:
			    return localization.GetString("$SpaceScannerNode");
			case SkillType.ShieldStrength:
			    return localization.GetString("$ShieldStrengthNode");
			case SkillType.ShieldRecharge:
			    return localization.GetString("$ShieldRechargeNode");
			case SkillType.ExceedTheLimits:
			    return localization.GetString("$ExceedTheLimitsNode");
			case SkillType.EnergyShieldStrength:
			    return localization.GetString("$EnergyShieldStrengthNode");
			case SkillType.EnergyShieldRecharge:
			    return localization.GetString("$EnergyShieldRechargeNode");
			case SkillType.ShieldHeatDefense:
			    return localization.GetString("$ShieldHeatDefenseNode");
			case SkillType.ShieldEnergyDefense:
			    return localization.GetString("$ShieldEnergyDefenseNode");
			case SkillType.ShieldKineticDefense:
			    return localization.GetString("$ShieldKineticDefenseNode");
			case SkillType.EnergyShieldHeatDefense:
			    return localization.GetString("$EnergyShieldHeatDefenseNode");
			case SkillType.EnergyShieldEnergyDefense:
			    return localization.GetString("$EnergyShieldEnergyDefenseNode");
			case SkillType.EnergyShieldKineticDefense:
			    return localization.GetString("$EnergyShieldKineticDefenseNode");
			case SkillType.QuantumDefense:
			    return localization.GetString("$QuantumDefenseNode");
			case SkillType.ShieldQuantumDefense:
			    return localization.GetString("$ShieldQuantumDefenseNode");
			case SkillType.EnergyShieldQuantumDefense:
			    return localization.GetString("$EnergyShieldQuantumDefenseNode");
			case SkillType.MoreEmery:
			    return localization.GetString("$MoreEmeryNode");
			case SkillType.MoreAlly:
			    return localization.GetString("$MoreAllyNode");
			case SkillType.StealthRadar:
			    return localization.GetString("$StealthRadarNode");
			case SkillType.JammingRadar:
			    return localization.GetString("$JammingRadarNode");
			case SkillType.LargeShipExploration:
			    return localization.GetString("$LargeShipExplorationNode");
            }

            return string.Empty;
		}

		public static string GetDescription(this SkillType type, ILocalization localization, int level = 1)
		{
			switch (type)
			{
				case SkillType.MainFuelCapacity:
					return localization.GetString("$FuelTankCapacityDesc", 50 * level);
				case SkillType.MainEnginePower:
					return localization.GetString("$MainEnginePowerDesc", 6 * level, 40 * level);
				case SkillType.MainRescueUnit:
					return localization.GetString("$MainRescueUnitDesc");
				case SkillType.ShipAttack:
					return localization.GetString("$AttackBoosterDesc", 10 * level);
				case SkillType.ShipDefense:
					return localization.GetString("$DefenseBoosterDesc", 10 * level);
				case SkillType.ShipExperience:
					return localization.GetString("$ExperienceBoosterDesc", 10 * level);
				case SkillType.RequierementMaxLevel:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.MaxPlayerRank);
				case SkillType.RequierementMaxLevel2:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.MaxPlayerRank2);
				case SkillType.RequierementLevel_80:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.PlayerRank_80);
				case SkillType.RequierementLevel_100:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.PlayerRank_100);
				case SkillType.RequierementLevel_120:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.PlayerRank_120);
				case SkillType.RequierementLevel_30:
					return localization.GetString("$ShipLevelsRequirement", level, Maths.Experience.PlayerRank_30);
				case SkillType.HangarSlot1:
					return localization.GetString("$NewHangarSlotDesc");
				case SkillType.HangarSlot2:
					return localization.GetString("$UpgradeHangarSlotDesc1");
				case SkillType.HangarSlot3:
					return localization.GetString("$UpgradeHangarSlotDesc2");
				case SkillType.HangarSlot4:
					return localization.GetString("$UpgradeHangarSlotDesc3");
				case SkillType.HangarSlot5:
					return localization.GetString("$UpgradeHangarSlotDesc4");
				case SkillType.HangarSlot6:
					return localization.GetString("$UpgradeHangarSlotDesc5");
				case SkillType.PlanetaryScanner:
					return localization.GetString("$PlanetaryScannerDesc", 10 * level);
				case SkillType.HeatDefense:
					return localization.GetString("$HeatDefenseDesc", 10 * level);
				case SkillType.EnergyDefense:
					return localization.GetString("$EnergyDefenseDesc", 10 * level);
				case SkillType.KineticDefense:
					return localization.GetString("$KineticDefenseDesc", 10 * level);
				case SkillType.Trading:
					return localization.GetString("$TradingDesc", 5 * level);
				case SkillType.MasterTrader:
					return localization.GetString("$MasterTraderDesc");
				case SkillType.CraftingPrice:
					return localization.GetString("$CraftingPriceDesc", 5 * level);
				case SkillType.CraftingLevel:
					return localization.GetString("$CraftingLevelDesc", 5 * level);
				case SkillType.SpaceScanner:
					return localization.GetString("$SpaceScannerDesc", level);
				case SkillType.ShieldStrength:
					return localization.GetString("$ShieldStrengthDesc", level * 10);
				case SkillType.ShieldRecharge:
					return localization.GetString("$ShieldRechargeDesc", level * 10);
				case SkillType.RequierementBeatAllEnemies:
					return localization.GetString("$BeatAllEnemiesRequirement");
				case SkillType.ExceedTheLimits:
					return localization.GetString("$ExceedTheLimitsDesc");
				case SkillType.EnergyShieldStrength:
					return localization.GetString("$EnergyShieldStrengthDesc", level * 10);
				case SkillType.EnergyShieldRecharge:
					return localization.GetString("$EnergyShieldRechargeDesc", level * 10);
				case SkillType.ShieldHeatDefense:
					return localization.GetString("$ShieldHeatDefenseDesc", level * 10);
				case SkillType.ShieldEnergyDefense:
					return localization.GetString("$ShieldEnergyDefenseDesc", level * 10);
				case SkillType.ShieldKineticDefense:
					return localization.GetString("$ShieldKineticDefenseDesc", level * 10);
				case SkillType.EnergyShieldHeatDefense:
					return localization.GetString("$EnergyShieldHeatDefenseDesc", level * 10);
				case SkillType.EnergyShieldEnergyDefense:
					return localization.GetString("$EnergyShieldEnergyDefenseDesc", level * 10);
				case SkillType.EnergyShieldKineticDefense:
					return localization.GetString("$EnergyShieldKineticDefenseDesc", level * 10);
				case SkillType.QuantumDefense:
					return localization.GetString("$QuantumDefenseDesc", level * 10);
				case SkillType.ShieldQuantumDefense:
					return localization.GetString("$ShieldQuantumDefenseDesc", level * 10);
				case SkillType.EnergyShieldQuantumDefense:
					return localization.GetString("$EnergyShieldQuantumDefenseDesc", level * 10);
                case SkillType.MoreEmery:
					return localization.GetString("$MoreEmeryDesc", level);
                case SkillType.MoreAlly:
                    return localization.GetString("$MoreAllyDesc", level);
                case SkillType.StealthRadar:
                    return localization.GetString("$StealthRadarDesc", level*5);
                case SkillType.JammingRadar:
                    return localization.GetString("$JammingRadarDesc", level*5);
                case SkillType.LargeShipExploration:
					return localization.GetString("$LargeShipExplorationDesc", level);
            }

            return string.Empty;
		}
	}
}
