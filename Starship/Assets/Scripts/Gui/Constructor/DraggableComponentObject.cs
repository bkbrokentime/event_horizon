using System;
using Constructor;
using DebugLogSetting;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ViewModel;
using Zenject;

namespace Gui.Constructor
{
    public class DraggableComponentObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private ComponentIconViewModel _icon;
        [SerializeField] private ShipLayoutViewModel _shipLayout;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_left_1;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_right_1;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_left_2;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_right_2;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_left_3;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_right_3;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_left_4;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_right_4;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_left_5;
        [SerializeField] private ShipLayoutViewModel PlatformLayout_right_5;

        [SerializeField] private ShipLayoutViewModel Second_shipLayout;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_left_1;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_right_1;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_left_2;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_right_2;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_left_3;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_right_3;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_left_4;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_right_4;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_left_5;
        [SerializeField] private ShipLayoutViewModel Second_PlatformLayout_right_5;
        public int layoutnum;

        [SerializeField] private ConstructorViewModel.CommandEvent _onCommandExecutedEvent = new ConstructorViewModel.CommandEvent();

        [Serializable]
        public class PositionChangedEvent : UnityEvent<Vector2> { }

        public void Initialize(IntegratedComponent component, PointerEventData eventData, Vector2 blockSize, ICommand removeComponentCommand)
        {
            if (removeComponentCommand != null && !removeComponentCommand.TryExecute())
                return;

            _component = component;
            _removeComponentCommand = removeComponentCommand;

            gameObject.SetActive(true);
            var size = _component.Info.Data.Layout.Size * blockSize;
            RectTransform.position = eventData.position;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            if (component.Info.Data.GIFIcon)
            {
                Sprite[] spr = _resourceLocator.GetGIFSprite(component.Info.Data.Icon);

                if (OtherDebugLogSetting.GifIconDebugLog)
                    UnityEngine.Debug.Log("draggablespr = " + spr.Length);

                _icon.SetGIFIcon(spr, spr.Length, component.Info.Data.Layout.Data, component.Info.Data.Layout.Size, component.Info.Data.Color);
            }
            else
                _icon.SetIcon(_resourceLocator.GetSprite(component.Info.Data.Icon), component.Info.Data.Layout.Data, component.Info.Data.Layout.Size, component.Info.Data.Color);

            eventData.pointerDrag = gameObject;
            ExecuteEvents.Execute<IBeginDragHandler>(gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.position = eventData.position;
            var position = eventData.position;
            if (layoutnum == 0)
            {
                _shipLayout.PreviewComponent(position, _component.Info);
                PlatformLayout_left_1.PreviewComponent(position, _component.Info);
                PlatformLayout_right_1.PreviewComponent(position, _component.Info);
                PlatformLayout_left_2.PreviewComponent(position, _component.Info);
                PlatformLayout_right_2.PreviewComponent(position, _component.Info);
                PlatformLayout_left_3.PreviewComponent(position, _component.Info);
                PlatformLayout_right_3.PreviewComponent(position, _component.Info);
                PlatformLayout_left_4.PreviewComponent(position, _component.Info);
                PlatformLayout_right_4.PreviewComponent(position, _component.Info);
                PlatformLayout_left_5.PreviewComponent(position, _component.Info);
                PlatformLayout_right_5.PreviewComponent(position, _component.Info);
            }
            else if (layoutnum == 1)
            {
                Second_shipLayout.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_left_1.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_right_1.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_left_2.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_right_2.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_left_3.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_right_3.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_left_4.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_right_4.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_left_5.PreviewComponent(position, _component.Info);
                Second_PlatformLayout_right_5.PreviewComponent(position, _component.Info);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            gameObject.SetActive(false);
            var position = eventData.position;

            ICommand installCommand;
            if (layoutnum == 0)
            {
                if (!(installCommand = new InstallComponentCommand(_shipLayout, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                    if (!(installCommand = new InstallComponentCommand(PlatformLayout_left_1, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                        if (!(installCommand = new InstallComponentCommand(PlatformLayout_right_1, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                            if (!(installCommand = new InstallComponentCommand(PlatformLayout_left_2, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                if (!(installCommand = new InstallComponentCommand(PlatformLayout_right_2, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                    if (!(installCommand = new InstallComponentCommand(PlatformLayout_left_3, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                        if (!(installCommand = new InstallComponentCommand(PlatformLayout_right_3, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                            if (!(installCommand = new InstallComponentCommand(PlatformLayout_left_4, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                if (!(installCommand = new InstallComponentCommand(PlatformLayout_right_4, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                    if (!(installCommand = new InstallComponentCommand(PlatformLayout_left_5, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                        if (!(installCommand = new InstallComponentCommand(PlatformLayout_right_5, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                            installCommand = null;
            }
            else if (layoutnum == 1)
            {
                if (!(installCommand = new InstallComponentCommand(Second_shipLayout, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                    if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_left_1, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                        if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_right_1, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                            if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_left_2, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_right_2, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                    if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_left_3, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                        if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_right_3, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                            if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_left_4, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_right_4, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                    if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_left_5, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                        if (!(installCommand = new InstallComponentCommand(Second_PlatformLayout_right_5, layoutnum, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                                                            installCommand = null;
            }
            else
                installCommand = null;

            if (ComponentDebugLogSetting.ComponentConstructDebugLog)
            {
                if (installCommand != null)
                    UnityEngine.Debug.Log("layout:  " + layoutnum + "  installCommand != null");
                else
                    UnityEngine.Debug.Log("layout:  " + layoutnum + "  installCommand = null");
            }
            
            if (_removeComponentCommand != null || installCommand != null)
                _onCommandExecutedEvent.Invoke(new ComplexCommand(_removeComponentCommand, installCommand));

            _component = null;
            _removeComponentCommand = null;
        }


        private RectTransform RectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }

        private RectTransform _rectTransform;
        private IntegratedComponent _component;
        private ICommand _removeComponentCommand;
    }
}
