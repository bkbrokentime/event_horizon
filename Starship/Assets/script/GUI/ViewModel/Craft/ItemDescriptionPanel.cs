﻿using System.Collections.Generic;
using System.Linq;
using Constructor;
using Economy.ItemType;
using Economy.Products;
using UnityEngine;
using UnityEngine.UI;
using GameModel;
using Services.Localization;
using Services.ObjectPool;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using Services.Reources;
using Zenject;

namespace ViewModel.Craft
{
    public class ItemDescriptionPanel : MonoBehaviour
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly GameObjectFactory _factory;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly IDatabase _database;

        [SerializeField] Image _icon;
        [SerializeField] Text _name;
        [SerializeField] Text _description;
        [SerializeField] Text _modification;
        [SerializeField] LayoutGroup _stats;
        [SerializeField] LayoutGroup _weaponSlots;

        [SerializeField] Sprite _emptyIcon;

        public void Initialize(IProduct item)
        {
            if (item == null)
            {
                CreateEmpty();
            }
            else if (item.Type is ComponentItem)
            {
                CreateComponent(((ComponentItem)item.Type).Component);
            }
            else if (item.Type is ShipItem)
            {
                CreateShip(((ShipItem)item.Type).Ship);
            }
            else if (item.Type is SatelliteItem)
            {
                CreateSatellite(((SatelliteItem)item.Type).Satellite);
            }
            else
            {
                CreateDefault(item.Type);
            }
        }

        private void CreateShip(IShip ship)
        {
            _icon.sprite = _resourceLocator.GetSprite(ship.Model.ModelImage);
            _icon.color = Color.white;
            _name.text = _localization.GetString(ship.Name);
            _name.color = ColorTable.QualityColor(ship.Model.Quality());
            _description.gameObject.SetActive(false);

            _modification.gameObject.SetActive(ship.Model.Modifications.Any());
            _modification.text = string.Join("\n", ship.Model.Modifications.Select(item => item.GetDescription(_localization)).ToArray());

            _stats.gameObject.SetActive(true);
            _stats.InitializeElements<TextFieldViewModel, KeyValuePair<string, string>>(GetShipDescription(ship), UpdateTextField);
            _weaponSlots.gameObject.SetActive(true);/*******************************************/
            _weaponSlots.InitializeElements<BlockViewModel, Barrel>(ship.Model.Barrels, UpdateWeaponSlot);
        }

        private void CreateSatellite(Satellite satellite)
        {
            _icon.sprite = _resourceLocator.GetSprite(satellite.ModelImage);
            _icon.color = Color.white;
            _name.text = _localization.GetString(satellite.Name);
            _name.color = ColorTable.QualityColor(ItemQuality.Common);
            _description.gameObject.SetActive(false);

            _modification.gameObject.SetActive(false);

            _stats.gameObject.SetActive(true);
            _stats.InitializeElements<TextFieldViewModel, KeyValuePair<string, string>>(GetSatelliteDescription(satellite), UpdateTextField);
            //_weaponSlots.gameObject.SetActive(satellite.Barrels.Any());/*******************************************/
            _weaponSlots.gameObject.SetActive(true);/*******************************************/
            _weaponSlots.InitializeElements<BlockViewModel, Barrel>(satellite.Barrels, UpdateWeaponSlot);
        }

        private void CreateComponent(ComponentInfo info)
        {
            _icon.sprite = _resourceLocator.GetSprite(info.Data.Icon);
            _icon.color = info.Data.Color;
            _name.text = _localization.GetString(info.Data.Name);
            _name.color = ColorTable.QualityColor(info.ItemQuality);
            _description.gameObject.SetActive(false);

            var component = info.CreateComponent(100);

            var modification = component.Modification ?? Constructor.Modification.EmptyModification.Instance;
            _modification.gameObject.SetActive(!string.IsNullOrEmpty(_modification.text = modification.GetDescription(_localization)));
            _modification.color = ColorTable.QualityColor(info.ItemQuality);

            _stats.gameObject.SetActive(true);
            _stats.InitializeElements<TextFieldViewModel, KeyValuePair<string, string>>(ComponentViewModel.GetDescription(component, _localization, _database), UpdateTextField, _factory);
            _weaponSlots.gameObject.SetActive(false);
        }

        private void CreateEmpty()
        {
            _icon.sprite = _emptyIcon;
            _icon.color = Color.white;
            _name.text = string.Empty;
            _description.gameObject.SetActive(false);
            _stats.gameObject.SetActive(false);
            _weaponSlots.gameObject.SetActive(false);
            _modification.gameObject.SetActive(false);
        }

        private void CreateDefault(IItemType item)
        {
            _icon.sprite = item.GetIcon(_resourceLocator);
            _icon.color = item.Color;
            _name.text = item.Name;
            _name.color = ColorTable.QualityColor(item.Quality);

            var description = item.Description;
            _description.gameObject.SetActive(!string.IsNullOrEmpty(description));
            _description.text = item.Description;

            _stats.gameObject.SetActive(false);
            _weaponSlots.gameObject.SetActive(false);
            _modification.gameObject.SetActive(false);
        }

        private void UpdateTextField(TextFieldViewModel viewModel, KeyValuePair<string, string> data)
        {
            viewModel.Label.text = _localization.GetString(data.Key);
            viewModel.Value.text = data.Value;
        }

        private static void UpdateWeaponSlot(BlockViewModel viewModel, Barrel data)
        {
            viewModel.Label.text = string.IsNullOrEmpty(data.WeaponClass) ? "•" : data.WeaponClass;
        }

        private static void UpdateWeaponSlot(BlockViewModel viewModel, BarrelSerializable data)
        {
            viewModel.Label.text = string.IsNullOrEmpty(data.WeaponClass) ? "•" : data.WeaponClass;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetShipDescription(IShip ship)
        {
            var size = ship.Model.Layout.CellCount;
            yield return new KeyValuePair<string, string>("$CellCount", size.ToString());
            yield return new KeyValuePair<string, string>("$WeaponSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.Weapon).ToString());
            yield return new KeyValuePair<string, string>("$EnergySize", ship.Model.Layout.Data.Count(value => value == (char)CellType.Engine).ToString());
            yield return new KeyValuePair<string, string>("$ArmorSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.Outer).ToString());
            yield return new KeyValuePair<string, string>("$SpecialSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.Special).ToString());
            yield return new KeyValuePair<string, string>("$UAVplatformSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.UAVPlatform).ToString());
            yield return new KeyValuePair<string, string>("$EngineSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.Engine).ToString());
            yield return new KeyValuePair<string, string>("$CompoundSpaceSize", ship.Model.Layout.Data.Count(value => value == (char)CellType.InnerOuter || value == (char)CellType.InnerSpecial || value == (char)CellType.OuterUAVPlatform).ToString());
            size = ship.Model.SecondLayout.CellCount;
            if (size > 0)
            {
                yield return new KeyValuePair<string, string>("$SecondCellCount", size.ToString());
                yield return new KeyValuePair<string, string>("$SecondWeaponSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.Weapon).ToString());
                yield return new KeyValuePair<string, string>("$SecondEnergySize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.Engine).ToString());
                yield return new KeyValuePair<string, string>("$SecondArmorSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.Outer).ToString());
                yield return new KeyValuePair<string, string>("$SecondSpecialSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.Special).ToString());
                yield return new KeyValuePair<string, string>("$SecondUAVplatformSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.UAVPlatform).ToString());
                yield return new KeyValuePair<string, string>("$SecondEngineSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.Engine).ToString());
                yield return new KeyValuePair<string, string>("$SecondCompoundSpaceSize", ship.Model.SecondLayout.Data.Count(value => value == (char)CellType.InnerOuter || value == (char)CellType.InnerSpecial || value == (char)CellType.OuterUAVPlatform).ToString());
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSatelliteDescription(Satellite satellite)
        {
            var size = satellite.Layout.CellCount;
            yield return new KeyValuePair<string, string>("$CellCount", size.ToString());

            var engineSize = satellite.Layout.Data.Count(value => value == (char)CellType.Engine);
            if (engineSize > 0)
                yield return new KeyValuePair<string, string>("$EngineSize", satellite.Layout.Data.Count(value => value == (char)CellType.Engine).ToString());
            size = satellite.SecondLayout.CellCount;
            yield return new KeyValuePair<string, string>("$SecondCellCount", size.ToString());

            engineSize = satellite.SecondLayout.Data.Count(value => value == (char)CellType.Engine);
            if (engineSize > 0)
                yield return new KeyValuePair<string, string>("$SecondEngineSize", satellite.SecondLayout.Data.Count(value => value == (char)CellType.Engine).ToString());
        }
    }
}
