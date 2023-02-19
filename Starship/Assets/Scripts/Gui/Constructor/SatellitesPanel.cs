using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameServices.Player;
using Services.Localization;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Gui.Constructor
{
    public class SatellitesPanel : MonoBehaviour
    {
        [InjectOptional] private readonly PlayerInventory _playerInventory;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private InstallSatelliteEvent _installSatelliteEvent = new InstallSatelliteEvent();
        [SerializeField] private LayoutGroup _itemsLayoutGroup;
        [SerializeField] private GameObject _removeSatelliteItem;

        [Serializable]
        public class InstallSatelliteEvent : UnityEvent<ItemId<SatelliteBuild>, ItemId<Satellite>, CompanionLocation> {}

        public void Open(IShip selectedShip, bool isEditorMode)
        {
            _isEditorMode = isEditorMode;
            _selectedShip = selectedShip;
            gameObject.SetActive(true);
            UpdateItems();
        }

        public void InstallOnLeft_1(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Left);
            UpdateItems();
        }

        public void InstallOnRight_1(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Right);
            UpdateItems();
        }

        public void InstallOnLeft_2(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Left_2);
            UpdateItems();
        }

        public void InstallOnRight_2(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Right_2);
            UpdateItems();
        }

        public void InstallOnLeft_3(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Left_3);
            UpdateItems();
        }

        public void InstallOnRight_3(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Right_3);
            UpdateItems();
        }

        public void InstallOnLeft_4(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Left_4);
            UpdateItems();
        }

        public void InstallOnRight_4(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Right_4);
            UpdateItems();
        }

        public void InstallOnLeft_5(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Left_5);
            UpdateItems();
        }

        public void InstallOnRight_5(SatelliteListItem item)
        {
            _installSatelliteEvent.Invoke(item.BuildId, item.Id, CompanionLocation.Right_5);
            UpdateItems();
        }

        public void RemoveFromLeft_1()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty,  ItemId<Satellite>.Empty, CompanionLocation.Left);
            UpdateItems();
        }

        public void RemoveFromRight_1()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty, ItemId<Satellite>.Empty, CompanionLocation.Right);
            UpdateItems();
        }
        public void RemoveFromLeft_2()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty,  ItemId<Satellite>.Empty, CompanionLocation.Left_2);
            UpdateItems();
        }

        public void RemoveFromRight_2()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty, ItemId<Satellite>.Empty, CompanionLocation.Right_2);
            UpdateItems();
        }
        public void RemoveFromLeft_3()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty,  ItemId<Satellite>.Empty, CompanionLocation.Left_3);
            UpdateItems();
        }

        public void RemoveFromRight_3()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty, ItemId<Satellite>.Empty, CompanionLocation.Right_3);
            UpdateItems();
        }

        public void RemoveFromLeft_4()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty,  ItemId<Satellite>.Empty, CompanionLocation.Left_4);
            UpdateItems();
        }

        public void RemoveFromRight_4()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty, ItemId<Satellite>.Empty, CompanionLocation.Right_4);
            UpdateItems();
        }

        public void RemoveFromLeft_5()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty,  ItemId<Satellite>.Empty, CompanionLocation.Left_5);
            UpdateItems();
        }

        public void RemoveFromRight_5()
        {
            _installSatelliteEvent.Invoke(ItemId<SatelliteBuild>.Empty, ItemId<Satellite>.Empty, CompanionLocation.Right_5);
            UpdateItems();
        }

        private void UpdateItems()
        {
            _removeSatelliteItem.gameObject.SetActive(_selectedShip.Satellite_Left_1 != null
                || _selectedShip.Satellite_Right_1 != null
                || _selectedShip.Satellite_Left_2 != null
                || _selectedShip.Satellite_Right_2 != null
                || _selectedShip.Satellite_Left_3 != null
                || _selectedShip.Satellite_Right_3 != null
                || _selectedShip.Satellite_Left_4 != null
                || _selectedShip.Satellite_Right_4 != null
                || _selectedShip.Satellite_Left_5 != null
                || _selectedShip.Satellite_Right_5 != null);

            if (_isEditorMode)
            {
                var builds = _database.SatelliteBuildList;
                _itemsLayoutGroup.InitializeElements<SatelliteListItem, SatelliteBuild>(builds, UpdateSatellite);
            }
            else if (_playerInventory != null)
            {
                var satellites = _playerInventory.Satellites.Items.OrderBy(data => data.Key.Name);
                _itemsLayoutGroup.InitializeElements<SatelliteListItem, KeyValuePair<Satellite, ObscuredInt>>(satellites, UpdateSatellite);
            }
        }

        private void UpdateSatellite(SatelliteListItem item, SatelliteBuild build)
        {
            var canBeInstalled = _selectedShip.IsSuitableSatelliteSize(build.Satellite);

            item.Id = build.Satellite.Id;
            item.BuildId = build.Id;
            item.Icon.sprite = _resourceLocator.GetSprite(build.Satellite.ModelImage);
            item.Icon.color = canBeInstalled ? Color.white : Color.gray;
            item.NameText.text = build.Id.ToString();
            item.QuantityText.text = string.Empty;
            item.SizeText.text = build.Satellite.Layout.CellCount.ToString();
            item.WeaponText.text = build.Satellite.Barrels.Any() ? build.Satellite.Barrels.First().WeaponClass : "-";
            item.ButtonsPanel.gameObject.SetActive(false);
            item.ClickToInstallText.gameObject.SetActive(canBeInstalled);
            item.CantBeInstalledText.gameObject.SetActive(!canBeInstalled);
        }

        private void UpdateSatellite(SatelliteListItem item, KeyValuePair<Satellite, ObscuredInt> data)
        {
            var satellite = data.Key;
            var canBeInstalled = _selectedShip.IsSuitableSatelliteSize(satellite);

            item.Id = satellite.Id;
            item.BuildId = ItemId<SatelliteBuild>.Empty;
            item.Icon.sprite = _resourceLocator.GetSprite(satellite.ModelImage);
            item.Icon.color = canBeInstalled ? Color.white : Color.gray;
            item.NameText.text = _localization.GetString(satellite.Name);
            item.QuantityText.text = data.Value.ToString();
            item.SizeText.text = satellite.Layout.CellCount.ToString();
            item.WeaponText.text = satellite.Barrels.Any() ? satellite.Barrels.First().WeaponClass : "-";

            item.ButtonsPanel.gameObject.SetActive(false);
            item.ClickToInstallText.gameObject.SetActive(canBeInstalled);
            item.CantBeInstalledText.gameObject.SetActive(!canBeInstalled);
        }

        private bool _isEditorMode;
        private IShip _selectedShip;
    }
}
