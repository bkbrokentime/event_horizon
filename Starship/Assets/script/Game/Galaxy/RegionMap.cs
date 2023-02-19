using System.Collections.Generic;
using Galaxy;
using GameDatabase;
using GameServices;
using GameServices.Random;
using Services.Messenger;
using Session;
using Utils;
using Zenject;

namespace GameModel
{
	public class RegionMap : GameServiceBase
    {
        [Inject]
	    public RegionMap(
            IRandom random, 
            ISessionData session,
            IDatabase database,
            BaseCapturedSignal.Trigger baseCapturedTrigger, 
            RegionFleetDefeatedSignal.Trigger regionFleetDefeatedTrigger, 
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
	    {
	        _random = random;
	        _session = session;
	        _database = database;
	        _baseCapturedTrigger = baseCapturedTrigger;
	        _regionFleetDefeatedTrigger = regionFleetDefeatedTrigger;
	    }

        public void GetAdjacentRegions(int starId, int distanceMin, int distanceMax, List<Region> regions)
        {
            if (_regionList.Count < _session.Regions.ExploredRegionCount)
            {
                foreach (var id in _session.Regions.Regions)
                {
					if (_regions.ContainsKey(id)) continue;
                    _regions.Add(id, _regionList.Count);
                    _regionList.Add(null);
                }
            }

            regions.Clear();
            foreach (var item in _regions)
            {
                var homestar = RegionLayout.GetRegionHomeStar(item.Key);
                //StarLayout.IdToPosition(starId, out var x, out var y);
				//GetRsndomSize(x, y, _random, out var size);
                //var homestar = RegionLayout.GetRandomSizeRegionHomeStar(item.Key, size);
                var distance = StarLayout.Distance(homestar, starId);
                if (distance < distanceMin || distance > distanceMax) continue;

				var region = _regionList[item.Value] ?? this[item.Key];
                if (region.Id == Region.UnoccupiedRegionId) continue;
				if (region.IsPirateBase) continue;

				regions.Add(region);
            }
        }

        public Region GetNearestRegion(int starId)
        {
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y,
                out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
                out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

            var distance1 = Distance(x, y, x2, y2);
            var distance2 = Distance(x, y, x2, y2);
            var distance3 = Distance(x, y, x3, y3);
            var distance4 = Distance(x, y, x4, y4);
            var distance5 = Distance(x, y, x5, y5);
            var distance6 = Distance(x, y, x6, y6);
            var distance7 = Distance(x, y, x7, y7);
            var distance8 = Distance(x, y, x8, y8);

            var minDistance = distance1;
            var regionId = RegionLayout.PositionToId(x1, y1);

            if (distance2 < minDistance)
            {
                minDistance = distance2;
                regionId = RegionLayout.PositionToId(x2, y2);
            }
            if (distance3 < minDistance)
            {
                minDistance = distance3;
                regionId = RegionLayout.PositionToId(x3, y3);
			}
            if (distance4 < minDistance)
            {
                minDistance = distance4;
                regionId = RegionLayout.PositionToId(x4, y4);
            }
            if (distance5 < minDistance)
            {
                minDistance = distance5;
                regionId = RegionLayout.PositionToId(x5, y5);
            }
            if (distance6 < minDistance)
            {
                minDistance = distance6;
                regionId = RegionLayout.PositionToId(x6, y6);
            }
            if (distance7 < minDistance)
            {
                minDistance = distance7;
                regionId = RegionLayout.PositionToId(x7, y7);
			}
			if (distance8 < minDistance)
            {
                minDistance = distance8;
                regionId = RegionLayout.PositionToId(x8, y8);
            }

            return this[regionId];
        }

		public Region GetStarRegion(int starId)
		{
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y, 
				out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
				out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

			var distance = int.MaxValue;
			var region = Region.Empty;

			BelongsToRegion(x, y, x1, y1, ref distance, ref region);
			BelongsToRegion(x, y, x2, y2, ref distance, ref region);
			BelongsToRegion(x, y, x3, y3, ref distance, ref region);
			BelongsToRegion(x, y, x4, y4, ref distance, ref region);
			BelongsToRegion(x, y, x5, y5, ref distance, ref region);
			BelongsToRegion(x, y, x6, y6, ref distance, ref region);
			BelongsToRegion(x, y, x7, y7, ref distance, ref region);
			BelongsToRegion(x, y, x8, y8, ref distance, ref region);

			return region;
		}

        public bool IsStarReachable(int starId, int maxDistance)
		{
            StarLayout.IdToPosition(starId, out var x, out var y);

            GetAdjacentRegions(x, y, 
	            out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
	        	out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

			if (Distance(x, y, x1, y1) <= maxDistance && this[RegionLayout.PositionToId(x1,y1)].IsCaptured)
				return true;
			if (Distance(x, y, x2, y2) <= maxDistance && this[RegionLayout.PositionToId(x2,y2)].IsCaptured)
				return true;
			if (Distance(x, y, x3, y3) <= maxDistance && this[RegionLayout.PositionToId(x3,y3)].IsCaptured)
				return true;
			if (Distance(x, y, x4, y4) <= maxDistance && this[RegionLayout.PositionToId(x4,y4)].IsCaptured)
				return true;
			if (Distance(x, y, x5, y5) <= maxDistance && this[RegionLayout.PositionToId(x5,y5)].IsCaptured)
				return true;
			if (Distance(x, y, x6, y6) <= maxDistance && this[RegionLayout.PositionToId(x6,y6)].IsCaptured)
				return true;
			if (Distance(x, y, x7, y7) <= maxDistance && this[RegionLayout.PositionToId(x7,y7)].IsCaptured)
				return true;
			if (Distance(x, y, x8, y8) <= maxDistance && this[RegionLayout.PositionToId(x8,y8)].IsCaptured)
				return true;

			return false;
		}

		public bool IsRegionReachable(int regionId)
		{
            RegionLayout.IdToPosition(regionId, out var x, out var y);

			if (this[RegionLayout.PositionToId(x,y+2)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x-1,y+1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x+1,y+1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x,y-2)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x-1,y-1)].IsCaptured)
				return true;
			if (this[RegionLayout.PositionToId(x+1,y-1)].IsCaptured)
				return true;

			return false;
		}

		public static bool IsHomeStar(int x, int y)
		{
			GetAdjacentRegions(x, y,
				out var x1, out var y1, out var x2, out var y2, out var x3, out var y3, out var x4, out var y4,
				out var x5, out var y5, out var x6, out var y6, out var x7, out var y7, out var x8, out var y8);

			if (x == x1 * RegionLayout.RegionFourthSize_X && y == y1 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x2 * RegionLayout.RegionFourthSize_X && y == y2 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x3 * RegionLayout.RegionFourthSize_X && y == y3 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x4 * RegionLayout.RegionFourthSize_X && y == y4 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x5 * RegionLayout.RegionFourthSize_X && y == y5 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x6 * RegionLayout.RegionFourthSize_X && y == y6 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x7 * RegionLayout.RegionFourthSize_X && y == y7 * RegionLayout.RegionFourthSize_Y)
				return true;
			if (x == x8 * RegionLayout.RegionFourthSize_X && y == y8 * RegionLayout.RegionFourthSize_Y)
				return true;

			return false;
		}

		public static bool IsRandomSizeHomeStar(IRandom random, int x, int y)
		{
            int size = 1;
            bool noregions = false;
            int[] xn = new int[8];
            int[] yn = new int[8];
            for (int i = 5; i > 0; i--)
            {
                GetRsndomSizeAdjacentRegions(x, y, random, i, out xn, out yn, out noregions);
                if (!noregions)
                {
                    size = i;
                    break;
                }
            }
            if (x == xn[0] * RegionLayout.RegionFourthSize_X && y == yn[0] * RegionLayout.RegionFourthSize_Y && !noregions)
				return true;

			return false;
        }

		public Region GetRandomSizeStarRegion(int starId)
		{
			StarLayout.IdToPosition(starId, out var x, out var y);
			int size = 1;
			bool noregions = false;
			int[] xn = new int[8];
			int[] yn = new int[8];
			for (int i = 5; i > 0; i--)
			{
				GetRsndomSizeAdjacentRegions(x, y, _random, i, out xn, out yn, out noregions);
				if (!noregions)
				{
					size = i;
					break;
				}
			}
			if (noregions)
				return Region.Empty;
			var distance = int.MaxValue;
			var region = Region.Empty;

			for (int i = 0; i < xn.Length; i++)
				BelongsToRandomSizeRegion(x, y, size, xn[i], yn[i], ref distance, ref region);

			return region;

		}

        public Region this[int id]
		{
			get
			{
				if (id < 0) return Region.Empty;
			    var unoccupied = id != Region.PlayerHomeRegionId && _random.RandomInt(id, 5) == 0;

			    if (unoccupied && _random.RandomInt(id + 1, 100) >= 25)
			        return Region.Empty;

                if (!_regions.TryGetValue(id, out var index))
                {
                    index = _regionList.Count;
                    _regions.Add(id, index);
					_regionList.Add(null);
				}

                var region = _regionList[index];
				if (region == null)
				{
					//StarLayout.IdToPosition(id, out var x, out var y);
					//GetRsndomSize(x, y, _random, out var size);
					//region = new Region(id, size, unoccupied, _session, _database, _baseCapturedTrigger, _regionFleetDefeatedTrigger);
					region = new Region(id, 5, _random, unoccupied, _session, _database, _baseCapturedTrigger, _regionFleetDefeatedTrigger);
					_regionList[index] = region;
				}

                return region;
            }
		}

	    protected override void OnSessionDataLoaded()
	    {
	        _regions.Clear();
	    }

	    protected override void OnSessionCreated()
        {
        }

        private static int Distance(int starX, int starY, int regionX, int regionY)
		{
			var x1 = regionX * RegionLayout.RegionFourthSize_X;
			var y1 = regionY * RegionLayout.RegionFourthSize_Y;
			return StarLayout.Distance(starX, starY, x1, y1);
		}

		private static void GetAdjacentRegions(
			int starX, int starY, 
	        out int x1, out int y1,
	        out int x2, out int y2,
			out int x3, out int y3,
			out int x4, out int y4,
			out int x5, out int y5,
			out int x6, out int y6,
			out int x7, out int y7,
			out int x8, out int y8)
		{
			var y0 = Frame(starY, RegionLayout.RegionFourthSize_Y);
			var x0 = Frame(starX, RegionLayout.RegionFourthSize_X);
			var odd = (x0 + y0) % 2 == 0;

            if (odd)
            {
                x1 = x0 + 0; y1 = y0 + 0;//0+0
                x2 = x0 + 0; y2 = y0 + 2;//0+4
                x3 = x0 + 1; y3 = y0 + 1;//3+2
                x4 = x0 + 1; y4 = y0 - 1;//3-1

                x5 = x0 + 0; y5 = y0 - 2;//0-4
                x6 = x0 + 0; y6 = y0 + 2;//0+4
                x7 = x0 + 2; y7 = y0 + 0;//6+0
                x8 = x0 + 1; y8 = y0 + 3;//3+6
            }
            else
            {
                x1 = x0 + 0; y1 = y0 + 1;//0+2
                x2 = x0 + 0; y2 = y0 - 1;//0-2
                x3 = x0 + 1; y3 = y0 + 0;//3+0
                x4 = x0 + 1; y4 = y0 + 2;//3+4

                x5 = x0 + 0; y5 = y0 + 3;//0+6
                x6 = x0 + 2; y6 = y0 + 1;//6+1
                x7 = x0 - 1; y7 = y0 + 0;//-3+0
                x8 = x0 + 1; y8 = y0 - 2;//3-4
            }
            /*
			if (odd)
			{
				x1 = x0 + 0; y1 = y0 + 0;//0+0=0
				x2 = x0 + 0; y2 = y0 + 2;//0+4=4
				x3 = x0 + 1; y3 = y0 + 1;//3+2=5
				x4 = x0 + 1; y4 = y0 - 1;//3-1=2

				x5 = x0 + 0; y5 = y0 - 2;//0-4=-4
				x6 = x0 + 0; y6 = y0 + 2;//0+4=4
				x7 = x0 + 2; y7 = y0 + 0;//6+0=6
				x8 = x0 + 1; y8 = y0 + 3;//3+6=9
			}
			else
			{
				x1 = x0 + 0; y1 = y0 + 1;//0+2=2
				x2 = x0 + 0; y2 = y0 - 1;//0-2=-2
				x3 = x0 + 1; y3 = y0 + 0;//3+0=3
				x4 = x0 + 1; y4 = y0 + 2;//3+4=7

				x5 = x0 + 0; y5 = y0 + 3;//0+6=6
				x6 = x0 + 2; y6 = y0 + 1;//6+1=7
				x7 = x0 - 1; y7 = y0 + 0;//-3+0=-3
				x8 = x0 + 1; y8 = y0 - 2;//3-4=-1//7
			}
			*/
        }
		private static void GetRsndomSizeAdjacentRegions(
			int starX, int starY, IRandom random,int size,
	        out int[] x, out int[] y,out bool noregions)
		{
			x = new int[8];
			y = new int[8];
			var x0 = Frame(starX, RegionLayout.RegionFourthSize_X * size);
			var y0 = Frame(starY, RegionLayout.RegionFourthSize_Y * size);
			//size = random.RandomInt(RegionLayout.PositionToId(x0 * RegionLayout.RegionFourthSize_X, y0 * RegionLayout.RegionFourthSize_Y), 1, 5);
			//GetRsndomSize(starX, starY, random, out size);
            var odd = (x0 + y0) % 2 == 0;

			if (odd)
			{
				x[0] = x0 + 0; y[0] = y0 + 0;//0+0
				x[1] = x0 + 0; y[1] = y0 + 2;//0+4
				x[2] = x0 + 1; y[2] = y0 + 1;//3+2
				x[3] = x0 + 1; y[3] = y0 - 1;//3-1

				x[4] = x0 + 0; y[4] = y0 - 2;//0-4
				x[5] = x0 + 0; y[5] = y0 + 2;//0+4
				x[6] = x0 + 2; y[6] = y0 + 0;//6+0
				x[7] = x0 + 1; y[7] = y0 + 3;//3+6
			}
			else
			{
				x[0] = x0 + 0; y[0] = y0 + 0;//0+2
				x[1] = x0 + 0; y[1] = y0 - 1;//0-2
				x[2] = x0 + 1; y[2] = y0 + 0;//3+0
				x[3] = x0 + 1; y[3] = y0 + 2;//3+4

				x[4] = x0 + 0; y[4] = y0 + 3;//0+6
				x[5] = x0 + 2; y[5] = y0 + 1;//6+1
				x[6] = x0 - 1; y[6] = y0 + 0;//-3+0
				x[7] = x0 + 1; y[7] = y0 - 2;//3-4
			}
			noregions = random.RandomInt(StarLayout.PositionToId(starX, starY), 10) < size ? false : true;
        }
        private static void GetRsndomSize(int starX, int starY, IRandom random, out int size)
		{
            size = 1;
            for (int i = 5; i > 0; i--)
            {
                GetRsndomSizeAdjacentRegions(starX, starY, random, i, out var xn, out var yn, out var noregions);
                if (!noregions)
                {
                    size = i;
                    break;
                }
            }
            //var x0 = Frame(starX, RegionLayout.RegionFourthSize_X);
            //var y0 = Frame(starY, RegionLayout.RegionFourthSize_Y);
            //size = random.RandomInt(RegionLayout.PositionToId(x0 * RegionLayout.RegionFourthSize_X, y0 * RegionLayout.RegionFourthSize_Y), 1, 5);
        }
        public void PublicGetRsndomSize(int starX, int starY, out int size)
		{
            size = 1;
            for (int i = 5; i > 0; i--)
            {
                GetRsndomSizeAdjacentRegions(starX, starY, _random, i, out var xn, out var yn, out var noregions);
                if (!noregions)
                {
                    size = i;
                    break;
                }
            }
            //var x0 = Frame(starX, RegionLayout.RegionFourthSize_X);
            //var y0 = Frame(starY, RegionLayout.RegionFourthSize_Y);
            //size = _random.RandomInt(RegionLayout.PositionToId(x0 * RegionLayout.RegionFourthSize_X, y0 * RegionLayout.RegionFourthSize_Y), 1, 5);
        }

        private bool BelongsToRegion(int x0, int y0, int x, int y, ref int minDistance, ref Region region)
		{
			var distance = Distance(x0, y0, x, y);

			if (distance < minDistance)
			{
				var value = this[RegionLayout.PositionToId(x,y)];
				if (value == null || distance > value.Size)
					return false;
				if (distance > 0 && distance == value.Size && _random.RandomInt(StarLayout.PositionToId(x0,y0)) % 2 == 0)
					return false;

				region = value;
				minDistance = distance;
				return true;
			}

			return false;
		}
		private bool BelongsToRandomSizeRegion(int x0, int y0, int size, int x, int y, ref int minDistance, ref Region region)
		{
			var distance = Distance(x0, y0, x, y);

			if (distance < minDistance)
			{
				var value = this[RegionLayout.PositionToId(x, y)];
				if (value == null || distance > value.Size * value._randomsize)
					return false;
				if (distance > 0 && distance == value.Size * value._randomsize && _random.RandomInt(StarLayout.PositionToId(x0, y0)) % 2 == 0)
					return false;

				region = value;
				minDistance = distance;
				return true;
			}

			return false;
		}
		
		private static int Frame(int x, int size)
		{
			return x >= 0 ? x / size : (x + 1)/size - 1;
		}

		private readonly Dictionary<int, int> _regions = new Dictionary<int, int>();
        private readonly List<Region> _regionList = new List<Region>();
	    private readonly IRandom _random;
	    private readonly ISessionData _session;
	    private readonly IDatabase _database;
	    private readonly BaseCapturedSignal.Trigger _baseCapturedTrigger;
	    private readonly RegionFleetDefeatedSignal.Trigger _regionFleetDefeatedTrigger;
	}

    public class BaseCapturedSignal : SmartWeakSignal<Region>
    {
        public class Trigger : TriggerBase {}
    }

    public class RegionFleetDefeatedSignal : SmartWeakSignal<Region>
    {
        public class Trigger : TriggerBase { }
    }
}
