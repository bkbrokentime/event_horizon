using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Constructor.Ships;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Reources;
using Zenject;

namespace StarSystem
{
	public class FleetObjects : MonoBehaviour
	{
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public void CreateShips(IEnumerable<IShip> ships, System.Random random, IList<Planet> planets)
		{
			var count = 0;
			var orbit = 0;
			foreach (var ship in ships)
			{
				count++;
				if (count >= 3 * orbit + 3)
                {
                    count = 0;
                    orbit++;
                }
                CreateShip(ship.Model.OriginalShip, planets[random.Next(planets.Count)].transform.localPosition, orbit);
			}
		}

		public void CreateShips(IEnumerable<IShip> ships, Vector2 position)
		{
			var count = 0;
            var orbit = 0;
            foreach (var ship in ships.Where(item => item.Model.Category != ShipCategory.Starbase))
			{
                count++;
				if (count >= 3 * orbit + 3)
				{
					count = 0;
					orbit++;
				}
                CreateShip(ship.Model.OriginalShip, position, orbit);
			}
		}
		
		public Ship CreateShip(GameDatabase.DataModel.Ship data, Vector2 position, int orbit)
		{
			var ship = CreateShipObject();
			ship.Initialize(data, _resourceLocator, orbit);
			ship.transform.parent = transform;
			ship.transform.localPosition = position;
			ship.transform.localScale = Vector3.one;
			_ships.Add(ship);
			return ship;
		}

		public Ship CreateFlagship(GameDatabase.DataModel.Ship model, Vector2 position, float rotation)
		{
            var ship = CreateShipObject();
            ship.InitializeFlagship(model, _resourceLocator);
			ship.transform.parent = transform;
			ship.transform.localPosition = position;
			ship.Rotation = rotation;
			ship.transform.localScale = Vector3.one;
			_ships.Add(ship);
			return ship;
		}

		public void MoveTo(Vector2 position)
		{
			foreach (var ship in _ships)
				ship.MoveTo(position);
		}

		public IList<Ship> Ships { get { return _ships; } }

		public bool AreEnemiesNearby(Vector2 position, float distance)
		{
			foreach (var ship in _ships)
				if (Vector2.Distance(ship.transform.localPosition, position) <= distance)
					return true;

			return false;
		}

		public void Cleanup()
		{
			foreach (var ship in _ships)
				ship.gameObject.SetActive(false);

			_ships.Clear();
		    _shipEnumerator = null;
		}

		public void Escape(Vector2 position, IEnumerable<Planet> planets)
		{
			var distance = transform.parent.localScale.z;
			var availablePlanets = planets.Where(planet => Vector2.Distance(planet.transform.localPosition, position) > distance).ToList();
			if (availablePlanets.Count == 0)
				return;

			var random = new System.Random(GetHashCode());
			foreach (var ship in _ships)
				if (Vector2.Distance(ship.transform.localPosition, position) <= distance)
					ship.MoveTo(availablePlanets[random.Next(availablePlanets.Count)].transform.localPosition);
		}

		public bool IsActive { get { return gameObject.activeSelf; } }

	    private Ship CreateShipObject()
	    {
	        if (_shipEnumerator == null)
	            _shipEnumerator = transform.CreateChildren<Ship>().GetEnumerator();

	        _shipEnumerator.MoveNext();
	        return _shipEnumerator.Current;
	    }

	    private IEnumerator<Ship> _shipEnumerator;
        private List<Ship> _ships = new List<Ship>();
	}
}
