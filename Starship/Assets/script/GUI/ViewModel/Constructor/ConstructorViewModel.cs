using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Satellites;
using GameModel;
using GameServices.Player;
using GameStateMachine.States;
using Gui.Constructor;
using Services.Messenger;
using Constructor.Ships;
using Diagnostics;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Gui;
using Services.Audio;
using Services.Localization;
using Services.Reources;
using UnityEngine.Events;
using Utils;
using Zenject;
using Component = GameDatabase.DataModel.Component;
using ICommand = Gui.Constructor.ICommand;
using DebugLogSetting;

namespace ViewModel
{
	public class ConstructorViewModel : MonoBehaviour
	{
	    [InjectOptional] private readonly PlayerFleet _playerFleet;
        [InjectOptional] private readonly PlayerInventory _playerInventory;
        [Inject] private readonly Config _config;
	    [Inject] private readonly ExitSignal.Trigger _exitTrigger;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly GuiHelper _guiHelper;
	    [Inject] private readonly IDatabase _database;
	    [Inject] private readonly IResourceLocator _resourceLocator;
	    [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly IDebugManager _debugManager;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<IShip>(EventType.ConstructorShipChanged, OnShipSelected);
			messenger.AddListener(EventType.EscapeKeyPressed, OnCancel);
        }

		[SerializeField] public ToggleGroup HeaderButtons;
        [SerializeField] private ComponentList _componentList;
        [SerializeField] public ComponentInfoViewModel ComponentInfo;
        [SerializeField] public FleetPanelViewModel FleetPanel;
        [SerializeField] public StatsPanelViewModel Stats;
        [SerializeField] public GameObject ScorePanel;
        [SerializeField] public ScorePanelViewModel Score;
        [SerializeField] public ShipLayoutViewModel ShipLayoutViewModel;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Left_1;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Right_1;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Left_2;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Right_2;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Left_3;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Right_3;
		[SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Left_4;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Right_4;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Left_5;
        [SerializeField] public ShipLayoutViewModel PlatformLayoutViewModel_Right_5;

		public GameObject Panel;

        [SerializeField] public ShipLayoutViewModel Second_ShipLayoutViewModel;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_1;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_1;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_2;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_2;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_3;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_3;
		[SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_4;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_4;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_5;
        [SerializeField] public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_5;

		public GameObject Second_Panel;

		public int layout;
		public Image layout_toggle_image;
		public Sprite[] layout_sprite;

		public DraggableComponentObject draggableComponentObject;

        [SerializeField] public SatellitesPanel SatellitesPanel;
        [SerializeField] public CustomInputField NameText;
        [SerializeField] public CanvasGroup ShipPanel;
        [SerializeField] public CanvasGroup Second_ShipPanel;
        [SerializeField] public GameObject[] DisabledInViewMode;
		[SerializeField] public Toggle ViewModeToggle;
		[SerializeField] public GameObject[] DisabledInFreeMoveMode;
	    [SerializeField] private Button _rollbackButton;
        [SerializeField] private AudioClip _installSound;

		[SerializeField] ScrollRect scrollRect;
		[SerializeField] Zoom zoom;
		[SerializeField] RectTransform panel_1;
		[SerializeField] RectTransform panel_2;


		[Serializable]
        public class CommandEvent : UnityEvent<ICommand> { }

        public int ShipSize { get; private set; }

		public IShip Ship { get; private set; }

        public bool IsEditorMode { get; private set; }

	    public int GetDefaultKey(ItemId<Component> componentId)
		{
			var keys = new HashSet<int>();
			var items = ShipLayoutViewModel.Components.
				Concat(PlatformLayoutViewModel_Left_1.Components).
				Concat(PlatformLayoutViewModel_Right_1.Components).
				Concat(PlatformLayoutViewModel_Left_2.Components).
				Concat(PlatformLayoutViewModel_Right_2.Components).
				Concat(PlatformLayoutViewModel_Left_3.Components).
				Concat(PlatformLayoutViewModel_Right_3.Components).
				Concat(PlatformLayoutViewModel_Left_4.Components).
				Concat(PlatformLayoutViewModel_Right_4.Components).
				Concat(PlatformLayoutViewModel_Left_5.Components).
				Concat(PlatformLayoutViewModel_Right_5.Components).
				Concat(Second_ShipLayoutViewModel.Components).
				Concat(Second_PlatformLayoutViewModel_Left_1.Components).
				Concat(Second_PlatformLayoutViewModel_Right_1.Components).
				Concat(Second_PlatformLayoutViewModel_Left_2.Components).
				Concat(Second_PlatformLayoutViewModel_Right_2.Components).
				Concat(Second_PlatformLayoutViewModel_Left_3.Components).
				Concat(Second_PlatformLayoutViewModel_Right_3.Components).
				Concat(Second_PlatformLayoutViewModel_Left_4.Components).
				Concat(Second_PlatformLayoutViewModel_Right_4.Components).
				Concat(Second_PlatformLayoutViewModel_Left_5.Components).
				Concat(Second_PlatformLayoutViewModel_Right_5.Components)
				;

			foreach (var item in items)
			{
				if (item.Info.CreateComponent(ShipSize).ActivationType == ActivationType.None)
					continue;
				var key = item.KeyBinding;
				if (item.Info.Data.Id == componentId)
					return key;
				keys.Add(key);
			}
			
			for (int i = 0; i < 10; ++i)
				if (!keys.Contains(i))
					return i;
			
			return 0;
		}

		public bool IsUniqueItemInstalled(Component component)
		{
			var keys = component.GetUniqueKey();
			if (keys.Length == 0)
				return false;

			var items = ShipLayoutViewModel.Components.
                Concat(PlatformLayoutViewModel_Left_1.Components).
                Concat(PlatformLayoutViewModel_Right_1.Components).
                Concat(PlatformLayoutViewModel_Left_2.Components).
                Concat(PlatformLayoutViewModel_Right_2.Components).
                Concat(PlatformLayoutViewModel_Left_3.Components).
                Concat(PlatformLayoutViewModel_Right_3.Components).
                Concat(PlatformLayoutViewModel_Left_4.Components).
                Concat(PlatformLayoutViewModel_Right_4.Components).
                Concat(PlatformLayoutViewModel_Left_5.Components).
                Concat(PlatformLayoutViewModel_Right_5.Components).
                Concat(Second_ShipLayoutViewModel.Components).
                Concat(Second_PlatformLayoutViewModel_Left_1.Components).
                Concat(Second_PlatformLayoutViewModel_Right_1.Components).
                Concat(Second_PlatformLayoutViewModel_Left_2.Components).
                Concat(Second_PlatformLayoutViewModel_Right_2.Components).
                Concat(Second_PlatformLayoutViewModel_Left_3.Components).
                Concat(Second_PlatformLayoutViewModel_Right_3.Components).
                Concat(Second_PlatformLayoutViewModel_Left_4.Components).
                Concat(Second_PlatformLayoutViewModel_Right_4.Components).
                Concat(Second_PlatformLayoutViewModel_Left_5.Components).
                Concat(Second_PlatformLayoutViewModel_Right_5.Components)
                ;

			return items.Any(item => item.Info.Data.GetUniqueKey().Any(key => keys.Contains(key)));
		}
		public void OnComponentInstalled(ComponentInfo component)
		{
			_components.Remove(component);
		    var shouldShowComponent = ComponentInfo.gameObject.activeSelf && _components.Items.GetQuantity(component) > 0;

            UpdateStats();
			UpdateComponentList();

			if (shouldShowComponent)
				ShowComponent(component);

            if (_installSound)
                _soundPlayer.Play(_installSound);
		}

		public void OnComponentRemoved(ComponentInfo component)
		{
			_components.Add(component);

			UpdateStats();
			UpdateComponentList();
		}

		public void OnComponentUnlocked(IntegratedComponent component)
		{
		}

		public void OnViewModeSelected(bool selected)
		{
			if (layout == 0)
			{
				ShowComponentList();
				ShipPanel.blocksRaycasts = !selected;
				foreach (var item in DisabledInViewMode)
					item.SetActive(!selected);

				var transform = ShipPanel.GetComponent<RectTransform>();
				if (selected)
				{
					var size = transform.rect.size;
					var container = transform.parent.GetComponent<RectTransform>().rect.size;
					transform.localScale = Vector3.one * Mathf.Clamp01(container.y / size.y);
				}
				else
				{
					transform.localScale = Vector3.one;
				}
			}
			else if(layout == 1)
			{
                ShowComponentList();
                Second_ShipPanel.blocksRaycasts = !selected;
                foreach (var item in DisabledInViewMode)
                    item.SetActive(!selected);

                var transform = Second_ShipPanel.GetComponent<RectTransform>();
                if (selected)
                {
                    var size = transform.rect.size;
                    var container = transform.parent.GetComponent<RectTransform>().rect.size;
                    transform.localScale = Vector3.one * Mathf.Clamp01(container.y / size.y);
                }
                else
                {
                    transform.localScale = Vector3.one;
                }
            }
        }

		public void OnFreeMoveModeSelected(bool selected)
		{
			ShowComponentList();
            foreach (var item in DisabledInFreeMoveMode)
                item.SetActive(!selected);

        }

		public void OnSecondLayoutModeSelected(bool selected)
		{
			layout = selected ? 1 : 0;
			layout_toggle_image.sprite = layout_sprite[layout];
            UpgradeLayout();
        }

        public void ShowWeapons()
		{
			UnityEngine.Debug.Log("ShowWeapons");
            ShowComponentList();
		    _componentList.ShowWeapon();
		}

		public void ShowEnergy()
		{
			UnityEngine.Debug.Log("ShowEnergy");
		    ShowComponentList();
		    _componentList.ShowEnergy();
		}

		public void ShowDefense()
		{
			UnityEngine.Debug.Log("ShowDefense");
		    ShowComponentList();
		    _componentList.ShowArmor();
		}

		public void ShowEngine()
		{
			UnityEngine.Debug.Log("ShowEngine");
		    ShowComponentList();
		    _componentList.ShowEngine();
		}
        
        public void ShowDrones()
		{
			UnityEngine.Debug.Log("ShowDrones");
		    ShowComponentList();
		    _componentList.ShowDrone();
		}

		public void ShowSpecial()
		{
			UnityEngine.Debug.Log("ShowSpecial");
		    ShowComponentList();
		    _componentList.ShowSpecial();
		}

		public void ShowEquipment()
		{
			UnityEngine.Debug.Log("ShowEquipment");
		    ShowComponentList();
		    _componentList.ShowEquipment();
		}

        public void ShowAll()
	    {
	        ShowComponentList();
	        _componentList.ShowAll();
        }

        public void OnItemDeselected()
	    {
	        if (!HeaderButtons.AnyTogglesOn())
                ShowAll();
	    }

		public void RemoveAll()
		{
			if (HasUnlockedComponents)
                _guiHelper.ShowConfirmation(_localization.GetString("$RemoveAllConfirmation"), RemoveAllComponents);
		}

	    public void OnCommandExecuted(ICommand command)
	    {
	        _commands.Push(command);
	        _rollbackButton.interactable = true;
	    }

	    public void Undo()
	    {
	        if (_commands.Count > 0)
	        {
	            var command = _commands.Pop();

	            if (!command.TryRollback())
	            {
	                UnityEngine.Debug.Log("Undo - failed");
                    _commands.Clear();
	            }
	        }

            _rollbackButton.interactable = _commands.Count > 0;
        }
        
		public void Exit()
		{
			if (!this) return;
            SaveCurrentShip();
		    _exitTrigger.Fire();
		}

		public void SaveCurrentShip()
		{
			Ship.Components.Assign(Second_layout != null ? _layout.Components.Concat(Second_layout.Components) : _layout.Components);
			if (!String.IsNullOrEmpty(NameText.text))
				Ship.Name = NameText.text;

			Ship.Satellite_Left_1?.Components.Assign(Second_PlatformLayout_left_1 != null ? PlatformLayout_left_1.Components.Concat(Second_PlatformLayout_left_1.Components) : PlatformLayout_left_1.Components);
            Ship.Satellite_Right_1?.Components.Assign(Second_PlatformLayout_right_1 != null ? PlatformLayout_right_1.Components.Concat(Second_PlatformLayout_right_1.Components) : PlatformLayout_right_1.Components);
            Ship.Satellite_Left_2?.Components.Assign(Second_PlatformLayout_left_2 != null ? PlatformLayout_left_2.Components.Concat(Second_PlatformLayout_left_2.Components) : PlatformLayout_left_2.Components);
            Ship.Satellite_Right_2?.Components.Assign(Second_PlatformLayout_right_2 != null ? PlatformLayout_right_2.Components.Concat(Second_PlatformLayout_right_2.Components) : PlatformLayout_right_2.Components);
            Ship.Satellite_Left_3?.Components.Assign(Second_PlatformLayout_left_3 != null ? PlatformLayout_left_3.Components.Concat(Second_PlatformLayout_left_3.Components) : PlatformLayout_left_3.Components);
            Ship.Satellite_Right_3?.Components.Assign(Second_PlatformLayout_right_3 != null ? PlatformLayout_right_3.Components.Concat(Second_PlatformLayout_right_3.Components) : PlatformLayout_right_3.Components);
            Ship.Satellite_Left_4?.Components.Assign(Second_PlatformLayout_left_4 != null ? PlatformLayout_left_4.Components.Concat(Second_PlatformLayout_left_4.Components) : PlatformLayout_left_4.Components);
            Ship.Satellite_Right_4?.Components.Assign(Second_PlatformLayout_right_4 != null ? PlatformLayout_right_4.Components.Concat(Second_PlatformLayout_right_4.Components) : PlatformLayout_right_4.Components);
            Ship.Satellite_Left_5?.Components.Assign(Second_PlatformLayout_left_5 != null ? PlatformLayout_left_5.Components.Concat(Second_PlatformLayout_left_5.Components) : PlatformLayout_left_5.Components);
            Ship.Satellite_Right_5?.Components.Assign(Second_PlatformLayout_right_5 != null ? PlatformLayout_right_5.Components.Concat(Second_PlatformLayout_right_5.Components) : PlatformLayout_right_5.Components);
/*
            Ship.Satellite_Left_1?.Components.Assign(PlatformLayout_left_1.Components);
            Ship.Satellite_Right_1?.Components.Assign(PlatformLayout_right_1.Components);
            Ship.Satellite_Left_2?.Components.Assign(PlatformLayout_left_2.Components);
            Ship.Satellite_Right_2?.Components.Assign(PlatformLayout_right_2.Components);
            Ship.Satellite_Left_3?.Components.Assign(PlatformLayout_left_3.Components);
            Ship.Satellite_Right_3?.Components.Assign(PlatformLayout_right_3.Components);
            Ship.Satellite_Left_4?.Components.Assign(PlatformLayout_left_4.Components);
            Ship.Satellite_Right_4?.Components.Assign(PlatformLayout_right_4.Components);
            Ship.Satellite_Left_5?.Components.Assign(PlatformLayout_left_5.Components);
            Ship.Satellite_Right_5?.Components.Assign(PlatformLayout_right_5.Components);
*/
            if (!IsEditorMode)
				_components.SaveToInventory(_playerInventory);
        }

        public void ShowSatellites()
        {
            _componentList.gameObject.SetActive(false);
            ComponentInfo.gameObject.SetActive(false);
            FleetPanel.Close();
            SatellitesPanel.Open(Ship, IsEditorMode);
            ScorePanel.SetActive(false);
        }

        public void ShowComponent(ComponentInfo component)
		{
			FleetPanel.Close();
            _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(true);
            ComponentInfo.SetComponent(component);
            SatellitesPanel.gameObject.SetActive(false);
            ScorePanel.SetActive(false);
        }

        public void ShowComponent(ShipLayoutViewModel activeLayout, int id)
		{
			FleetPanel.Close();
            _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(true);
			ComponentInfo.SetComponent(activeLayout, id);
            SatellitesPanel.gameObject.SetActive(false);
            ScorePanel.SetActive(false);
        }

        public void ShowComponentList()
		{
			FleetPanel.Close();
            ComponentInfo.gameObject.SetActive(false);
            _componentList.gameObject.SetActive(true);
            SatellitesPanel.gameObject.SetActive(false);
            ScorePanel.SetActive(false);
        }

        public void ShowShipList()
		{
	        _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(false);
			FleetPanel.Open(IsEditorMode);
            SatellitesPanel.gameObject.SetActive(false);
			ScorePanel.SetActive(false);
        }
        public void ShowShipScore()
		{
	        _componentList.gameObject.SetActive(false);
			ComponentInfo.gameObject.SetActive(false);
            FleetPanel.Close();
            SatellitesPanel.gameObject.SetActive(false);
			ScorePanel.SetActive(true);
        }

	    public void InstallSatellite(ItemId<SatelliteBuild> buildId, ItemId<Satellite> id, CompanionLocation location)
	    {
            SaveCurrentShip();

            ISatellite satellite = null;
            if (!id.IsNull)
            {
                if (IsEditorMode)
                {
                    var item = _database.GetSatelliteBuild(buildId);
                    if (item == null || !Ship.IsSuitableSatelliteSize(item.Satellite))
                        throw new ArgumentException("cannot install " + buildId + " in " + Ship.Id);

                    satellite = new EditorModeSatellite(item, _database);
                }
                else
                {
                    var item = _database.GetSatellite(id);
                    if (item == null || !Ship.IsSuitableSatelliteSize(item))
                        throw new ArgumentException("cannot install " + id + " in " + Ship.Id);

                    if (_playerInventory.Satellites.Remove(item) < 1)
                        throw new ArgumentException("satellite not found in inventory");

                    satellite = new CommonSatellite(item, Enumerable.Empty<IntegratedComponent>());
                }
            }

	        ISatellite oldValue;
	        if (location == CompanionLocation.Left)
	        {
	            oldValue = Ship.Satellite_Left_1;
	            Ship.Satellite_Left_1 = satellite;
	        }
            else if(location == CompanionLocation.Right)
            {
	            oldValue = Ship.Satellite_Right_1;
	            Ship.Satellite_Right_1 = satellite;
	        }
            else if(location == CompanionLocation.Left_2)
            {
	            oldValue = Ship.Satellite_Left_2;
	            Ship.Satellite_Left_2 = satellite;
	        }
            else if(location == CompanionLocation.Right_2)
            {
	            oldValue = Ship.Satellite_Right_2;
	            Ship.Satellite_Right_2 = satellite;
	        }
            else if(location == CompanionLocation.Left_3)
            {
	            oldValue = Ship.Satellite_Left_3;
	            Ship.Satellite_Left_3 = satellite;
	        }
            else if(location == CompanionLocation.Right_3)
            {
	            oldValue = Ship.Satellite_Right_3;
	            Ship.Satellite_Right_3 = satellite;
	        }
            else if(location == CompanionLocation.Left_4)
            {
	            oldValue = Ship.Satellite_Left_4;
	            Ship.Satellite_Left_4 = satellite;
	        }
            else if(location == CompanionLocation.Right_4)
            {
	            oldValue = Ship.Satellite_Right_4;
	            Ship.Satellite_Right_4 = satellite;
	        }
            else if(location == CompanionLocation.Left_5)
            {
	            oldValue = Ship.Satellite_Left_5;
	            Ship.Satellite_Left_5 = satellite;
	        }
            else
            {
	            oldValue = Ship.Satellite_Right_5;
	            Ship.Satellite_Right_5 = satellite;
	        }

            if (oldValue != null && !IsEditorMode)
            {
            	_playerInventory.Satellites.Add(oldValue.Information);
                foreach (var item in oldValue.Components)
                    _playerInventory.Components.Add(item.Info);
            }

            Initialize(Ship);
	    }

        private void OnShipSelected(IShip ship)
        {
            if (Ship != null)
                SaveCurrentShip();

            Ship = ship;
            Initialize(ship);
        }

		private void Initialize(IShip ship)
		{
		    Ship = ship;

		    IsEditorMode = _playerFleet == null || _playerInventory == null || !_playerFleet.Ships.Contains(ship);

		    var debug = _debugManager.CreateLog(ship.Name);
			_layout = new ShipLayout(Ship.Model.Layout, ship.Model.Barrels, ship.Components.Where(item => item.Layout == 0), debug);
			Second_layout = Ship.Model.SecondLayout.Data != string.Empty ? new ShipLayout(Ship.Model.SecondLayout, null, ship.Components.Where(item => item.Layout == 1), debug) : null;
			if (ShipDebugLogSetting.ShipLayoutDebugLog)
			{
				if (_layout != null)
					UnityEngine.Debug.Log("_layout set  " + Ship.Model.Layout.Data);
				if (Second_layout != null)
					UnityEngine.Debug.Log("Second_layout set  " + Ship.Model.SecondLayout.Data);
			}
            NameText.text = _localization.GetString(ship.Name);

            PlatformLayout_left_1 = ship.Satellite_Left_1 != null ? new ShipLayout(ship.Satellite_Left_1.Information.Layout, ship.Satellite_Left_1.Information.Barrels, ship.Satellite_Left_1.Components.Where(item => item.Layout == 0), debug) : null;
            PlatformLayout_right_1 = ship.Satellite_Right_1 != null ? new ShipLayout(ship.Satellite_Right_1.Information.Layout, ship.Satellite_Right_1.Information.Barrels,  ship.Satellite_Right_1.Components.Where(item => item.Layout == 0), debug) : null;

            PlatformLayout_left_2 = ship.Satellite_Left_2 != null ? new ShipLayout(ship.Satellite_Left_2.Information.Layout, ship.Satellite_Left_2.Information.Barrels, ship.Satellite_Left_2.Components.Where(item => item.Layout == 0), debug) : null;
            PlatformLayout_right_2 = ship.Satellite_Right_2 != null ? new ShipLayout(ship.Satellite_Right_2.Information.Layout, ship.Satellite_Right_2.Information.Barrels, ship.Satellite_Right_2.Components.Where(item => item.Layout == 0), debug) : null;

            PlatformLayout_left_3 = ship.Satellite_Left_3 != null ? new ShipLayout(ship.Satellite_Left_3.Information.Layout, ship.Satellite_Left_3.Information.Barrels, ship.Satellite_Left_3.Components.Where(item => item.Layout == 0), debug) : null;
            PlatformLayout_right_3 = ship.Satellite_Right_3 != null ? new ShipLayout(ship.Satellite_Right_3.Information.Layout, ship.Satellite_Right_3.Information.Barrels,ship.Satellite_Right_3.Components.Where(item => item.Layout == 0), debug) : null;

            PlatformLayout_left_4 = ship.Satellite_Left_4 != null ? new ShipLayout(ship.Satellite_Left_4.Information.Layout, ship.Satellite_Left_4.Information.Barrels, ship.Satellite_Left_4.Components.Where(item => item.Layout == 0), debug) : null;
            PlatformLayout_right_4 = ship.Satellite_Right_4 != null ? new ShipLayout(ship.Satellite_Right_4.Information.Layout, ship.Satellite_Right_4.Information.Barrels, ship.Satellite_Right_4.Components.Where(item => item.Layout == 0), debug) : null;

            PlatformLayout_left_5 = ship.Satellite_Left_5 != null ? new ShipLayout(ship.Satellite_Left_5.Information.Layout, ship.Satellite_Left_5.Information.Barrels,ship.Satellite_Left_5.Components.Where(item => item.Layout == 0), debug) : null;
            PlatformLayout_right_5 = ship.Satellite_Right_5 != null ? new ShipLayout(ship.Satellite_Right_5.Information.Layout, ship.Satellite_Right_5.Information.Barrels, ship.Satellite_Right_5.Components.Where(item => item.Layout == 0), debug) : null;

			Second_PlatformLayout_left_1 = ship.Satellite_Left_1 != null && ship.Satellite_Left_1.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Left_1.Information.SecondLayout, null, ship.Satellite_Left_1.Components.Where(item=>item.Layout==1), debug) : null;
            Second_PlatformLayout_right_1 = ship.Satellite_Right_1 != null && ship.Satellite_Right_1.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Right_1.Information.SecondLayout, null, ship.Satellite_Right_1.Components.Where(item => item.Layout == 1), debug) : null;

            Second_PlatformLayout_left_2 = ship.Satellite_Left_2 != null && ship.Satellite_Left_2.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Left_2.Information.SecondLayout, null, ship.Satellite_Left_2.Components.Where(item => item.Layout == 1), debug) : null;
            Second_PlatformLayout_right_2 = ship.Satellite_Right_2 != null && ship.Satellite_Right_2.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Right_2.Information.SecondLayout, null, ship.Satellite_Right_2.Components.Where(item => item.Layout == 1), debug) : null;

            Second_PlatformLayout_left_3 = ship.Satellite_Left_3 != null && ship.Satellite_Left_3.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Left_3.Information.SecondLayout, null, ship.Satellite_Left_3.Components.Where(item => item.Layout == 1), debug) : null;
            Second_PlatformLayout_right_3 = ship.Satellite_Right_3 != null && ship.Satellite_Right_3.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Right_3.Information.SecondLayout, null, ship.Satellite_Right_3.Components.Where(item => item.Layout == 1), debug) : null;

            Second_PlatformLayout_left_4 = ship.Satellite_Left_4 != null && ship.Satellite_Left_4.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Left_4.Information.SecondLayout, null, ship.Satellite_Left_4.Components.Where(item => item.Layout == 1), debug) : null;
            Second_PlatformLayout_right_4 = ship.Satellite_Right_4 != null && ship.Satellite_Right_4.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Right_4.Information.SecondLayout, null, ship.Satellite_Right_4.Components.Where(item => item.Layout == 1), debug) : null;

            Second_PlatformLayout_left_5 = ship.Satellite_Left_5 != null && ship.Satellite_Left_5.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Left_5.Information.SecondLayout, null, ship.Satellite_Left_5.Components.Where(item => item.Layout == 1), debug) : null;
            Second_PlatformLayout_right_5 = ship.Satellite_Right_5 != null && ship.Satellite_Right_5.Information.SecondLayout.Data != string.Empty ? new ShipLayout(ship.Satellite_Right_5.Information.SecondLayout,null, ship.Satellite_Right_5.Components.Where(item => item.Layout == 1), debug) : null;

			ShipSize = Ship.Model.Layout.CellCount + Ship.Model.SecondLayout.CellCount;

		    if (IsEditorMode)
				_components.LoadFromDatabase(_database);
		    else
		        _components.LoadFromInventory(_playerInventory);

            _componentList.Initialize(_components.Items);

			layout = 0;
			draggableComponentObject.layoutnum = 0;

            ResetLayout();
			UpdateStats();
		}

		private void ResetLayout()
		{
			_commands.Clear();
			ShipLayoutViewModel.Initialize(_layout, _components);
			ShipLayoutViewModel.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Model.ModelImage);
			if (PlatformLayout_left_1 != null)
			{
				PlatformLayoutViewModel_Left_1.Initialize(PlatformLayout_left_1, _components);
				PlatformLayoutViewModel_Left_1.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_1.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Left_1 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Left_1.Reset();
			}

			if (PlatformLayout_right_1 != null)
			{
				PlatformLayoutViewModel_Right_1.Initialize(PlatformLayout_right_1, _components);
				PlatformLayoutViewModel_Right_1.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_1.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Right_1 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_1.Reset();
			}

			if (PlatformLayout_left_2 != null)
			{
				PlatformLayoutViewModel_Left_2.Initialize(PlatformLayout_left_2, _components);
				PlatformLayoutViewModel_Left_2.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_2.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Left_2 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Left_2.Reset();
			}

			if (PlatformLayout_right_2 != null)
			{
				PlatformLayoutViewModel_Right_2.Initialize(PlatformLayout_right_2, _components);
				PlatformLayoutViewModel_Right_2.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_2.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Right_2 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_2.Reset();
			}

			if (PlatformLayout_left_3 != null)
			{
				PlatformLayoutViewModel_Left_3.Initialize(PlatformLayout_left_3, _components);
				PlatformLayoutViewModel_Left_3.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_3.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Left_3 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Left_3.Reset();
			}

			if (PlatformLayout_right_3 != null)
			{
				PlatformLayoutViewModel_Right_3.Initialize(PlatformLayout_right_3, _components);
				PlatformLayoutViewModel_Right_3.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_3.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Right_3 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_3.Reset();
			}

			if (PlatformLayout_left_4 != null)
			{
				PlatformLayoutViewModel_Left_4.Initialize(PlatformLayout_left_4, _components);
				PlatformLayoutViewModel_Left_4.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_4.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Left_4 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Left_4.Reset();
			}

			if (PlatformLayout_right_4 != null)
			{
				PlatformLayoutViewModel_Right_4.Initialize(PlatformLayout_right_4, _components);
				PlatformLayoutViewModel_Right_4.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_4.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Right_4 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_4.Reset();
			}

			if (PlatformLayout_left_5 != null)
			{
				PlatformLayoutViewModel_Left_5.Initialize(PlatformLayout_left_5, _components);
				PlatformLayoutViewModel_Left_5.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_5.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Left_5 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Left_5.Reset();
			}

			if (PlatformLayout_right_5 != null)
			{
				PlatformLayoutViewModel_Right_5.Initialize(PlatformLayout_right_5, _components);
				PlatformLayoutViewModel_Right_5.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_5.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("PlatformLayoutViewModel_Right_5 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_5.Reset();
			}

			if (Second_layout != null)
			{
				Second_ShipLayoutViewModel.Initialize(Second_layout, _components);
				Second_ShipLayoutViewModel.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Model.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_ShipLayoutViewModel set  " + Second_layout.CellCount);
            }
            else
			{
                Second_ShipLayoutViewModel.Reset();
            }

            if (Second_PlatformLayout_left_1 != null)
			{
				Second_PlatformLayoutViewModel_Left_1.Initialize(Second_PlatformLayout_left_1, _components);
				Second_PlatformLayoutViewModel_Left_1.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_1.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Left_1 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Left_1.Reset();
			}

			if (Second_PlatformLayout_right_1 != null)
			{
				Second_PlatformLayoutViewModel_Right_1.Initialize(Second_PlatformLayout_right_1, _components);
				Second_PlatformLayoutViewModel_Right_1.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_1.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Right_1 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Right_1.Reset();
			}

			if (Second_PlatformLayout_left_2 != null)
			{
				Second_PlatformLayoutViewModel_Left_2.Initialize(Second_PlatformLayout_left_2, _components);
				Second_PlatformLayoutViewModel_Left_2.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_2.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Left_2 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Left_2.Reset();
			}

			if (Second_PlatformLayout_right_2 != null)
			{
				Second_PlatformLayoutViewModel_Right_2.Initialize(Second_PlatformLayout_right_2, _components);
				Second_PlatformLayoutViewModel_Right_2.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_2.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Right_2 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Right_2.Reset();
			}

			if (Second_PlatformLayout_left_3 != null)
			{
				Second_PlatformLayoutViewModel_Left_3.Initialize(Second_PlatformLayout_left_3, _components);
				Second_PlatformLayoutViewModel_Left_3.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_3.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Left_3 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Left_3.Reset();
			}

			if (Second_PlatformLayout_right_3 != null)
			{
				Second_PlatformLayoutViewModel_Right_3.Initialize(Second_PlatformLayout_right_3, _components);
				Second_PlatformLayoutViewModel_Right_3.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_3.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Right_3 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Right_3.Reset();
			}

			if (Second_PlatformLayout_left_4 != null)
			{
				Second_PlatformLayoutViewModel_Left_4.Initialize(Second_PlatformLayout_left_4, _components);
				Second_PlatformLayoutViewModel_Left_4.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_4.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Left_4 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Left_4.Reset();
			}

			if (Second_PlatformLayout_right_4 != null)
			{
				Second_PlatformLayoutViewModel_Right_4.Initialize(Second_PlatformLayout_right_4, _components);
				Second_PlatformLayoutViewModel_Right_4.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_4.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Right_4 set  ");
            }
            else
			{
				PlatformLayoutViewModel_Right_4.Reset();
			}

			if (Second_PlatformLayout_left_5 != null)
			{
				Second_PlatformLayoutViewModel_Left_5.Initialize(Second_PlatformLayout_left_5, _components);
				Second_PlatformLayoutViewModel_Left_5.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Left_5.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Left_5 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Left_5.Reset();
			}

			if (Second_PlatformLayout_right_5 != null)
			{
				Second_PlatformLayoutViewModel_Right_5.Initialize(Second_PlatformLayout_right_5, _components);
				Second_PlatformLayoutViewModel_Right_5.BackgroundImage.sprite = _resourceLocator.GetSprite(Ship.Satellite_Right_5.Information.ModelImage);
                if (ShipDebugLogSetting.ShipLayoutDebugLog)
                    UnityEngine.Debug.Log("Second_PlatformLayoutViewModel_Right_5 set  ");
            }
            else
			{
				Second_PlatformLayoutViewModel_Right_5.Reset();
			}
		}

		private void UpgradeLayout()
		{
			if (layout == 0)
			{
				Panel.SetActive(true);
				Second_Panel.SetActive(false);
				scrollRect.content = panel_1;
				zoom.ChangeZoom(panel_1);
            }
			else if(layout == 1)
			{
                Panel.SetActive(false);
                Second_Panel.SetActive(true);
				scrollRect.content = panel_2;
				zoom.ChangeZoom(panel_2);
            }
			draggableComponentObject.layoutnum = layout;
        }

        private bool HasUnlockedComponents
		{
			get
			{
				if (_layout.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_left_1 != null && PlatformLayout_left_1.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_right_1 != null && PlatformLayout_right_1.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_left_2 != null && PlatformLayout_left_2.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_right_2 != null && PlatformLayout_right_2.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_left_3 != null && PlatformLayout_left_3.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_right_3 != null && PlatformLayout_right_3.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_left_4 != null && PlatformLayout_left_4.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_right_4 != null && PlatformLayout_right_4.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_left_5 != null && PlatformLayout_left_5.Components.Any(item => !item.Locked && item.Layout == 0)) return true;
				if (PlatformLayout_right_5 != null && PlatformLayout_right_5.Components.Any(item => !item.Locked && item.Layout == 0)) return true;

				if (Second_layout != null && Second_layout.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_left_1 != null && Second_PlatformLayout_left_1.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_right_1 != null && Second_PlatformLayout_right_1.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_left_2 != null && Second_PlatformLayout_left_2.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_right_2 != null && Second_PlatformLayout_right_2.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_left_3 != null && Second_PlatformLayout_left_3.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_right_3 != null && Second_PlatformLayout_right_3.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_left_4 != null && Second_PlatformLayout_left_4.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_right_4 != null && Second_PlatformLayout_right_4.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_left_5 != null && Second_PlatformLayout_left_5.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				if (Second_PlatformLayout_right_5 != null && Second_PlatformLayout_right_5.Components.Any(item => !item.Locked && item.Layout == 1)) return true;
				return false;
			}
		}

		private void RemoveAllComponents()
		{
			if (IsEditorMode)
			{
				if (layout == 0)
				{
					_layout.Clear();
					PlatformLayout_left_1?.Clear();
					PlatformLayout_right_1?.Clear();
					PlatformLayout_left_2?.Clear();
					PlatformLayout_right_2?.Clear();
					PlatformLayout_left_3?.Clear();
					PlatformLayout_right_3?.Clear();
					PlatformLayout_left_4?.Clear();
					PlatformLayout_right_4?.Clear();
					PlatformLayout_left_5?.Clear();
					PlatformLayout_right_5?.Clear();
				}
				else if (layout == 1)
				{
					Second_layout.Clear();
					Second_PlatformLayout_left_1?.Clear();
					Second_PlatformLayout_right_1?.Clear();
					Second_PlatformLayout_left_2?.Clear();
					Second_PlatformLayout_right_2?.Clear();
					Second_PlatformLayout_left_3?.Clear();
					Second_PlatformLayout_right_3?.Clear();
					Second_PlatformLayout_left_4?.Clear();
					Second_PlatformLayout_right_4?.Clear();
					Second_PlatformLayout_left_5?.Clear();
					Second_PlatformLayout_right_5?.Clear();
				}
			}
			else
			{
				if (layout == 0)
				{
					var components = _layout.Components.Where(item => !item.Locked).ToArray();
					foreach (var item in components)
					{
						_components.Add(item.Info);
						_layout.RemoveComponent(item);
					}

					if (Second_layout != null)
					{
						components = Second_layout.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_layout.RemoveComponent(item);
						}
					}

					if (PlatformLayout_left_1 != null)
					{
						components = PlatformLayout_left_1.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_left_1.RemoveComponent(item);
						}
					}

					if (PlatformLayout_right_1 != null)
					{
						components = PlatformLayout_right_1.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_right_1.RemoveComponent(item);
						}
					}

					if (PlatformLayout_left_2 != null)
					{
						components = PlatformLayout_left_2.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_left_2.RemoveComponent(item);
						}
					}

					if (PlatformLayout_right_2 != null)
					{
						components = PlatformLayout_right_2.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_right_2.RemoveComponent(item);
						}
					}

					if (PlatformLayout_left_3 != null)
					{
						components = PlatformLayout_left_3.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_left_3.RemoveComponent(item);
						}
					}

					if (PlatformLayout_right_3 != null)
					{
						components = PlatformLayout_right_3.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_right_3.RemoveComponent(item);
						}
					}

					if (PlatformLayout_left_4 != null)
					{
						components = PlatformLayout_left_4.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_left_4.RemoveComponent(item);
						}
					}

					if (PlatformLayout_right_4 != null)
					{
						components = PlatformLayout_right_4.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_right_4.RemoveComponent(item);
						}
					}

					if (PlatformLayout_left_5 != null)
					{
						components = PlatformLayout_left_5.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_left_5.RemoveComponent(item);
						}
					}

					if (PlatformLayout_right_5 != null)
					{
						components = PlatformLayout_right_5.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							PlatformLayout_right_5.RemoveComponent(item);
						}
					}
				}
				else if (layout == 1)
				{
					var components = new IntegratedComponent[0];

                    if (Second_PlatformLayout_left_1 != null)
					{
						components = Second_PlatformLayout_left_1.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_left_1.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_right_1 != null)
					{
						components = Second_PlatformLayout_right_1.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_right_1.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_left_2 != null)
					{
						components = Second_PlatformLayout_left_2.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_left_2.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_right_2 != null)
					{
						components = Second_PlatformLayout_right_2.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_right_2.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_left_3 != null)
					{
						components = Second_PlatformLayout_left_3.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_left_3.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_right_3 != null)
					{
						components = Second_PlatformLayout_right_3.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_right_3.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_left_4 != null)
					{
						components = Second_PlatformLayout_left_4.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_left_4.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_right_4 != null)
					{
						components = Second_PlatformLayout_right_4.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_right_4.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_left_5 != null)
					{
						components = Second_PlatformLayout_left_5.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_left_5.RemoveComponent(item);
						}
					}

					if (Second_PlatformLayout_right_5 != null)
					{
						components = Second_PlatformLayout_right_5.Components.Where(item => !item.Locked).ToArray();
						foreach (var item in components)
						{
							_components.Add(item.Info);
							Second_PlatformLayout_right_5.RemoveComponent(item);
						}
					}
				}
			}

			ResetLayout();
			UpdateStats();
			UpdateComponentList();
		}
		
		private void UpdateStats()
		{
			var builder = new ShipBuilder(Ship.Model, Second_layout != null ? _layout.Components.Concat(Second_layout.Components) : _layout.Components);
			if (PlatformLayout_left_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_1.Information, Second_PlatformLayout_left_1 != null ? PlatformLayout_left_1.Components.Concat(Second_PlatformLayout_left_1.Components) : PlatformLayout_left_1.Components), CompanionLocation.Left);
			if (PlatformLayout_right_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_1.Information, Second_PlatformLayout_right_1 != null ? PlatformLayout_right_1.Components.Concat(Second_PlatformLayout_right_1.Components): PlatformLayout_right_1.Components), CompanionLocation.Right);
			if (PlatformLayout_left_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_2.Information, Second_PlatformLayout_left_2 != null ? PlatformLayout_left_2.Components.Concat(Second_PlatformLayout_left_2.Components): PlatformLayout_left_2.Components), CompanionLocation.Left_2);
			if (PlatformLayout_right_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_2.Information, Second_PlatformLayout_right_2 != null ? PlatformLayout_right_2.Components.Concat(Second_PlatformLayout_right_2.Components): PlatformLayout_right_2.Components), CompanionLocation.Right_2);
			if (PlatformLayout_left_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_3.Information, Second_PlatformLayout_left_3 != null ? PlatformLayout_left_3.Components.Concat(Second_PlatformLayout_left_3.Components): PlatformLayout_left_3.Components), CompanionLocation.Left_3);
			if (PlatformLayout_right_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_3.Information, Second_PlatformLayout_right_3 != null ? PlatformLayout_right_3.Components.Concat(Second_PlatformLayout_right_3.Components): PlatformLayout_right_3.Components), CompanionLocation.Right_3);
			if (PlatformLayout_left_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_4.Information, Second_PlatformLayout_left_4 != null ? PlatformLayout_left_4.Components.Concat(Second_PlatformLayout_left_4.Components): PlatformLayout_left_4.Components), CompanionLocation.Left_4);
			if (PlatformLayout_right_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_4.Information, Second_PlatformLayout_right_4 != null ? PlatformLayout_right_4.Components.Concat(Second_PlatformLayout_right_4.Components): PlatformLayout_right_4.Components), CompanionLocation.Right_4);
			if (PlatformLayout_left_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_5.Information, Second_PlatformLayout_left_5 != null ? PlatformLayout_left_5.Components.Concat(Second_PlatformLayout_left_5.Components): PlatformLayout_left_5.Components), CompanionLocation.Left_5);
			if (PlatformLayout_right_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_5.Information, Second_PlatformLayout_right_5 != null ? PlatformLayout_right_5.Components.Concat(Second_PlatformLayout_right_5.Components): PlatformLayout_right_5.Components), CompanionLocation.Right_5);
/*
			if (Second_PlatformLayout_left_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_1.Information, Second_PlatformLayout_left_1.Components), CompanionLocation.Left);
			if (Second_PlatformLayout_right_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_1.Information, Second_PlatformLayout_right_1.Components), CompanionLocation.Right);
			if (Second_PlatformLayout_left_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_2.Information, Second_PlatformLayout_left_2.Components), CompanionLocation.Left_2);
			if (Second_PlatformLayout_right_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_2.Information, Second_PlatformLayout_right_2.Components), CompanionLocation.Right_2);
			if (Second_PlatformLayout_left_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_3.Information, Second_PlatformLayout_left_3.Components), CompanionLocation.Left_3);
			if (Second_PlatformLayout_right_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_3.Information, Second_PlatformLayout_right_3.Components), CompanionLocation.Right_3);
			if (Second_PlatformLayout_left_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_4.Information, Second_PlatformLayout_left_4.Components), CompanionLocation.Left_4);
			if (Second_PlatformLayout_right_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_4.Information, Second_PlatformLayout_right_4.Components), CompanionLocation.Right_4);
			if (Second_PlatformLayout_left_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_5.Information, Second_PlatformLayout_left_5.Components), CompanionLocation.Left_5);
			if (Second_PlatformLayout_right_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_5.Information, Second_PlatformLayout_right_5.Components), CompanionLocation.Right_5);
*/
/*
			var builder = new ShipBuilder(Ship.Model, _layout.Components);
			if (PlatformLayout_left_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_1.Information, PlatformLayout_left_1.Components), CompanionLocation.Left);
			if (PlatformLayout_right_1 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_1.Information, PlatformLayout_right_1.Components), CompanionLocation.Right);
			if (PlatformLayout_left_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_2.Information, PlatformLayout_left_2.Components), CompanionLocation.Left_2);
			if (PlatformLayout_right_2 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_2.Information, PlatformLayout_right_2.Components), CompanionLocation.Right_2);
			if (PlatformLayout_left_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_3.Information, PlatformLayout_left_3.Components), CompanionLocation.Left_3);
			if (PlatformLayout_right_3 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_3.Information, PlatformLayout_right_3.Components), CompanionLocation.Right_3);
			if (PlatformLayout_left_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_4.Information, PlatformLayout_left_4.Components), CompanionLocation.Left_4);
			if (PlatformLayout_right_4 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_4.Information, PlatformLayout_right_4.Components), CompanionLocation.Right_4);
			if (PlatformLayout_left_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Left_5.Information, PlatformLayout_left_5.Components), CompanionLocation.Left_5);
			if (PlatformLayout_right_5 != null)
				builder.AddSatellite(new CommonSatellite(Ship.Satellite_Right_5.Information, PlatformLayout_right_5.Components), CompanionLocation.Right_5);
*/

			Stats.UpdateStats(builder.Build(_database.ShipSettings));
			Score.UpdateStats(builder.Build(_database.ShipSettings), _database.ShipSettings);
		    _rollbackButton.interactable = _commands.Count > 0;
		}
		private void UpdateComponentList()
		{
            ShowComponentList();
            _componentList.RefreshList();
		}

		private void OnCancel()
        {
            if (!this) return;

			if (ViewModeToggle.isOn)
				ViewModeToggle.isOn = false;
			else
				Exit();
		}
		public IDatabase Database => _database;

        private readonly InventoryComponents _components = new InventoryComponents();
	    private ShipLayout _layout;
		private ShipLayout PlatformLayout_left_1;
		private ShipLayout PlatformLayout_right_1;
		private ShipLayout PlatformLayout_left_2;
		private ShipLayout PlatformLayout_right_2;
		private ShipLayout PlatformLayout_left_3;
		private ShipLayout PlatformLayout_right_3;
		private ShipLayout PlatformLayout_left_4;
		private ShipLayout PlatformLayout_right_4;
		private ShipLayout PlatformLayout_left_5;
		private ShipLayout PlatformLayout_right_5;

        [SerializeField] private ShipLayout Second_layout;
		private ShipLayout Second_PlatformLayout_left_1;
		private ShipLayout Second_PlatformLayout_right_1;
		private ShipLayout Second_PlatformLayout_left_2;
		private ShipLayout Second_PlatformLayout_right_2;
		private ShipLayout Second_PlatformLayout_left_3;
		private ShipLayout Second_PlatformLayout_right_3;
		private ShipLayout Second_PlatformLayout_left_4;
		private ShipLayout Second_PlatformLayout_right_4;
		private ShipLayout Second_PlatformLayout_left_5;
		private ShipLayout Second_PlatformLayout_right_5;
        private Stack<ICommand> _commands = new Stack<ICommand>();

		//private IEnumerable<Barrel> emptybarrels;

    }
}
