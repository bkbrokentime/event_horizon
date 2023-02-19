using System;
using GameDatabase.Enums;

namespace Maths
{
	public static class Distance
	{
		public static int ToShipLevel(int distance)
		{
			var level = ToShipNoLimitedLevel(distance);
			return UnityEngine.Mathf.Min(level, Experience.MaxRank);
		}

		public static int ToShipNoLimitedLevel(int distance)
		{
			return UnityEngine.Mathf.Max(0, (distance - 20) / 2);
		}

		public static int ToShipSize(int distance)
		{
			return 20 + 2*distance;
		}

		public static int FromShipLevel(int level)
		{
			return 2*(level + 5);
		}

		public static int FleetSize(int distance, System.Random random)
		{
			var max = 8 + random.Next(200)*random.Next(200)/2000;
			return 1 + System.Math.Min(max, distance/20);
		}
		
		public static int CombatTime(int distance) { return Math.Max(30, 120 - distance / 5); }
		public static int AiLevel(int distance) { return distance*2; }
		public static int ComponentLevel(int distance) { return Math.Max(distance, 1); }
        public static int Credits(int distance) { return 100 + Math.Max(distance, 1) * 2; }
        public static DifficultyClass MaxShipClass(int distance) { return (DifficultyClass)(distance/50); }
		public static DifficultyClass MinShipClass(int distance) { return distance < 100 ? DifficultyClass.Default : DifficultyClass.Class1; }
		public static DifficultyClass CompanionClass(int distance) { return (DifficultyClass)((distance+10)/20); }
	}
}