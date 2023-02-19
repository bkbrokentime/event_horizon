using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Common;
using Constructor;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameServices.Random;
using Constructor.Ships;
using Database.Legacy;
using Game;
using Game.Exploration;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using GameModel;
using GameServices.Player;
using Zenject;
using Component = GameDatabase.DataModel.Component;
using static Maths.Threat;

namespace GameServices.Economy
{
    public class LootGenerator
    {
        [Inject] private readonly ItemTypeFactory _factory;
        [Inject] private readonly Research.Research _research;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly HolidayManager _holidayManager;
        [Inject] private readonly PlayerSkills _playerSkills;

        public ItemTypeFactory Factory { get { return _factory; } }

        public IEnumerable<IProduct> GetCommonReward(IEnumerable<IShip> ships, int distance, Faction faction, int seed)
        {
            var random = _random.CreateRandom(seed);

            var scraps = 0;
            var money = 0;

            var moduleLevel = Maths.Distance.ComponentLevel(distance);

            foreach (var ship in ships)
            {
                scraps += ship.Scraps() * random.Next(80, 120) / 100;
                money += ship.Price()/ 20 * random.Next(80, 120) / 100;

                if (ship.Model.Category == ShipCategory.Flagship||ship.Model.Category == ShipCategory.SuperFlagship)
                {
                    var bossFaction = ship.Model.Faction;
                    foreach (var item in RandomComponents(moduleLevel + 35, random.Next(1, 2), bossFaction, random, false, false))
                        yield return new Product(item);

                    if (ship.ExtraThreatLevel >= DifficultyClass.Class2)
                    {
                        yield return Price.Premium(1).GetProduct(_factory);
                        foreach (var item in RandomComponents(moduleLevel + 75, random.Next(1, 3), bossFaction, random, false, false))
                            yield return new Product(item);
                    }
                }
                else
                {
                    foreach (var item in RandomComponents(moduleLevel, random.Next(-10, 2), faction, random, false, false))
                        yield return new Product(item);
                }
            }

            var star = random.NextN(money / 5000, 5) - 1;
            if (star > 0)
            {
                money -= star * 5000;
                yield return Price.Premium(star).GetProduct(_factory);
            }

            if (money > 0)
                yield return Price.Common(money).GetProduct(_factory);

            var toxicWaste = random.Next2(scraps/2);
            if (toxicWaste > 0)
                yield return new Product(CreateArtifact(CommodityType.ToxicWaste), toxicWaste);

            scraps -= toxicWaste;
            if (scraps > 0)
                yield return new Product(CreateArtifact(CommodityType.Scraps), scraps);

            foreach (var item in GetHolidayLoot(random))
                yield return item;
        }

        public IEnumerable<IProduct> GetSocialShareReward()
        {
            yield return Price.Premium(10).GetProduct(_factory);
        }

        public IEnumerable<IProduct> GetAdReward()
        {
            yield return Price.Premium(1).GetProduct(_factory);
        }

        public IEnumerable<IProduct> GetHolidayLoot(System.Random random)
        {
            if (_holidayManager.IsChristmas)
            {
                if (random.Percentage(33))
                    yield return new Product(_factory.CreateCurrencyItem(Currency.Snowflakes));
            }
        }

        public IEnumerable<IProduct> GetMeteoriteLoot(Faction faction, int level, int seed)//小行星
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            yield return new Product(CreateArtifact(CommodityType.Minerals), 1 + random.Next2(Mathf.RoundToInt(30 + 10 * multiplier)));
            if (random.Percentage(Mathf.RoundToInt(2+2 * quality)))
                yield return new Product(CreateArtifact(CommodityType.Gems), 1 + random.Next2(Mathf.RoundToInt(2 + 1 * multiplier)));
            if (random.Percentage(Mathf.RoundToInt(10+10 * quality)))
                yield return new Product(CreateArtifact(CommodityType.PreciousMetals), 1 + random.Next2(Mathf.RoundToInt(10 + 2 * multiplier)));

            var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Meteorite);
            if (resources.Count() > 0)
            {
                var resource = resources.RandomElement(random);
                foreach (var item in resource.ExplorationLoot)
                    if (random.Percentage(item.Chance))
                        yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
            }
        }

        public IEnumerable<IProduct> GetOutpostLoot(Faction faction, int level, int seed)//前哨站
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            yield return new Product(CreateArtifact(CommodityType.Scraps), 1 + random.Next2(Mathf.RoundToInt(200 + 50 * multiplier)));

            if (random.Percentage(Mathf.RoundToInt(10 + 5 * quality)))
            {
                var tech = _research.GetAvailableTechs(faction).Where(item => item.Hidden || item.Price <= 10).RandomElement(random);
                if (tech != null)
                    yield return new Product(_factory.CreateBlueprintItem(tech));
            }

            for (var i = 0; i < random.Next(Mathf.RoundToInt(3 + 2 * multiplier)); ++i)
                yield return new Product(RandomComponent(level, faction, random, true, false));

            if (random.Percentage(Mathf.RoundToInt(25+25 * quality)))
                yield return new Product(_factory.CreateResearchItem(faction));

            yield return Price.Premium(1 + random.Next(Mathf.RoundToInt(3 + 2 * multiplier))).GetProduct(_factory);

            var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Outpost);
            if (resources.Count() > 0)
            {
                var resource = resources.RandomElement(random);
                foreach (var item in resource.ExplorationLoot)
                    if (random.Percentage(item.Chance))
                        yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
            }
        }

        public IEnumerable<IProduct> GetHiveLoot(int level, int seed)//母巢
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            yield return new Product(CreateArtifact(CommodityType.Artifacts), 1 + random.Next2(Mathf.RoundToInt(3 + 2 * multiplier)));
            for (var i = 0; i < random.Next(Mathf.RoundToInt(1 + 0.5f * multiplier)); ++i)
                if (random.Percentage(Mathf.RoundToInt(20+20 * quality)))
                    yield return new Product(RandomComponent(level, _database.ExplorationSettings.InfectedPlanetFaction, random, true, false));

            //if (random.Percentage(20 + quality / 10))
            //    yield return new Product(RandomComponent(level, _database.ExplorationSettings.InfectedPlanetFaction, random, true, false));
            //if (random.Percentage(20 + quality / 10))
            //    yield return new Product(RandomComponent(level, _database.ExplorationSettings.InfectedPlanetFaction, random, true, false));

            if (random.Percentage(Mathf.RoundToInt(15 + 10 * quality)))
                yield return new Product(RandomFactionShip(level, _database.ExplorationSettings.InfectedPlanetFaction, random));

            if (random.Percentage(Mathf.RoundToInt(10 + 5 * quality)))
            {
                var tech = _research.GetAvailableTechs((_database.ExplorationSettings.InfectedPlanetFaction)).Where(item => item.Hidden || item.Price <= 10).RandomElement(random);
                if (tech != null)
                    yield return new Product(_factory.CreateBlueprintItem(tech));
            }

            yield return Price.Premium(1 + random.Next(Mathf.RoundToInt(3 + 2 * multiplier))).GetProduct(_factory);

            var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Hive);
            if (resources.Count() > 0)
            {
                var resource = resources.RandomElement(random);
                foreach (var item in resource.ExplorationLoot)
                    if (random.Percentage(item.Chance))
                        yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
            }
        }

        public IEnumerable<IProduct> GetPlanetResources(PlanetType planetType, Faction faction, int level, int seed, bool rare = false)//矿脉
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            if (planetType == PlanetType.Gas)
            {
                yield return new Product(CreateArtifact(CommodityType.ToxicWaste), 1 + random.Next2(Mathf.RoundToInt(200 + 100 * multiplier)));
                if (rare && random.Percentage(Mathf.RoundToInt(10+10 * quality)))
                    yield return new Product(_factory.CreateFuelItem(), 1 + random.Next2(Mathf.RoundToInt(5 + 5 * multiplier)));

                if (rare)
                {
                    var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.GasRare);
                    if (resources.Count() > 0)
                    {
                        var resource = resources.RandomElement(random);
                        foreach (var item in resource.ExplorationLoot)
                            if (random.Percentage(item.Chance))
                                yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
                    }
                }
                else
                {
                    var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Gas);
                    if (resources.Count() > 0)
                    {
                        var resource = resources.RandomElement(random);
                        foreach (var item in resource.ExplorationLoot)
                            if (random.Percentage(item.Chance))
                                yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
                    }
                }
            }
            else
            {
                yield return new Product(CreateArtifact(CommodityType.Minerals), 1 + random.Next2(Mathf.RoundToInt(50 + 20 * multiplier)));
                if (rare && random.Percentage(Mathf.RoundToInt(2+2 * quality)))
                    yield return new Product(CreateArtifact(CommodityType.Gems), 1 + random.Next2(Mathf.RoundToInt(5 + 2 * multiplier)));
                if (random.Percentage(Mathf.RoundToInt(10+10 * quality)))
                    yield return new Product(CreateArtifact(CommodityType.PreciousMetals), 1 + random.Next2(Mathf.RoundToInt(15 + 3 * multiplier)));

                if (rare)
                {
                    var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.MineralsRare);
                    if (resources.Count() > 0)
                    {
                        var resource = resources.RandomElement(random);
                        foreach (var item in resource.ExplorationLoot)
                            if (random.Percentage(item.Chance))
                                yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
                    }
                }
                else
                {
                    var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Minerals);
                    if (resources.Count() > 0)
                    {
                        var resource = resources.RandomElement(random);
                        foreach (var item in resource.ExplorationLoot)
                            if (random.Percentage(item.Chance))
                                yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
                    }
                }
            }
        }

        public IEnumerable<IProduct> GetPlanetRareResources(PlanetType planetType, Faction faction, int level, int seed)
        {
            return GetPlanetResources(planetType, faction, level, seed, true);
        }

        public IEnumerable<IProduct> GetContainerLoot(Faction faction, int level, int seed)//容器
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            yield return new Product(_factory.CreateCurrencyItem(Currency.Credits), Maths.Distance.Credits(level) + random.Next2(Mathf.RoundToInt(2000 + 1000 * multiplier)));

            if (random.Percentage(30))
                yield return new Product(CreateArtifact(CommodityType.Alloys), 1 + random.Next2(Mathf.RoundToInt(30 + 10 * multiplier)));
            if (random.Percentage(30))
                yield return new Product(CreateArtifact(CommodityType.Polymers), 1 + random.Next2(Mathf.RoundToInt(30 + 10 * multiplier)));
            if (random.Percentage(10))
                yield return new Product(CreateArtifact(CommodityType.Artifacts), 1 + random.Next2(Mathf.RoundToInt(10 + 2 * multiplier)));

            for (var i = 0; i < random.Next(Mathf.Min(Mathf.RoundToInt(3 + 1 * quality), 10)); ++i)
                yield return new Product(RandomComponent(level, faction, random, true, false));

            var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.Container);
            if (resources.Count() > 0)
            {
                var resource = resources.RandomElement(random);
                foreach (var item in resource.ExplorationLoot)
                    if (random.Percentage(item.Chance))
                        yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
            }
        }

        public IEnumerable<IProduct> GetShipWreckLoot(Faction faction, int level, int seed)//残骸
        {
            var random = new System.Random(seed);
            var quality = _playerSkills.PlanetaryScanner - 1;
            var multiplier = quality + level / 100;

            yield return new Product(CreateArtifact(CommodityType.Scraps), 1 + random.Next2(Mathf.RoundToInt(100 + 50 * multiplier)));

            if (random.Percentage(30))
                yield return new Product(CreateArtifact(CommodityType.Alloys), 1 + random.Next2(Mathf.RoundToInt(30 + 10 * multiplier)));
            if (random.Percentage(30))
                yield return new Product(CreateArtifact(CommodityType.Polymers), 1 + random.Next2(Mathf.RoundToInt(30 + 10 * multiplier)));
            if (random.Percentage(20))
                yield return new Product(_factory.CreateFuelItem(), 1 + random.Next2(Mathf.RoundToInt(5 + 3 * multiplier)));

            if (random.Percentage(Mathf.RoundToInt(10+10 * quality)))
                yield return new Product(_factory.CreateResearchItem(faction));

            for (var i = 0; i < random.Next(Mathf.Min(Mathf.RoundToInt(5 + 3 * quality), 20)); ++i)
                yield return new Product(RandomComponent(level, faction, random, true, false));

            var resources = _database.ExplorationSettings.ExplorationResource.Where(item => item.Type == ExplorationType.ShipWreck);
            if (resources.Count() > 0)
            {
                var resource = resources.RandomElement(random);
                foreach (var item in resource.ExplorationLoot)
                    if (random.Percentage(item.Chance))
                        yield return new Product(_factory.CreateArtifactItem(item.QuestItem), 1 + random.Next2(item.MaxAmount));
            }
        }

        public IEnumerable<IProduct> GetStarBaseSpecialReward(Region region)//空间站
        {
            yield return new Product(_factory.CreateResearchItem(region.Faction), Mathf.FloorToInt(1f + region.BaseDefensePower / 3f));

            if (region.IsPirateBase)
            {
                var random = _random.CreateRandom(region.Id);

                yield return Price.Premium(Mathf.Min(15, 1 + region.MilitaryPower / 20)).GetProduct(_factory);
                foreach (var faction in _database.FactionList.Visible().RandomUniqueElements(5, random))
                    yield return new Product(_factory.CreateResearchItem(faction), Mathf.Min(15, 1 + region.MilitaryPower / 20));

                if (random.Percentage(30))
                {
                    var tech = _research.GetAvailableTechs(region.Faction).Where(item => item.Hidden || item.Price <= 10).RandomElement(random);
                    if (tech != null)
                        yield return new Product(_factory.CreateBlueprintItem(tech));
                }
            }
        }

        //public IEnumerable<IProduct> GetCommonPlanetReward(Faction faction, int level, System.Random random, float successChances)
        //{
        //    if (random.NextFloat() < successChances * successChances && random.Percentage(7))
        //        yield return Price.Premium(1).GetProduct(_factory);

        //    if (random.NextFloat() < successChances * successChances && random.Percentage(2))
        //    {
        //        var tech = _research.GetAvailableTechs(faction).Where(item => item.Hidden || item.Price <= 10).RandomElement(random);
        //        if (tech != null)
        //            yield return new Product(_factory.CreateBlueprintItem(tech));
        //    }

        //    if (System.DateTime.UtcNow.IsEaster())
        //        if (random.NextFloat() < successChances * successChances && random.Percentage(2))
        //            yield return new Product(_factory.CreateShipItem(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("fns3_mk2"))).Unlocked()));
        //}

        public IEnumerable<IProduct> GetSpaceWormLoot(int level, int seed)
        {
            var random = _random.CreateRandom(seed);
            yield return new Product(CreateArtifact(CommodityType.Artifacts), 1 + random.Next2(level));
            yield return Price.Premium(5 + random.Next2(level / 20)).GetProduct(_factory);

            if (random.Percentage(30))
            {
                var tech = _research.GetAvailableTechs(Faction.Undefined).Where(item => item.Price <= 15).RandomElement(random);
                if (tech != null)
                    yield return new Product(_factory.CreateBlueprintItem(tech));
            }
        }

        public IEnumerable<IProduct> GetRuinsRewards(int level, int seed)
        {
            var random = _random.CreateRandom(seed);

            yield return Price.Common(5 * Maths.Distance.Credits(level)).GetProduct(_factory);
            yield return new Product(_factory.CreateFuelItem(), random.Next(5,15));

            if (random.Next(3) == 0)
            {
                var itemLevel = Mathf.Max(6, level / 2);
                var companions = _database.SatelliteList.Where(item => item.Layout.CellCount <= itemLevel && item.SizeClass != SizeClass.Titan);
                foreach (var item in companions.Where(item => item.SizeClass != SizeClass.Titan).RandomUniqueElements(1, random))
                    yield return new Product(_factory.CreateSatelliteItem(item));
            }

            foreach (var item in RandomComponents(Maths.Distance.ComponentLevel(level) + 35, random.Next(1, 3), Faction.Undefined, random, false, false))
                yield return new Product(item);

            var quantity = random.Next(3);
            if (quantity > 0)
                yield return Price.Premium(quantity).GetProduct(_factory);

            yield return new Product(_factory.CreateResearchItem(_database.GalaxySettings.AbandonedStarbaseFaction));
        }

        public IEnumerable<IProduct> GetXmasRewards(int level, int seed)
        {
            var random = _random.CreateRandom(seed);

            yield return new Price(random.Range(level/5 + 15, level/5 + 30), Currency.Snowflakes).GetProduct(_factory);

            var items = _database.ComponentList.CommonAndRare().LevelLessOrEqual(level + 50)
                .RandomElements(random.Range(5, 10), random).Select(item =>
                    ComponentInfo.CreateRandomModification(item, random, ModificationQuality.P2, ModificationQuality.P3));

            if (random.Percentage(10))
                yield return new Product(_factory.CreateComponentItem(new ComponentInfo(_database.GetComponent(new ItemId<Component>(96))))); // xmas bomb
            if (random.Percentage(5) && level > 50)
                yield return new Product(_factory.CreateComponentItem(new ComponentInfo(_database.GetComponent(new ItemId<Component>(215))))); // drone bay
            if (random.Percentage(5) && level > 50)
                yield return new Product(_factory.CreateComponentItem(new ComponentInfo(_database.GetComponent(new ItemId<Component>(220))))); // drone bay
            if (random.Percentage(5) && level > 50)
                yield return new Product(_factory.CreateComponentItem(new ComponentInfo(_database.GetComponent(new ItemId<Component>(219))))); // drone bay
            if (random.Percentage(5) && level > 100)
                yield return new Product(_factory.CreateComponentItem(new ComponentInfo(_database.GetComponent(new ItemId<Component>(213))))); // holy cannon

            foreach (var item in items)
                yield return new Product(_factory.CreateComponentItem(item));
        }

        public IEnumerable<IProduct> GetDailyReward(int day, int level, int seed)
        {
            if (day <= 0)
                yield break;

            yield return new Price(Mathf.Min((int)(day * 100f * (1f + (day - 1) * 0.2f)), 20000), Currency.Credits).GetProduct(_factory);//30

            if (day % 2 == 0)
                yield return new Product(_factory.CreateFuelItem(), Mathf.Min(30, 5 + 5 * day / 2));//10
            if (day % 3 == 0)
                yield return new Product(_factory.CreateResearchItem(_database.FactionList.Visible().AtDistance(level).RandomElement(new System.Random(seed))), Mathf.Min(15, 3 + 2 * day / 3));//18
            if (day % 5 == 0)
                yield return Price.Premium(Mathf.Min(10, 4 + day / 5)).GetProduct(_factory);//30

            if (day % 7 == 0)
            {
                yield return new Price(Mathf.Min(50000, 5000 + 5000 * day / 7), Currency.Credits).GetProduct(_factory);//63
                yield return new Product(_factory.CreateFuelItem(), Mathf.Min(50, 20 + 10 * day / 7));//21
                var Researchitems = GetRandomCountFactionResearchItem(Mathf.Min(50, 10 + 10 * day / 7), Mathf.Min(_database.FactionList.Visible().AtDistance(level).Count() > 8 ? 8 : _database.FactionList.Visible().AtDistance(level).Count(), 3 * day / 7), level, seed);//28
                foreach (var item in Researchitems)
                    yield return item;
                yield return Price.Premium(Mathf.Min(30, 5 + 5 * day / 7)).GetProduct(_factory);//35
                var Componentitems = GetRandomDailyComponents(Mathf.Min(5, day / 7), level, seed, ComponentQuality.P4);//35
                foreach (var item in Componentitems)
                    yield return item;
            }
            if (day % 31 == 0)
            {
                yield return new Price(Mathf.Min(250000, 50000 + 50000 * day / 31), Currency.Credits).GetProduct(_factory);//124
                yield return new Product(_factory.CreateFuelItem(), Mathf.Min(150, 50 * day / 31));//93
                var Researchitems = GetRandomCountFactionResearchItem(Mathf.Min(150, 50 + 25 * day / 31), Mathf.Min(_database.FactionList.Visible().AtDistance(level).Count(), 5 + 5 * day / 31), level, seed);//124
                foreach (var item in Researchitems)
                    yield return item;
                yield return Price.Premium(Mathf.Min(100, 25 + 15 * day / 31)).GetProduct(_factory);//155
                var Componentitems = GetRandomDailyComponents(Mathf.Min(5, 1 + day / 31), level, seed, ComponentQuality.P5);//124
                foreach (var item in Componentitems)
                    yield return item;
            }

            if (day > 3)
            {
                var Componentitems = GetRandomDailyComponents(Mathf.Min(10, 2 + day / 5), level, seed, (ComponentQuality)Mathf.Min((int)ComponentQuality.P0 + day / 3, (int)ComponentQuality.P3));//40
                foreach (var item in Componentitems)
                    yield return item;
            }
        }
        public IEnumerable<IProduct> GetRandomCountFactionResearchItem(int maxcount,int maxfactioncount, int level, int seed)
        {
            int lastcount = maxcount;
            int factioncount = new System.Random(seed).Range(1, maxfactioncount);
            int[] count = new int[factioncount];
            for (int i = 0; i < factioncount - 1; i++)
            {
                count[i] = lastcount > factioncount - i ? new System.Random(seed).Range(1, lastcount * 2 / (factioncount - i)) : 1;
                lastcount -= count[i];
            }
            count[factioncount - 1] = lastcount;
            for (int i = 0; i < factioncount; i++)
                yield return new Product(_factory.CreateResearchItem(_database.FactionList.Visible().AtDistance(level).RandomElement(new System.Random(seed))), count[i]);
        }
        public IEnumerable<IProduct> GetRandomDailyComponents(int count,int level,int seed,ComponentQuality quality)
        {
            for (int i = 0; i < count; i++)
            {
                var component = ComponentInfo.CreateRandom(_database, level, Faction.Undefined, _random.CreateRandom(seed + i), false, false, quality);
                yield return new Product(_factory.CreateComponentItem(component));
            }
        }
        public IItemType GetRandomComponent(int distance, Faction faction, int seed, bool allowRare, bool blackmarket)
        {
            var random = _random.CreateRandom(seed);
            return RandomComponent(distance, faction, random, allowRare, blackmarket);
        }

        public IEnumerable<IItemType> GetRandomComponents(int distance, int count, Faction faction, int seed, bool allowRare, bool blackmarket, ComponentQuality maxQuality = ComponentQuality.P3)
        {
            var random = _random.CreateRandom(seed);
            return RandomComponents(distance, count, faction, random, allowRare, blackmarket, maxQuality);
        }

        public IItemType GetRandomFactionShip(int distance, Faction faction, int seed)
        {
            var random = _random.CreateRandom(seed);
            return RandomFactionShip(distance, faction, random);
        }

        public DamagedShipItem GetRandomDamagedShip(int distance, int seed)
        {
            var random = _random.CreateRandom(seed);

            var value = random.Next(distance);
            var ships = value > 20 ? _database.ShipBuildList.Available().NormalShips() : _database.ShipBuildList.Available().Common();
            var ship = ships.Playable().LimitLevel(value).OfFaction(Faction.Undefined, distance/2).RandomElements(1, random).First();

            return (DamagedShipItem)_factory.CreateDamagedShipItem(ship, random.Next());
        }

        private IItemType RandomFactionShip(int distance, Faction faction, System.Random random)
        {
            var ships = _database.ShipBuildList.Available().Common().Playable().OfFaction(faction).LimitLevel(distance).ToArray();
            return ships.Length > 0 ? _factory.CreateShipItem(new CommonShip(ships[random.Next(ships.Length)])) : null;
        }

        private IEnumerable<IItemType> RandomComponents(int distance, int count, Faction faction, System.Random random, bool allowRare, bool blackmarket, ComponentQuality maxQuality = ComponentQuality.P3)
        {
            for (var i = 0; i < count; ++i)
                yield return _factory.CreateComponentItem(ComponentInfo.CreateRandom(_database, distance, faction, random, allowRare, blackmarket, maxQuality));
        }

        private IItemType RandomComponent(int distance, Faction faction, System.Random random, bool allowRare, bool blackmarket, ComponentQuality maxQuality = ComponentQuality.P3)
        {
            return _factory.CreateComponentItem(ComponentInfo.CreateRandom(_database, distance, faction, random, allowRare, blackmarket, maxQuality));
        }

        private IItemType CreateArtifact(CommodityType commodityType)
        {
            var artifact = _database.GetQuestItem(new ItemId<QuestItem>((int)commodityType));
            return _factory.CreateArtifactItem(artifact);
        }
    }
}
