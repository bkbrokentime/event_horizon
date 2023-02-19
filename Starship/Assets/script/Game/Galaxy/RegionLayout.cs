using UnityEngine;
using System;
using System.Collections.Generic;
using GameServices.Random;

namespace GameModel
{
	public static class RegionLayout
	{
		public static int PositionToId(int x, int y)
		{
			return 1 + StarLayout.PositionToId(x,y);
		}
		
		public static void IdToPosition(int id, out int x, out int y)
		{
			if (id <= 0)
			{
				x = 0;
				y = 0;
			}
			else
			{
				StarLayout.IdToPosition(id-1, out x, out y);
			}
		}
		
		public static int GetRegionHomeStar(int regionId)
		{
			if (regionId < 0)
				return 0;
			
			int x,y;
			IdToPosition(regionId, out x, out y);
			x *= RegionFourthSize_X;
			y *= RegionFourthSize_Y;
			
			return StarLayout.PositionToId(x,y);
		}
		public static int GetRandomSizeRegionHomeStar(int regionId, int size)
		{
			if (regionId < 0)
				return 0;
			
			int x,y;
			IdToPosition(regionId, out x, out y);
            x *= RegionFourthSize_X*size;
			y *= RegionFourthSize_Y*size;
			
			return StarLayout.PositionToId(x,y);
		}

		public const int RegionFourthSize = 3;
		public const int RegionFourthSize_X = 3 * RegionFourthSize;
		public const int RegionFourthSize_Y = 2 * RegionFourthSize;
		//public const int RegionFourthSize_DX = 3 * RegionFourthSize_X;
		//public const int RegionFourthSize_DY = 3 * RegionFourthSize_Y;
	}
}
