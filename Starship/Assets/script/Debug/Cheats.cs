using Constructor;
using GameServices.Database;
using GameServices.Player;
using Maths;
using Session;
using Constructor.Ships;
using Economy.ItemType;
using GameServices.GameManager;
using GameServices.Gui;
using Services.Account;
using Services.Unity;
using UnityEngine;
using Zenject;
using Research = GameServices.Research.Research;
using Status = Services.Account.Status;
using System.Linq;
using Database.Legacy;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.LevelManager;
using GameDatabase.Extensions;
using Constructor.Ships.Modification;

public class Cheats
{
	[Inject] private readonly PlayerFleet _playerFleet;
	[Inject] private readonly PlayerInventory _playerInventory;
	[Inject] private readonly PlayerResources _playerResources;
	[Inject] private readonly PlayerSkills _playerSkills;
	[Inject] private readonly Research _research;
	[Inject] private readonly ITechnologies _technologies;
	[Inject] private readonly ISessionData _session;
	[Inject] private readonly IGameDataManager _gameDataManager;
	[Inject] private readonly SessionDataLoadedSignal.Trigger _dataLoadedTrigger;
	[Inject] private readonly SessionCreatedSignal.Trigger _sesionCreatedTrigger;
	[Inject] private readonly IAccount _account;
	[Inject] private readonly ICoroutineManager _coroutineManager;
	[Inject] private readonly GuiHelper _guiHelper;
	[Inject] private readonly ItemTypeFactory _itemTypeFactory;
	[Inject] private readonly MotherShip _motherShip;
	[Inject] private readonly StarMap _starMap;
	[Inject] private readonly IDatabase _database;
	[Inject] private readonly ILevelLoader _levelLoader;

	public bool TryExecuteCommand(string command, int hash)
	{
#if UNITY_EDITOR
		if (command == "123")
		{
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(49))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(19))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(80))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(78))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(65))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(85))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(99))) { Experience = Maths.Experience.FromLevel(50) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(25))) { Experience = Maths.Experience.FromLevel(50) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(28))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(7))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(16))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(10))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(31))) { Experience = Maths.Experience.FromLevel(100) });
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(22))) { Experience = Maths.Experience.FromLevel(100) });

			foreach (var item in _database.ComponentList.CommonAndRare())
				_playerInventory.Components.Add(new ComponentInfo(item), 25);

			//      _playerResources.Money += 200000;
			//      _playerResources.Stars += 50;

			foreach (var faction in _database.FactionList.Visible())
				_research.AddResearchPoints(faction, 5000);

			_playerSkills.Experience = GameModel.Skills.Experience.FromLevel(_playerSkills.Experience.Level + 100);
		}
		else if (command == "345")
		{
			_playerResources.Tokens += 1000;
		}
		else if (command == "000")
		{
			_playerResources.Fuel += 1000;
		}
		else if (command == "666")
		{
			var experience = Experience.FromLevel(200);
			for (var i = 0; i < 3; ++i)
			{
				_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader1"))) { Experience = experience });
				_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader2"))) { Experience = experience });
				_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader3"))) { Experience = experience });
				_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader4"))) { Experience = experience });
			}
		}
		else if (command == "667")
		{
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(265))));
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(266))));
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(267))));
			_playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(235))));
		}
		else if (command == "999")
		{
			var random = new System.Random();
			for (var i = 0; i < 100; ++i)
			{
				_playerInventory.Components.Add(ComponentInfo.CreateRandomModification(_database.GetComponent(LegacyComponentNames.GetId("Nanofiber")), random, ModificationQuality.N3, ModificationQuality.N3));
			}
		}
#endif

		if (command == "000")
		{
			if (_account.Status != Status.Connected)
				_guiHelper.ShowMessage("Not logged in");
			else
				_guiHelper.ShowMessage("DisplayName: " + _account.DisplayName + "\nId: " + _account.Id);
			return true;
		}

		var id = DebugCommands.Decode(command, hash);

		switch(id)
		{
			case 0:
				_playerResources.Money += 10000000;
                break;
			case 1:
                _playerResources.Stars += 10000000;
                break;
			case 2:
                _playerResources.Fuel += 10000000;
                break;
			case 3:
                _playerResources.Snowflakes += 10000000;
                break;
			case 4:
                _playerResources.Tokens += 10000000;
                break;
			case 5:
                foreach (var item in _database.QuestItemList.Where(item => item.Price > 0))
                    _playerResources.AddResource(item.Id, 1000000);
                break;
			case 6:
                foreach (var item in _database.QuestItemList.Where(item => item.Price == 0))
                    _playerResources.AddResource(item.Id, 1);
                break;
			case 10:
                foreach (var ship in _playerFleet.ActiveShipGroup.Ships)
                    ship.SetLevel(ship.Experience.Level + 100);
                break;
			case 11:
                foreach (var ship in _playerFleet.ActiveShipGroup.Ships)
                {
                    var count = ship.Model.Modifications.Count();
                    for (int i = 0; i < 5 - count; i++)
                        ship.Model.Modifications.Add(new EmptyModification());
                }
                break;
			case 12:
                foreach (var ship in _playerFleet.Ships)
                    ship.SetLevel(ship.Experience.Level + 100);
                break;
			case 13:
				foreach (var ship in _playerFleet.Ships)
				{
					var count = ship.Model.Modifications.Count();
					for (int i = 0; i < 5 - count; i++)
						ship.Model.Modifications.Add(new EmptyModification());
				}
                break;
			case 14:
				_playerFleet.ShipClear();
				break;
			case 20:
                foreach (var faction in _database.FactionList.Visible())
                    _research.AddResearchPoints(faction, 10000);
                break;
			case 21:
                _playerSkills.Experience = GameModel.Skills.Experience.FromLevel(_playerSkills.Experience.Level + 50);
                break;
			case 22:
                _playerSkills.Reset();
                _playerSkills.Experience = GameModel.Skills.Experience.FromLevel(10);
                break;
			case 30:
                foreach (var ship in _database.ShipBuildList.Playable())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 31:
                foreach (var ship in _database.ShipBuildList.Frigates())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 32:
                foreach (var ship in _database.ShipBuildList.Destroyers())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 33:
                foreach (var ship in _database.ShipBuildList.Cruisers())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 34:
                foreach (var ship in _database.ShipBuildList.Battleships())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 35:
                foreach (var ship in _database.ShipBuildList.Titans())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 36:
                foreach (var ship in _database.ShipBuildList.Dominates())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 37:
                foreach (var ship in _database.ShipBuildList.Flagships())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 38:
                foreach (var ship in _database.ShipBuildList.SuperFlagship())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 39:
                foreach (var ship in _database.ShipBuildList.NotAvailable())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 40:
                foreach (var ship in _database.ShipBuildList.NotPlayable())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 41:
                foreach (var ship in _database.ShipBuildList.Drones())
                    _playerFleet.Ships.Add(new CommonShip(ship));
                break;
			case 60:
                foreach (var item in _database.SatelliteList)
                    _playerInventory.Satellites.Add(item, 200);
                break;
			case 61:
                foreach (var item in _database.ComponentList)
                    _playerInventory.Components.Add(new ComponentInfo(item), 1000);
                break;
			case 62:
                foreach (var item in _database.ComponentList.Common())
                    _playerInventory.Components.Add(new ComponentInfo(item), 1000);
                break;
			case 70:
                break;
			case 71:
                break;
			case 80:
                break;
			case 81:
                break;
			case 90:
                break;
			case 91:
                break;
			case 100:
                _session.Quests.Reset();
                break;
            case 101:
                var center = _motherShip.CurrentStar.Position;
                var stars = _starMap.GetVisibleStars(center - Vector2.one * 200f, center + Vector2.one * 200f);
                foreach (var item in stars)
                    item.SetVisited();
                break;
            case 102:
                _gameDataManager.LoadGameFromLocalCopy();
                break;
            case 103:
                _session.Game.Regenerate();
                _session.StarMap.Reset();
                _session.Regions.Reset();
                _dataLoadedTrigger.Fire();
                _sesionCreatedTrigger.Fire();
                break;
            default:
				return false;
		}
		return true;
	}
}