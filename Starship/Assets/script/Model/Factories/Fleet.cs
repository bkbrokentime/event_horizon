using System;
using System.Collections.Generic;
using System.Linq;
using Database.Legacy;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Model.Military;

namespace Model
{
	namespace Factories
	{
		public static class Fleet
		{
			public static IFleet Common(int distance, Faction faction, int seed, IDatabase database)
			{
				var random = new Random(seed);
				var count = Maths.Distance.FleetSize(distance, random);
				var ships = database.ShipBuildList.Available().Common().OfFaction(faction,distance).LimitLevelAndClass(distance).RandomElements(count, random).OrderBy(item => random.Next());
				return new CommonFleet(database, ships, distance, random.Next());
			}

			public static IFleet Boss(int distance, Faction faction, int seed, IDatabase database)
			{
				var random = new Random(seed);
				var count = Maths.Distance.FleetSize(distance, random) - 1;
				var bossClass = distance > 50 ? DifficultyClass.Class2 : DifficultyClass.Class1;
				
				var boss = database.ShipBuildList.Available().Flagships().OfFaction(faction,distance).OfClass(DifficultyClass.Class1, bossClass).RandomElement(random) ??
                   database.ShipBuildList.Available().Flagships().OfClass(DifficultyClass.Class1, bossClass).RandomElement(random);

				var ships = database.ShipBuildList.Available().NormalShips().OfFaction(faction,distance).LimitLevelAndClass(distance).RandomElements(count, random).OrderBy(item => random.Next());
				return new CommonFleet(database, ships.Prepend(boss), distance, random.Next());
			}

			public static IFleet Bossesfleet(int distance, Faction faction, int seed, IDatabase database, int level)
			{
				var random = new Random(seed);
				var count = Maths.Distance.FleetSize(distance, random)*level*level - 1;
				var bossClass = distance > 50 ? DifficultyClass.Class2 : DifficultyClass.Class1;
				/*
				var boss = database.ShipBuildList.Available().Flagships().OfFaction(faction,distance).OfClass(DifficultyClass.Class1, bossClass).RandomElement(random) ??
                           database.ShipBuildList.Available().Flagships().OfClass(DifficultyClass.Class1, bossClass).RandomElement(random);
				*/
				var bosses = database.ShipBuildList.Available().Flagships().OfFaction(faction, distance).OfClass(DifficultyClass.Class1, bossClass).RandomElements(level, random) ??
					         database.ShipBuildList.Available().Flagships().OfClass(DifficultyClass.Class1, bossClass).RandomElements(level, random);


				var ships = database.ShipBuildList.Available().NormalShips().OfFaction(faction,distance).LimitLevelAndClass(distance).RandomElements(count, random).OrderBy(item => random.Next());

				return new CommonFleet(database, bosses.Concat(ships), distance, random.Next());
			}

		    public static IFleet SingleBoss(int distance, Faction faction, int seed, IDatabase database)
		    {
		        var random = new Random(seed);
		        var bossClass = distance < 50 ? DifficultyClass.Default : distance < 150 ? DifficultyClass.Class1 : DifficultyClass.Class2;
		        var boss = database.ShipBuildList.Available().Flagships().OfFaction(faction, distance).OfClass(bossClass, bossClass).RandomElements(1, random);
		        return new CommonFleet(database, boss, distance, random.Next());
		    }

			public static IFleet Faction(GameModel.Region region, int seed, IDatabase database)
			{
				var random = new Random(seed);
				var distance = region.MilitaryPower;
				var count = Maths.Distance.FleetSize(distance, random);
				var ships = database.ShipBuildList.Available().NormalShips().OfFaction(region.Faction).LimitLevelAndClass(distance).RandomElements(count, random);
				return new CommonFleet(database, ships, distance, random.Next());
			}

			public static IFleet Capital(GameModel.Region region, IDatabase database)
			{
				var seed = region.HomeStar;
				var random = new Random(seed);

				var distance = region.MilitaryPower;

				var numberOfShips = 8 + (int)Math.Round(5 * region.BaseDefensePower);
				var numberOfBosses = 1 + (int)Math.Floor(region.BaseDefensePower);
				var bossClass = numberOfBosses >= 2 ? DifficultyClass.Class2 : DifficultyClass.Class1;
				var bosses = database.ShipBuildList.Available().Where(item => item.Ship.ShipCategory != ShipCategory.Starbase && item.Ship.SizeClass == SizeClass.Titan).
                    OfFactionExplicit(region.Faction).OfClass(DifficultyClass.Class1, bossClass).RandomElements(numberOfBosses, random);
                var ships = database.ShipBuildList.Available().NormalShips().OfFactionExplicit(region.Faction).LimitLevelAndClass(distance).RandomElements(numberOfShips, random);

                var starbaseClass = region.MilitaryPower < 40 ? DifficultyClass.Default : DifficultyClass.Class1;
			    var starbase = database.ShipBuildList.Available().Where(item => item.Ship.ShipCategory == ShipCategory.Starbase && item.BuildFaction == region.Faction).BestAvailableClass(starbaseClass).FirstOrDefault();

                var fleet = (starbase == null ? Enumerable.Empty<ShipBuild>() : Enumerable.Repeat(starbase, 1)).Concat((bosses.Concat(ships).OrderBy(item => random.Next())));
				return new CommonFleet(database, fleet, distance, random.Next());
			}

            public static IFleet Fortification(GameModel.Region region, IDatabase database)
            {
                var seed = region.HomeStar;
                var random = new Random(seed);

                var distance = region.MilitaryPower;

				var numberOfShips = 5 + (region.IsCaptured ? 0 : (int)Math.Round(5 * region.BaseDefensePower * 0.3f));
				var numberOfBosses = 1 + (region.IsCaptured ? 0 : (int)Math.Floor(region.BaseDefensePower * 0.3f));
                var bossClass = numberOfBosses >= 2 ? DifficultyClass.Class2 : DifficultyClass.Class1;
                var bosses = database.ShipBuildList.Available().Where(item => item.Ship.ShipCategory != ShipCategory.Starbase && item.Ship.SizeClass == SizeClass.Titan).
                    OfFactionExplicit(region.Faction).OfClass(DifficultyClass.Class1, bossClass).RandomElements(numberOfBosses, random);
                var ships = database.ShipBuildList.Available().NormalShips().OfFactionExplicit(region.Faction).LimitLevelAndClass(distance).RandomElements(numberOfShips, random);

                var starbaseClass = region.MilitaryPower < 40 ? DifficultyClass.Default : DifficultyClass.Class1;
                var starbase = database.ShipBuildList.Available().Where(item => item.Ship.ShipCategory == ShipCategory.Starbase && item.BuildFaction == region.Faction).BestAvailableClass(starbaseClass).FirstOrDefault();

                var fleet = (starbase == null ? Enumerable.Empty<ShipBuild>() : Enumerable.Repeat(starbase, 1)).Concat((bosses.Concat(ships).OrderBy(item => random.Next())));
                return new CommonFleet(database, fleet, distance, random.Next());
            }

            public static IFleet Ruins(int distance, int seed, IDatabase database)
            {
                var random = new Random(seed);
                var ships = database.ShipBuildList.Available().NormalShips().OfFaction(database.GalaxySettings.AbandonedStarbaseFaction).GreaterOrEqualClass(DifficultyClass.Class1).LimitLevel(distance).
                    RandomElements(Maths.Distance.FleetSize(distance, random) * 2, random);

                return new CommonFleet(database, ships, distance, random.Next());
            }

            public static IFleet Xmas(int distance, int seed, IDatabase database)
            {
                var random = new Random(seed);

                var starbase = database.GetShipBuild(new ItemId<ShipBuild>(232));
                var hidden = database.ShipBuildList.Neutral().Where(item => item.Ship.ShipCategory == ShipCategory.Hidden).GreaterOrEqualClass(DifficultyClass.Class1).LimitLevel(distance*2);
                var normal = database.ShipBuildList.Available().ShipsAndFlagships().GreaterOrEqualClass(DifficultyClass.Class1).LimitLevel(distance);

                var ships = starbase.ToEnumerable().Concat(hidden.Concat(normal.RandomElements(Maths.Distance.FleetSize(distance, random), random)).OrderBy(item => random.Next()));
                return new CommonFleet(database, ships, distance, random.Next());
            }

            public static IFleet Arena(int distance, int seed, IDatabase database)
			{
				var random = new Random(seed);
				var ships = database.ShipBuildList.Available().LimitLevelAndClass(distance).ShipsAndFlagships().RandomUniqueElements(1, random);
				return new CommonFleet(database, ships, distance, random.Next());
			}

			public static IFleet Survival(int distance, Faction faction, int seed, IDatabase database)
			{
				int fleetSize = 100 + distance / 10;
				var random = new Random(seed);
				var numberOfRandomShips = fleetSize/10;
				var randomShips = database.ShipBuildList.Available().ShipsAndFlagships().RandomElements(numberOfRandomShips, random);
				var factionShips = database.ShipBuildList.Available().NormalShips().OfFaction(faction,distance).RandomElements(fleetSize - numberOfRandomShips, random);
				return new SurvivalFleet(database, factionShips.Concat(randomShips).OrderBy(item => item.Ship.Layout.CellCount + item.Ship.SecondLayout.CellCount + random.Next(20)), distance, random.Next());
			}

			public static IFleet Endlessness(int distance, Faction faction, int seed, IDatabase database)
			{
				int fleetSize = 500 + distance * 5;
				var random = new Random(seed);
				var numberOfRandomShips = fleetSize/5;
				var randomShips = database.ShipBuildList.Available().ShipsAndFlagships().RandomElements(numberOfRandomShips, random);
				var factionShips = database.ShipBuildList.Available().NormalShips().OfFaction(faction,distance).RandomElements(fleetSize - numberOfRandomShips, random);
				return new SurvivalFleet(database, factionShips.Concat(randomShips).OrderBy(item => item.Ship.Layout.CellCount + item.Ship.SecondLayout.CellCount + random.Next(50)), distance, random.Next());
			}

			public static IFleet Training(IDatabase database, int distance, int faction, int seed)
			{
				var random = new Random(seed);
				var ships = database.ShipBuildList.Available().Common().LimitLevelAndClass(distance).RandomElements(20, random).OrderBy(item => random.Next());
				return new CommonFleet(database, ships, distance, random.Next());
			}

			public static IFleet Tutorial(IDatabase database)
			{
				var ships = new List<ShipBuild>();
				ships.Add(database.GetShipBuild(LegacyShipBuildNames.GetId("Invader3")));
				ships.Add(database.GetShipBuild(LegacyShipBuildNames.GetId("Invader3")));
				ships.Add(database.GetShipBuild(LegacyShipBuildNames.GetId("Invader3")));

				return new CommonFleet(database, ships, 0, 0);
			}

			public static IFleet Player(GameServices.Player.PlayerFleet fleet, IDatabase database)
			{
				return new PlayerFleet(database, fleet);
			}

			public static readonly IFleet Empty = new CommonFleet(null, Enumerable.Empty<ShipBuild>(), 0, 0);
		}
	}
}
