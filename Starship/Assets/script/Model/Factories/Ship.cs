using System;
using System.Linq;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;

namespace Model
{
	namespace Factories
	{
		public static class Ship
		{
			public static IShip Create(ShipBuild data, int distance, Random random, IDatabase database)
			{
				var shipLevel = Maths.Distance.ToShipLevel(distance);
				var delta = Math.Min(10, shipLevel/5);
				shipLevel += random.Next(delta+1) - delta/2;

				var minenhancementlevel = (EnhancementLevel)(distance / 200);
				var maxenhancementlevel = (EnhancementLevel)(distance / 20);
				var randomminenhancementlevel = data.MinEnhancementLevel == EnhancementLevel.Default ? minenhancementlevel : data.MinEnhancementLevel < minenhancementlevel ? minenhancementlevel : data.MinEnhancementLevel < maxenhancementlevel ? data.MinEnhancementLevel : maxenhancementlevel;
				var randommaxenhancementlevel = data.MaxEnhancementLevel == EnhancementLevel.Default ? maxenhancementlevel : data.MaxEnhancementLevel < minenhancementlevel ? data.MaxEnhancementLevel : data.MaxEnhancementLevel < maxenhancementlevel ? data.MaxEnhancementLevel : maxenhancementlevel;

				var enhancementlevel = randomminenhancementlevel <= randommaxenhancementlevel ? data.NoEnhancementLevel ? data.DefaultEnhancementLevel : (EnhancementLevel)UnityEngine.Random.Range((int)randomminenhancementlevel, (int)randommaxenhancementlevel) : randommaxenhancementlevel;
				//_extraEnhanceLevel = data.NoEnhancementLevel ? data.DefaultEnhancementLevel : (EnhancementLevel)UnityEngine.Random.Range((int)EnhancementLevel.Level1, (int)EnhancementLevel.Level100);
				var ship = new EnemyShip(data) { Experience = Maths.Experience.FromLevel(shipLevel), ExtraEnhanceLevel = enhancementlevel };

				if (data.Ship.ShipCategory == ShipCategory.Flagship || data.Ship.ShipCategory == ShipCategory.Starbase)
					return ship;
				
				var companionClass = Maths.Distance.CompanionClass(distance);
				
				var companions = database.SatelliteBuildList.LimitClass(companionClass).SuitableFor(data.Ship);

			    if (companions.Any())
			    {
			        if (random.Next(3) != 0)
			            ship.Satellite_Left_1 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Right_1 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Left_2 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Right_2 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Left_3 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Right_3 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Left_4 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Right_4 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Left_5 = new CommonSatellite(companions.RandomElement(random));
			        if (random.Next(3) != 0)
			            ship.Satellite_Right_5 = new CommonSatellite(companions.RandomElement(random));
			    }

			    return ship;
			}
		}
	}
}
