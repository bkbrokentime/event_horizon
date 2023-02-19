using System.Linq;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Maths;
using UnityEngine;

namespace Game.Exploration
{
    public class EnemyShipBuilder : IEnemyShipBuilder
    {
        public EnemyShipBuilder(ItemId<ShipBuild> id, IDatabase database, int level, int seed, bool randomizeColor = false, bool allowSatellites = true)
        {
            _database = database;
            _shipId = id.Value;
            _level = level;
            _seed = seed;
            _allowSatellites = allowSatellites;
            _randomizeColor = randomizeColor;
        }

        public Combat.Component.Ship.Ship Build(Combat.Factory.ShipFactory shipFactory, Combat.Factory.SpaceObjectFactory objectFactory, Vector2 position, float rotation)
        {
            var model = CreateShip();
            var spec = model.CreateBuilder().Build(_database.ShipSettings);
            var ship = shipFactory.CreateEnemyShip(spec, position, rotation, Maths.Distance.AiLevel(_level));
            return ship;
        }

        private IShip CreateShip()
        {
            var random = new System.Random(_seed);

            var build = _database.GetShipBuild(new ItemId<ShipBuild>(_shipId));
            var ship = new EnemyShip(build);

            var shipLevel = Maths.Distance.ToShipLevel(_level);
            shipLevel -= random.Next(shipLevel/3);
            ship.Experience = Maths.Experience.FromLevel(shipLevel);

            var minenhancementlevel = (EnhancementLevel)(_level / 50);
            var maxenhancementlevel = (EnhancementLevel)(_level / 10);
            var randomminenhancementlevel = build.MinEnhancementLevel == EnhancementLevel.Default ? minenhancementlevel : build.MinEnhancementLevel < minenhancementlevel ? minenhancementlevel : build.MinEnhancementLevel < maxenhancementlevel ? build.MinEnhancementLevel : maxenhancementlevel;
            var randommaxenhancementlevel = build.MaxEnhancementLevel == EnhancementLevel.Default ? maxenhancementlevel : build.MaxEnhancementLevel < minenhancementlevel ? build.MaxEnhancementLevel : build.MaxEnhancementLevel < maxenhancementlevel ? build.MaxEnhancementLevel : maxenhancementlevel;

            var enhancementlevel = randomminenhancementlevel <= randommaxenhancementlevel ? build.NoEnhancementLevel ? build.DefaultEnhancementLevel : (EnhancementLevel)UnityEngine.Random.Range((int)randomminenhancementlevel, (int)randommaxenhancementlevel) : randommaxenhancementlevel;
            ship.ExtraEnhanceLevel = enhancementlevel;


            var satelliteClass = Maths.Distance.MaxShipClass(_level);
            if (_allowSatellites && satelliteClass != DifficultyClass.Default && build.Ship.ShipCategory != ShipCategory.Drone)
            {
                var satellites = _database.SatelliteBuildList.LimitClass(satelliteClass).SuitableFor(build.Ship);
                if (satellites.Any())
                {
                    if (random.Next(3) != 0)
                        ship.Satellite_Left_1 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Right_1 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Left_2 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Right_2 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Left_3 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Right_3 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Left_4 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Right_4 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Left_5 = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.Satellite_Right_5 = new CommonSatellite(satellites.RandomElement(random));
                }
            }

            if (_randomizeColor)
            {
                ship.ColorScheme.Type = ShipColorScheme.SchemeType.Hsv;
                ship.ColorScheme.Hue = random.NextFloat();
            }

            return ship;
        }

        private readonly bool _randomizeColor;
        private readonly int _seed;
        private readonly int _shipId;
        private readonly int _level;
        private readonly bool _allowSatellites;
        private readonly IDatabase _database;
    }
}
