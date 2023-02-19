using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameServices.GameManager;
using Session;
using Session.Content;
using Zenject;
using Constructor.Ships;
using Database.Legacy;
using Diagnostics;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine.Assertions;
using Utils;
using UnityEngine;
using DebugLogSetting;

namespace GameServices.Player
{
    public sealed class PlayerFleet : GameServiceBase
    {
        [Inject]
        public PlayerFleet(
            ISessionData session, 
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal, 
            SessionAboutToSaveSignal sessionAboutToSaveSignal,
            IDebugManager debugManager,
            PlayerSkillsResetSignal playerSkillsResetSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
            _debugManager = debugManager;

            _sessionAboutToSaveSignal = sessionAboutToSaveSignal;
            _sessionAboutToSaveSignal.Event += OnSessionAboutToSave;
            _playerSkillsResetSignal = playerSkillsResetSignal;
            _playerSkillsResetSignal.Event += OnPlayerSkillsReset;

            _ships.ItemAddedEvent += OnShipAdded;
            _ships.ItemRemovedEvent += OnShipRemoved;
            _ships.EntireCollectionChangedEvent += OnShipCollectionChanged;
        }

        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly PlayerInventory _inventory;
        [Inject] private readonly IDatabase _database;

        public ShipSquad ActiveShipGroup => _activeShips;

        public IShip ExplorationShip
        {
            get => _explorationShip;
            set
            {
                Assert.IsNotNull(value);

                switch (_playerSkills.LargeShipExploration)
                {
                    case 1:
                        Assert.IsTrue(value.Model.SizeClass == SizeClass.Frigate || value.Model.SizeClass == SizeClass.Destroyer);
                        break;
                    case 2:
                        Assert.IsTrue(value.Model.SizeClass == SizeClass.Frigate || value.Model.SizeClass == SizeClass.Destroyer || value.Model.SizeClass == SizeClass.Cruiser);
                        break;
                    case 3:
                        Assert.IsTrue(value.Model.SizeClass == SizeClass.Frigate || value.Model.SizeClass == SizeClass.Destroyer || value.Model.SizeClass == SizeClass.Cruiser || value.Model.SizeClass == SizeClass.Battleship);
                        break;
                    default:
                        Assert.IsTrue(value.Model.SizeClass == SizeClass.Frigate);
                        break;
                }

                Assert.IsTrue(_ships.Contains(value));
                _explorationShip = value;
                DataChanged = true;
            }
        }
        public bool CanExploration(IShip ship)
        {
            switch (_playerSkills.LargeShipExploration)
            {
                case 1:
                    return ship.Model.SizeClass == SizeClass.Frigate || ship.Model.SizeClass == SizeClass.Destroyer;
                case 2:
                    return ship.Model.SizeClass == SizeClass.Frigate || ship.Model.SizeClass == SizeClass.Destroyer || ship.Model.SizeClass == SizeClass.Cruiser;
                case 3:
                    return ship.Model.SizeClass == SizeClass.Frigate || ship.Model.SizeClass == SizeClass.Destroyer || ship.Model.SizeClass == SizeClass.Cruiser || ship.Model.SizeClass == SizeClass.Battleship;
                default:
                    return ship.Model.SizeClass == SizeClass.Frigate;
            }
        }

        public IEnumerable<IShip> GetAllHangarShips()
        {
            return _activeShips.Ships;
        }

        public ObservableCollection<IShip> Ships => _ships;

        private void OnShipCollectionChanged()
        {
            DataChanged = true;
        }

        private void OnShipAdded(IShip ship)
        {
            DataChanged = true;
            _session.Statistics.UnlockShip(ship.Id);
        }

        private void OnShipRemoved(IShip ship)
        {
            DataChanged = true;
            _activeShips.Remove(ship);
            //foreach (var group in _shipGroups)
            //    group.Remove(ship);
        }

        public float Power { get { return ActiveShipGroup.Ships.Sum(ship => Maths.Threat.GetShipPower(ship)); } }

        protected override void OnSessionDataLoaded()
        {
            Load();
        }

        protected override void OnSessionCreated()
        {
            var components = new List<IntegratedComponent>();
            foreach (var ship in _ships)
            {
                if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                    UnityEngine.Debug.Log("CheckShipComponents in Ship Layout");

                var debug = _debugManager.CreateLog(ship.Name);
                CheckShipComponents(ship.Model.Layout, ship.Model.Barrels, ship.Components, components, debug);
                ship.Components.Assign(components);

                if (ship.Model.SecondLayout.Data != "0")
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Ship SecondLayout : "+ ship.Model.SecondLayout.Data);
                    CheckShipComponents(ship.Model.SecondLayout, null, ship.Components, components, debug);
                    ship.Components.Assign(components);
                }

                if (ship.Satellite_Left_1 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_1 Layout");
                    CheckShipComponents(ship.Satellite_Left_1.Information.Layout, ship.Satellite_Left_1.Information.Barrels, ship.Satellite_Left_1.Components, components, debug);
                    ship.Satellite_Left_1.Components.Assign(components);
                    if (ship.Satellite_Left_1.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_1 SecondLayout");
                        CheckShipComponents(ship.Satellite_Left_1.Information.SecondLayout, null, ship.Satellite_Left_1.Components, components, debug);
                        ship.Satellite_Left_1.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Right_1 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_1 Layout");
                    CheckShipComponents(ship.Satellite_Right_1.Information.Layout, ship.Satellite_Right_1.Information.Barrels, ship.Satellite_Right_1.Components, components, debug);
                    ship.Satellite_Right_1.Components.Assign(components);
                    if (ship.Satellite_Right_1.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_1 SecondLayout");
                        CheckShipComponents(ship.Satellite_Right_1.Information.SecondLayout, null, ship.Satellite_Right_1.Components, components, debug);
                        ship.Satellite_Right_1.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Left_2 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_2 Layout");
                    CheckShipComponents(ship.Satellite_Left_2.Information.Layout, ship.Satellite_Left_2.Information.Barrels, ship.Satellite_Left_2.Components, components, debug);
                    ship.Satellite_Left_2.Components.Assign(components);
                    if (ship.Satellite_Left_2.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_2 SecondLayout");
                        CheckShipComponents(ship.Satellite_Left_2.Information.SecondLayout, null, ship.Satellite_Left_2.Components, components, debug);
                        ship.Satellite_Left_2.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Right_2 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_2 Layout");
                    CheckShipComponents(ship.Satellite_Right_2.Information.Layout, ship.Satellite_Right_2.Information.Barrels, ship.Satellite_Right_2.Components, components, debug);
                    ship.Satellite_Right_2.Components.Assign(components);
                    if (ship.Satellite_Right_2.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_2 SecondLayout");
                        CheckShipComponents(ship.Satellite_Right_2.Information.SecondLayout, null, ship.Satellite_Right_2.Components, components, debug);
                        ship.Satellite_Right_2.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Left_3 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_3 Layout");
                    CheckShipComponents(ship.Satellite_Left_3.Information.Layout, ship.Satellite_Left_3.Information.Barrels, ship.Satellite_Left_3.Components, components, debug);
                    ship.Satellite_Left_3.Components.Assign(components);
                    if (ship.Satellite_Left_3.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_3 SecondLayout");
                        CheckShipComponents(ship.Satellite_Left_3.Information.SecondLayout, null, ship.Satellite_Left_3.Components, components, debug);
                        ship.Satellite_Left_3.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Right_3 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_3 Layout");
                    CheckShipComponents(ship.Satellite_Right_3.Information.Layout, ship.Satellite_Right_3.Information.Barrels, ship.Satellite_Right_3.Components, components, debug);
                    ship.Satellite_Right_3.Components.Assign(components);
                    if (ship.Satellite_Right_3.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_3 SecondLayout");
                        CheckShipComponents(ship.Satellite_Right_3.Information.SecondLayout, null, ship.Satellite_Right_3.Components, components, debug);
                        ship.Satellite_Right_3.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Left_4 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_4 Layout");
                    CheckShipComponents(ship.Satellite_Left_4.Information.Layout, ship.Satellite_Left_4.Information.Barrels, ship.Satellite_Left_4.Components, components, debug);
                    ship.Satellite_Left_4.Components.Assign(components);
                    if (ship.Satellite_Left_4.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_4 SecondLayout");
                        CheckShipComponents(ship.Satellite_Left_4.Information.SecondLayout, null, ship.Satellite_Left_4.Components, components, debug);
                        ship.Satellite_Left_4.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Right_4 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_4 Layout");
                    CheckShipComponents(ship.Satellite_Right_4.Information.Layout, ship.Satellite_Right_4.Information.Barrels, ship.Satellite_Right_4.Components, components, debug);
                    ship.Satellite_Right_4.Components.Assign(components);
                    if (ship.Satellite_Right_4.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_4 SecondLayout");
                        CheckShipComponents(ship.Satellite_Right_4.Information.SecondLayout, null, ship.Satellite_Right_4.Components, components, debug);
                        ship.Satellite_Right_4.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Left_5 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_5 Layout");
                    CheckShipComponents(ship.Satellite_Left_5.Information.Layout, ship.Satellite_Left_5.Information.Barrels, ship.Satellite_Left_5.Components, components, debug);
                    ship.Satellite_Left_5.Components.Assign(components);
                    if (ship.Satellite_Left_5.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Left_5 SecondLayout");
                        CheckShipComponents(ship.Satellite_Left_5.Information.SecondLayout, null, ship.Satellite_Left_5.Components, components, debug);
                        ship.Satellite_Left_5.Components.Assign(components);
                    }
                }

                if (ship.Satellite_Right_5 != null)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_5 Layout");
                    CheckShipComponents(ship.Satellite_Right_5.Information.Layout, ship.Satellite_Right_5.Information.Barrels, ship.Satellite_Right_5.Components, components, debug);
                    ship.Satellite_Right_5.Components.Assign(components);
                    if (ship.Satellite_Right_5.Information.SecondLayout.Data != "0")
                    {
                        if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                            UnityEngine.Debug.Log("CheckShipComponents in Satellite_Right_5 SecondLayout");
                        CheckShipComponents(ship.Satellite_Right_5.Information.SecondLayout, null, ship.Satellite_Right_5.Components, components, debug);
                        ship.Satellite_Right_5.Components.Assign(components);
                    }
                }
            }

            _activeShips.CheckIfValid(_playerSkills, true);
        }

        private void CheckShipComponents(Layout layoutData, IEnumerable<Barrel> barrels, IEnumerable<IntegratedComponent> components, IList<IntegratedComponent> validComponents, IDebugLog debugLog)
        {
            var layout = new Constructor.ShipLayout(layoutData, barrels, Enumerable.Empty<Constructor.IntegratedComponent>(), debugLog);
            validComponents.Clear();
            var random = new System.Random(_session.Game.Seed);

            if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                UnityEngine.Debug.Log("CheckShipComponents : " + components.Count());
            foreach (var item in components)
            {
                IntegratedComponent component = item;
                if (!component.Info)
                    continue;

                if (!item.Info.IsValidModification)
                {
                    component = new IntegratedComponent(ComponentInfo.CreateNoModification(item.Info.Data), item.Layout, item.X, item.Y,
                        item.BarrelId, item.KeyBinding, item.Behaviour, item.Locked);
                }

                var id = layout.InstallComponent(component.Info,component.Layout, component.X, component.Y);
                if (id >= 0)
                {
                    if (ComponentDebugLogSetting.ComponentConstructDebugLog)
                        UnityEngine.Debug.Log("validComponents : " + id);
                    validComponents.Add(component);
                }
                else
                    _inventory.Components.Add(component.Info);
            }
        }

        private void OnPlayerSkillsReset()
        {
            _activeShips.CheckIfValid(_playerSkills, true);
        }

        private void Load()
        {
            Clear();

            var ships = new List<IShip>();
            foreach (var item in _session.Fleet.Ships)
            {
                try
                {
                    ships.Add(ShipExtensions.FromShipData(_database, item));
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    ships.Add(null);
                    UnityEngine.Debug.Log("Unknown ship: " + item.Id);
                }
            }

            if (ships.Count == 0)
            {
                CreateDefault();
                return;
            }

            _ships.Assign(ships.Where(ship => ship != null));

            _activeShips.Clear();
            foreach (var item in _session.Fleet.Hangar)
            {
                UnityEngine.Debug.Log("group:" + item.Index + " ship:" + item.ShipId);
                _activeShips[item.Index] = ships[item.ShipId];
            }

            _explorationShip = _session.Fleet.ExplorationShipId >= 0 ? ships[_session.Fleet.ExplorationShipId] : null;

            UnityEngine.Debug.Log("PlayerFleet.Load: " + _ships.Count + " ships");

            DataChanged = false;
        }

        private void SaveShips()
        {
            UnityEngine.Debug.Log("PlayerFleet.SaveShips - " + _ships.Count);

            _session.Fleet.Ships.Clear();

            var shipIndices = new Dictionary<IShip, int>();
            var index = 0;
            foreach (var ship in _ships)
            {
                var info = ship.ToShipData();
                _session.Fleet.Ships.Add(info);
                shipIndices.Add(ship, index++);
            }

            _session.Fleet.Hangar.Clear();
            for (var j = 0; j < _activeShips.Count; ++j)
            {
                var ship = _activeShips[j];
                if (ship == null)
                    continue;

                if (!shipIndices.TryGetValue(ship, out var id))
                    continue;

                _session.Fleet.Hangar.Add(new FleetData.HangarSlotInfo { ShipId = id, Index = j });
            }

            if (_explorationShip != null && shipIndices.TryGetValue(_explorationShip, out var explorationShipId))
                _session.Fleet.ExplorationShipId = explorationShipId;
            else
                _session.Fleet.ExplorationShipId = -1;

            DataChanged = false;
        }

        private void CreateDefault()
        {
            Clear();

            var startingBuilds = _database.GalaxySettings.StartingShipBuilds;
            if (startingBuilds.Count == 0)
            {
                startingBuilds += new[] {
                    _database.GetShipBuild(new ItemId<ShipBuild>(39)),
                    _database.GetShipBuild(new ItemId<ShipBuild>(45)),
                    _database.GetShipBuild(new ItemId<ShipBuild>(51)),
                };
            }

            foreach (var build in startingBuilds)
            {
                var ship = new CommonShip(build);
                _ships.Add(ship);
                ActiveShipGroup.Add(ship);
            }

            if (_session.Purchases.SupporterPack)
            {
                _ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("fns2"))));
                ActiveShipGroup.Add(_ships.Last());
            }

            _explorationShip = _ships.FirstOrDefault(ship => ship.Model.SizeClass == SizeClass.Frigate);

            foreach (var ship in _ships)
                foreach (var item in ship.Components)
                    item.Locked = false;
        }

        private void OnSessionAboutToSave()
        {
            UnityEngine.Debug.Log("PlayerFleet.OnSessionAboutToSave");

            if (DataChanged)
                SaveShips();
        }

        private void Clear()
        {
            _ships.Clear();
            _activeShips.Clear();
        }

        public void ShipClear()
        {
            _ships.Clear();
        }
        public bool RemoveShip(IShip ship) { return _ships.Remove(ship); }

        private bool DataChanged
        {
            get
            {
                return _dataChanged || _activeShips.IsChanged || _ships.Any(ship => ship.DataChanged);
            }
            set
            {
                _dataChanged = value;
                if (_dataChanged)
                    return;

                _activeShips.IsChanged = false;

                foreach (var ship in _ships)
                    ship.DataChanged = false;
            }
        }

        private bool _dataChanged;
        private IShip _explorationShip;
        private readonly ShipSquad _activeShips = new ShipSquad();
        private readonly ObservableCollection<IShip> _ships = new ObservableCollection<IShip>();

        private readonly ISessionData _session;
        private readonly IDebugManager _debugManager;
        private readonly SessionAboutToSaveSignal _sessionAboutToSaveSignal;
        private readonly PlayerSkillsResetSignal _playerSkillsResetSignal;
    }
}
