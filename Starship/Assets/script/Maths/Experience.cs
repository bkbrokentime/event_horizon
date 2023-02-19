using System;
using Constructor.Ships;
using UnityEngine;

namespace Maths
{
	public struct Experience
	{
		public Experience(long value, bool limitMaxLevel = true)
		{
			if (value < 0)
				value = 0;
			else if (limitMaxLevel && value > MaxExperience)
				value = MaxExperience;

			_value = value;
		}

		public int Level
		{
			get
			{
				var exp = Value;
				var level = (int)Math.Pow(exp / 100, 1.0 / 3.0);

				if (LevelToExp(level + 1) <= exp)
					return level + 1;

				return level;
			}
		}

		public float PowerMultiplier { get { return LevelToPowerMultiplier(Level); } }

		public long ExpFromLastLevel { get { return Value - LevelToExp(Level); } }

		public long NextLevelCost
		{
			get
			{
				var level = Level;
				return LevelToExp(level + 1) - LevelToExp(level);
			}
		}

		public static Experience FromLevel(int level)
		{
			return new Experience(LevelToExp(level));
		}

		public static float LevelToPowerMultiplier(int level)
		{
			//float PM = 1;
			//for (int i = 0; i <= level; ++i)
			//{
			//    PM += ((i-1) / 10 + 1) * 0.10f;
			//}
			//return PM;
			//return (float)Math.Pow(10, (0.01*level));
			//return (float)Math.Pow(5, (0.005*level));
			//return 1+level/100*10;
			//return (level * 0.01f) * (level * 0.01f) + 1;
			float x = 1;
			for (int i = 0; i < level; ++i)
			{
				x *= Mathf.Pow(20.0f * 500 / (i + 500), 0.01f);
			}
			return x;
		}

		public static long TotalExpForShip(IShip ship)
		{
			return (1L + (long)Math.Pow(ship.Experience.Level + 1, 1.2)) * (long)Math.Pow(ship.Model.Layout.CellCount + ship.Model.SecondLayout.CellCount, 1.2) * (long)(1 + (int)ship.ExtraThreatLevel * 0.5f) * (long)(1 + (int)ship.ExtraEnhanceLevel * 0.05f);
		}

		public static implicit operator Experience(long value)
		{
			return new Experience(value);
		}

		public static implicit operator long(Experience data)
		{
			return data.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		private long Value { get { return _value; } }

		public static long LevelToExp(int level) { return 100L * level * level * level; }
		public static int ExpToLevel(long Exp) { return (int)Math.Pow(Exp / 100, 1f / 3f); }

		private readonly ObscuredLong _value;

		public const int MaxRank = 500;
		public const long MaxExperience = 100L * MaxRank * MaxRank * MaxRank;
		public const int MaxPlayerRank = 120;
		public const int MaxPlayerRank2 = 300;

		public const int PlayerRank_80 = 80;
		public const int PlayerRank_100 = 100;
		public const int PlayerRank_120 = 120;

		public const int PlayerRank_30 = 30;

		public const long MaxPlayerExperience = 100L * MaxPlayerRank * MaxPlayerRank * MaxPlayerRank;
		public const long MaxPlayerExperience2 = 100L * MaxPlayerRank2 * MaxPlayerRank2 * MaxPlayerRank2;

		public const long PlayerExperience_80 = 100L * PlayerRank_80 * PlayerRank_80 * PlayerRank_80;
		public const long PlayerExperience_100 = 100L * PlayerRank_100 * PlayerRank_100 * PlayerRank_100;
		public const long PlayerExperience_120 = 100L * PlayerRank_120 * PlayerRank_120 * PlayerRank_120;

		public const long PlayerExperience_30 = 100L * PlayerRank_30 * PlayerRank_30 * PlayerRank_30;
	}
}
