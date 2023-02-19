using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Services.Reources;
using Zenject;
using ViewModel;
using System.Linq;

namespace Gui.ComponentList
{
    public class GroupListItem : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;

        [SerializeField] private Image _icon;
        [SerializeField] private ComponentGIFIconViewModel _componentGIFIconViewModel;
        [SerializeField] private GameObject _expandIcon;
        [SerializeField] private GameObject _collapseIcon;
        [SerializeField] private Text _nameText;
        [SerializeField] private Selectable _button;

        public void Initialize(IComponentTreeNode node, IComponentTreeNode activeNode)
        {
            Node = node;
            _icon.sprite = _resourceLocator.GetSprite(node.Icon);
            if (_icon.sprite == null)
            {
                if (_componentGIFIconViewModel != null)
                {
                    var spr = _resourceLocator.GetGIFSprite(node.Icon);
                    _componentGIFIconViewModel.gif = true;
                    _componentGIFIconViewModel.icons = spr;
                    _icon.sprite = spr[0];
                }
            }
            else
            {
                if (_componentGIFIconViewModel != null)
                {
                    _componentGIFIconViewModel.gif = false;
                    _componentGIFIconViewModel.icons = new Sprite[0];
                }
            }

            _icon.color = node.Color;
            _nameText.text = _localization.GetString(node.Name);

            var isParent = node.IsParent(activeNode);
            _expandIcon.gameObject.SetActive(!isParent && node != activeNode);
            _collapseIcon.gameObject.SetActive(isParent);
            _button.interactable = node != activeNode;
        }

        public IComponentTreeNode Node { get; private set; }
    }
}
