using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Services.Localization;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gui.ComponentList
{
    class ComponentContentFiller : MonoBehaviour, IContentFiller
    {
        [Inject] private readonly GameObjectFactory _gameObjectFactory;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;

        [SerializeField] private ComponentListItemBase _itemPrefab;
        [SerializeField] private GroupListItem _groupPrefab;

        [SerializeField] private ItemSelectedEvent _itemSelectedEvent = new ItemSelectedEvent();

        [Serializable]
        public class ItemSelectedEvent : UnityEvent<ComponentInfo> {}

        private void Awake()
        {
            _itemPrefab.gameObject.SetActive(false);
            _groupPrefab.gameObject.SetActive(false);
        }

        public void InitializeItems(IComponentTreeNode node)
        {
            _node = node;
            _quantityProvider = node.QuantityProvider;
            _components.Clear();
            foreach (var item in node.Components)
                if (_quantityProvider.GetQuantity(item) > 0)
                    _components.Add(item);

            _components.Sort(
                (item1, item2) =>
                {
                    if (item1.Data.Id.Value != item2.Data.Id.Value)
                        return item1.Data.Id.Value.CompareTo(item2.Data.Id.Value);
                    else if (item1.ModificationType != item2.ModificationType)
                        return item1.ModificationType.CompareTo(item2.ModificationType);
                    else
                        return item1.ModificationQuality.CompareTo(item2.ModificationQuality);
                }
            );

            _nodes.Clear();

            for (var parent = node.Parent; parent != null; parent = parent.Parent)
                _nodes.Insert(0, parent);

            _nodes.AddRange(node.Nodes);
            if (_nodes.Count != 0)
                if (_nodes.FirstOrDefault().Components != null)
                    _nodes.Sort(
                        (item1, item2) =>
                        {
                            var component1 = item1.Components.FirstOrDefault();
                            var component2 = item2.Components.FirstOrDefault();
                            if (component1.Data.Id.Value != component2.Data.Id.Value)
                                return component1.Data.Id.Value.CompareTo(component2.Data.Id.Value);
                            else if (component1.ModificationType != component2.ModificationType)
                                return component1.ModificationType.CompareTo(component2.ModificationType);
                            else
                                return component1.ModificationQuality.CompareTo(component2.ModificationQuality);
                        }
                    );

            if (_components.IndexOf(SelectedItem) < 0)
                SelectedItem = ComponentInfo.Empty;
        }

        public GameObject GetListItem(int index, int itemType, GameObject obj)
        {
            if (obj == null)
            {
                obj = _gameObjectFactory.Create(itemType == 0 ? _groupPrefab.gameObject : _itemPrefab.gameObject);
            }

            if (itemType == 0)
            {
                var item = obj.GetComponent<GroupListItem>();
                UpdateItem(item, _nodes[index]);
            }
            else
            {
                var item = obj.GetComponent<ComponentListItemBase>();
                var component = _components[index - _nodes.Count];
                UpdateItem(item, component);
            }

            return obj;
        }

        public int GetItemCount() { return _nodes.Count + _components.Count; }
        public int GetItemType(int index) { return index < _nodes.Count ? 0 : 1; }

        public ComponentInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                _itemSelectedEvent.Invoke(SelectedItem);
            }
        }

        public void OnItemSelected(ComponentListItemBase item)
        {
            SelectedItem = item.Component;
        }

        private void UpdateItem(ComponentListItemBase item, ComponentInfo component)
        {
            item.gameObject.SetActive(true);
            item.Initialize(component, _quantityProvider.GetQuantity(component));
            item.Selected = component == SelectedItem;
        }

        private void UpdateItem(GroupListItem item, IComponentTreeNode node)
        {
            item.gameObject.SetActive(true);
            item.Initialize(node, _node);
        }


        private ComponentInfo _selectedItem;
        private IComponentTreeNode _node;
        private IComponentQuantityProvider _quantityProvider;
        private readonly List<IComponentTreeNode> _nodes = new List<IComponentTreeNode>();
        private readonly List<ComponentInfo> _components = new List<ComponentInfo>();

    }
}
