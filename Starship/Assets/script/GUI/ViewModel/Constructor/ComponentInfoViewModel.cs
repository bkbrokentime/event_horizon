using UnityEngine;
using UnityEngine.UI;
using Constructor;
using System.Linq;
using Constructor.Model;
using DataModel.Technology;
using Economy;
using GameDatabase.Enums;
using GameServices.Database;
using GameServices.Gui;
using GameServices.Player;
using GameServices.Research;
using Gui.Constructor;
using Services.Localization;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class ComponentInfoViewModel : MonoBehaviour
	{
		public enum Status
		{
			Ok,
			AlreadyInstalled,
			NotSuitable,
		};

	    [InjectOptional] private readonly PlayerResources _playerResources;
	    [InjectOptional] private readonly Research _research;
	    [InjectOptional] private readonly ITechnologies _technologies;
	    [Inject] private readonly GuiHelper _guiHelper;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField]
        private ConstructorViewModel.CommandEvent _onCommandExecutedEvent = new ConstructorViewModel.CommandEvent();

        public ComponentViewModel ComponentViewModel;
		public DragAndDropComponent ComponentPanel;
		public ShipLayoutViewModel ShipLayout;
		public ShipLayoutViewModel Second_ShipLayout;
        //public ShipLayoutViewModel LeftPlatformLayout;
        //public ShipLayoutViewModel RightPlatformLayout;
        public ShipLayoutViewModel PlatformLayoutViewModel_Left_1;
        public ShipLayoutViewModel PlatformLayoutViewModel_Right_1;
        public ShipLayoutViewModel PlatformLayoutViewModel_Left_2;
        public ShipLayoutViewModel PlatformLayoutViewModel_Right_2;
        public ShipLayoutViewModel PlatformLayoutViewModel_Left_3;
        public ShipLayoutViewModel PlatformLayoutViewModel_Right_3;
        public ShipLayoutViewModel PlatformLayoutViewModel_Left_4;
        public ShipLayoutViewModel PlatformLayoutViewModel_Right_4;
        public ShipLayoutViewModel PlatformLayoutViewModel_Left_5;
        public ShipLayoutViewModel PlatformLayoutViewModel_Right_5;

        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_1;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_1;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_2;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_2;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_3;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_3;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_4;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_4;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Left_5;
        public ShipLayoutViewModel Second_PlatformLayoutViewModel_Right_5;
        public ConstructorViewModel ConstructorViewModel;
		public GameObject InstallPanel;
		public GameObject InstallLabel;
		public GameObject AlreadyInstalledLabel;
		public GameObject NotSuitableLabel;
		public Button DeleteButton;
		public Button UnlockButton;
        public Button UnlockAllButton;
        public ControlsPanel Controls;
		public GameObject[] ActiveOnlyObjects;

		public Text SizeText;
		public Image RequiredCellIcon;
		public Text RequiredCellText;
		public Color[] ColorList;

		public int layoutnum;

		public void OnKeyBindingChanged()
		{
		    if (_componentId >= 0 && _activeLayout != null)
		    {
                var component = _activeLayout.Ship.GetComponent(_componentId);
                component.KeyBinding = Controls.KeyBinding;
                component.Behaviour = Controls.ComponentMode;
            }
		}

		public void OnComponentPositionChanged(Vector2 position)
		{
			if (layoutnum == 0)
			{
				ShipLayout.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Left_1.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Right_1.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Left_2.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Right_2.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Left_3.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Right_3.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Left_4.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Right_4.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Left_5.PreviewComponent(position, _component);
				PlatformLayoutViewModel_Right_5.PreviewComponent(position, _component);
			}
			else if (layoutnum == 1)
			{
				Second_ShipLayout.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Left_1.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Right_1.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Left_2.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Right_2.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Left_3.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Right_3.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Left_4.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Right_4.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Left_5.PreviewComponent(position, _component);
				Second_PlatformLayoutViewModel_Right_5.PreviewComponent(position, _component);
			}
		}

		public void OnComponentReleased(Vector2 position)
		{
			if (layoutnum == 0)
			{
				if (InstallComponentCommand.TryExecuteCommand(ShipLayout, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Left_1, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Right_1, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Left_2, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Right_2, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Left_3, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Right_3, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Left_4, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Right_4, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Left_5, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(PlatformLayoutViewModel_Right_5, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
			}
			else if (layoutnum == 1)
			{
				if (InstallComponentCommand.TryExecuteCommand(Second_ShipLayout, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Left_1, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Right_1, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Left_2, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Right_2, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Left_3, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Right_3, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Left_4, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Right_4, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Left_5, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
				if (InstallComponentCommand.TryExecuteCommand(Second_PlatformLayoutViewModel_Right_5, layoutnum, position, _component, Controls.KeyBinding, Controls.ComponentMode, _onCommandExecutedEvent))
					return;
			}
			else
				return;
		}
        
		public void Clear()
		{
			ComponentViewModel.Clear();
			ComponentPanel.Interactable = false;
			_componentId = -1;
			_activeLayout = null;
			Controls.Clear();

			foreach (var item in ActiveOnlyObjects)
				item.SetActive(false);
        }

		public void DeleteButtonPressed()
		{
			if (_activeLayout == null)
				throw new System.InvalidOperationException("layout not selected");

            var command = new RemoveComponentCommand(_activeLayout, _componentId);
            if (command.TryExecute())
                _onCommandExecutedEvent.Invoke(command);
		}

		public void UnlockButtonPressed()
		{
            if (_playerResources == null)
                return;

			if (_activeLayout == null)
				throw new System.InvalidOperationException("layout not selected");
			var component = _activeLayout.Ship.GetComponent(_componentId);
			var price = component.Info.Price*2;
            _guiHelper.ShowConfirmation(_localization.GetString("$UnlockConfirmation"), price, () => 
            {
                if (!CanBeUnlocked(component))
                    throw new System.InvalidOperationException("invalid component");

			    if (!price.TryWithdraw(_playerResources))
                    return;
				_activeLayout.UnlockComponent(_componentId);
				SetComponent(_activeLayout, _componentId);
			});
		}

        public void UnlockAllButtonPressed()
        {
            if (_playerResources == null)
                return;

            var lockedItems = ShipLayout.Components.
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
				Concat(Second_ShipLayout.Components).
				Concat(Second_PlatformLayoutViewModel_Left_1.Components).
				Concat(Second_PlatformLayoutViewModel_Right_1.Components).
				Concat(Second_PlatformLayoutViewModel_Left_2.Components).
				Concat(Second_PlatformLayoutViewModel_Right_2.Components).
				Concat(Second_PlatformLayoutViewModel_Left_3.Components).
				Concat(Second_PlatformLayoutViewModel_Right_3.Components).
				Concat(Second_PlatformLayoutViewModel_Left_4.Components).
				Concat(Second_PlatformLayoutViewModel_Right_4.Components).
				Concat(Second_PlatformLayoutViewModel_Left_5.Components).
				Concat(Second_PlatformLayoutViewModel_Right_5.Components).
				Where(CanBeUnlocked);

            var price = Price.Common(lockedItems.Sum(item => item.Info.Price.Amount*2));
            _guiHelper.ShowConfirmation(_localization.GetString("$UnlockAllConfirmation"), price, () => 
            {
                if (!price.TryWithdraw(_playerResources))
                    return;

                ShipLayout.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(ShipLayout.UnlockComponent);
                PlatformLayoutViewModel_Left_1.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Left_1.UnlockComponent);
                PlatformLayoutViewModel_Right_1.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Right_1.UnlockComponent);
                PlatformLayoutViewModel_Left_2.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Left_2.UnlockComponent);
                PlatformLayoutViewModel_Right_2.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Right_2.UnlockComponent);
                PlatformLayoutViewModel_Left_3.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Left_3.UnlockComponent);
                PlatformLayoutViewModel_Right_3.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Right_3.UnlockComponent);
                PlatformLayoutViewModel_Left_4.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Left_4.UnlockComponent);
                PlatformLayoutViewModel_Right_4.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Right_4.UnlockComponent);
                PlatformLayoutViewModel_Left_5.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Left_5.UnlockComponent);
                PlatformLayoutViewModel_Right_5.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 0).Select(item => item.Key).ToList().ForEach(PlatformLayoutViewModel_Right_5.UnlockComponent);

                Second_ShipLayout.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_ShipLayout.UnlockComponent);
                Second_PlatformLayoutViewModel_Left_1.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Left_1.UnlockComponent);
                Second_PlatformLayoutViewModel_Right_1.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Right_1.UnlockComponent);
                Second_PlatformLayoutViewModel_Left_2.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Left_2.UnlockComponent);
                Second_PlatformLayoutViewModel_Right_2.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Right_2.UnlockComponent);
                Second_PlatformLayoutViewModel_Left_3.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Left_3.UnlockComponent);
                Second_PlatformLayoutViewModel_Right_3.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Right_3.UnlockComponent);
                Second_PlatformLayoutViewModel_Left_4.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Left_4.UnlockComponent);
                Second_PlatformLayoutViewModel_Right_4.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Right_4.UnlockComponent);
                Second_PlatformLayoutViewModel_Left_5.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Left_5.UnlockComponent);
                Second_PlatformLayoutViewModel_Right_5.Layout?.ComponentsIndex.Where(item => CanBeUnlocked(item.Value) && item.Value.Layout == 1).Select(item => item.Key).ToList().ForEach(Second_PlatformLayoutViewModel_Right_5.UnlockComponent);

                SetComponent(_activeLayout, _componentId);
            });
        }

        public void SetComponent(ShipLayoutViewModel activeLayout, int id)
		{
			_activeLayout = activeLayout;
			var component = _activeLayout.Ship.GetComponent(id);
			_component = component.Info;
			_componentId = id;

			foreach (var item in ActiveOnlyObjects)
				item.SetActive(true);

			var componentInfo = _component.CreateComponent(ConstructorViewModel.ShipSize);
			Controls.Initialize(componentInfo, component.KeyBinding, ConstructorViewModel.GetDefaultKey(_component.Data.Id), component.Behaviour);

			InstallPanel.SetActive(false);
			DeleteButton.gameObject.SetActive(!component.Locked);
			UnlockButton.gameObject.SetActive(component.Locked);
            UnlockAllButton.gameObject.SetActive(component.Locked);

            UnlockButton.interactable = CanBeUnlocked(component);
            UnlockAllButton.interactable = UnlockButton.interactable;

            ComponentPanel.gameObject.SetActive(false);
			ComponentPanel.Interactable = false;

			UpdateDescription(componentInfo);
		}

	    private bool CanBeUnlocked(IntegratedComponent component)
	    {
	        if (!component.Locked)
	            return false;
	        if (component.Info.Data.Availability == Availability.Common)
	            return true;
	        if (component.Info.Data.Id.Value == 96) // Xmas bomb
	            return true;

            ITechnology tech;
            return _technologies == null || _research == null || _technologies.TryGetComponentTechnology(component.Info.Data, out tech) && _research.IsTechResearched(tech);
	    }
            
		public void SetComponent(ComponentInfo data)
		{
			_activeLayout = null;
			_component = data;
			_componentId = -1;

			foreach (var item in ActiveOnlyObjects)
				item.SetActive(true);

			var componentInfo = _component.CreateComponent(ConstructorViewModel.ShipSize);
			Controls.Initialize(componentInfo, -1, ConstructorViewModel.GetDefaultKey(data.Data.Id), 0);

			var status = Status.Ok;
			if (ConstructorViewModel.IsUniqueItemInstalled(_component.Data))
				status = Status.AlreadyInstalled;

			if (!componentInfo.IsSuitable(ConstructorViewModel.Ship.Model))
				status = Status.NotSuitable;

			InstallPanel.SetActive(true);
			InstallLabel.SetActive(status == Status.Ok);
			AlreadyInstalledLabel.SetActive(status == Status.AlreadyInstalled);
			NotSuitableLabel.SetActive(status == Status.NotSuitable);

			DeleteButton.gameObject.SetActive(false);
			UnlockButton.gameObject.SetActive(false);
			ComponentPanel.gameObject.SetActive(true);
			ComponentPanel.Interactable = status == Status.Ok;
			ComponentPanel.RectSize = _component.Data.Layout.Size*ShipLayout.BlockSize;
			if (_component.Data.GIFIcon)
			{
                Sprite[] spr = _resourceLocator.GetGIFSprite(_component.Data.Icon);

				//UnityEngine.Debug.Log("componentinfospr = " + spr.Length);

                ComponentPanel.GetComponent<ComponentIconViewModel>().SetGIFIcon(spr, spr.Length, _component.Data.Layout.Data, _component.Data.Layout.Size, _component.Data.Color);
			}
			else
				ComponentPanel.GetComponent<ComponentIconViewModel>().SetIcon(_resourceLocator.GetSprite(_component.Data.Icon), _component.Data.Layout.Data, _component.Data.Layout.Size, _component.Data.Color);

			UpdateDescription(componentInfo);
		}

		private void UpdateDescription(Constructor.Component.IComponent component)
		{
			ComponentViewModel.Initialize(_component, 0);

			switch (_component.Data.CellType)
			{
			case CellType.Empty:
				RequiredCellIcon.color = ColorList[0];
				break;
			case CellType.Weapon:
				RequiredCellIcon.color = ColorList[1];
				break;
			case CellType.Outer:
				RequiredCellIcon.color = ColorList[2];
				break;
			case CellType.Inner:
				RequiredCellIcon.color = ColorList[3];
				break;
			case CellType.InnerOuter:
				RequiredCellIcon.color = ColorList[4];
				break;
			case CellType.Special:
				RequiredCellIcon.color = ColorList[6];
				break;
			case CellType.UAVPlatform:
				RequiredCellIcon.color = ColorList[7];
				break;
			case CellType.InnerSpecial:
				RequiredCellIcon.color = ColorList[8];
				break;
			case CellType.OuterUAVPlatform:
				RequiredCellIcon.color = ColorList[9];
				break;
			}

			RequiredCellText.text = _component.Data.CellType == CellType.Weapon ? ((char)_component.Data.WeaponSlotType).ToString() : string.Empty;
			//RequiredCellText.text = _component.Data.CellType == CellType.Weapon ? _component.Data.WeaponSlotType.ToString() : string.Empty;

			var stats = new ShipEquipmentStats();
			component.UpdateStats(ref stats);
			
			SizeText.text = _component.Data.Layout.CellCount.ToString();
		}

		private void OnEnable()
		{
			Clear();
		}

		private ShipLayoutViewModel _activeLayout;
		private int _componentId = -1;
		private ComponentInfo _component;
	}
}
