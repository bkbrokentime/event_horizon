using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Skills;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel.Skills
{
    public class SkillTreeNode : MonoBehaviour
    {
		public enum NodeState
		{
			Disabled,
			Enabled,
			EnabledAndConnected,
		}

        public enum SkillClass
        {
            Mothership,
            Ship,
            Shield,
            EnergyShield,
            HangerSlot,
            Lock,
        }



        [Inject] private readonly ILocalization _localization;
		
        [SerializeField] SkillType _type;
        [SerializeField] SkillClass _class;

        [SerializeField] NodeColor _ndoecolor;
        [SerializeField] bool _freeposition;


        [SerializeField] int _multiplier = 1;
        [SerializeField] SkillTreeNode[] _linkedNodes;

        public bool _conditional;
        [SerializeField] SkillTreeNode[] _requireNodes;
        public bool _hide;

        [SerializeField] UiLine _linkPrefab;
        [SerializeField] Image _icon;
        [SerializeField] float _sizeScale = 0.85f;

        [SerializeField] Image _icon_lock;


        private Color _lockedColor;
        private Color _unlockedColor;
        private Color _lockedIconColor;
        private Color _unlockedIconColor;

        public float Size { get { return _sizeScale*GetComponent<RectTransform>().rect.width/2f; } }
        public IEnumerable<SkillTreeNode> LinkedNodes { get { return _linkedNodes; } }
		public string Name { get { return _type.GetName(_localization); } }
		public string Description { get { return _type.GetDescription(_localization, _multiplier); } }
        public SkillType Type { get { return _type; } }
        public int Multiplier { get { return _multiplier; } }

		public NodeState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                _state = value;
                UpdateState();
                UpdateLinks();
            }
        }

        public void ValidateLinks()
        {
            var valid = Array.IndexOf(_linkedNodes, null) < 0;
            if (valid)
                return;

            _linkedNodes = _linkedNodes.Where(item => item != null).ToArray();
        }

        public void AddLink(SkillTreeNode node)
        {
            _linkedNodes = _linkedNodes.AddIfNotExists(node);
        }

        public void RemoveLink(SkillTreeNode node)
        {
            _linkedNodes = _linkedNodes.RemoveAll(node);
        }

        public void ClearLinks()
        {
            _linkedNodes = new SkillTreeNode[] {};
        }

        private void Start()
        {
            CreateLines();
            UpdateState();
            if (_hide && _state == NodeState.Disabled)
            {
                gameObject.SetActive(false);
                foreach(var item in _links)
                    item.Value.color = new Color(0, 0, 0, 0);
            }
            else
                gameObject.SetActive(true);
        }


        public void GetColor(NodeColor nodecolor)
        {
            _lockedColor = nodecolor._lockedColor[(int)_class];
            _unlockedColor = nodecolor._unlockedColor[(int)_class];
            _lockedIconColor = nodecolor._lockedIconColor[(int)_class];
            _unlockedIconColor = nodecolor._unlockedIconColor[(int)_class];
        }

        private void UpdateState()
        {

            GetColor(_ndoecolor);

            var image = GetComponent<Image>();
            if (image)
				image.color = _state == NodeState.Disabled ? _lockedColor : _unlockedColor;

            if (_icon_lock)
                _icon_lock.color = _state == NodeState.Disabled ? _ndoecolor._lockedimageColor : _ndoecolor._unlockedimageColor;

            if (_icon)
				_icon.color = _state == NodeState.Disabled ? _lockedIconColor : _unlockedIconColor;

            bool enable = true;
            if (_hide)
            {
                foreach (var item in _requireNodes)
                {
                    if (item._state == NodeState.Disabled)
                    {
                        enable = false;
                        break;
                    }
                }
                if(!enable)
                {
                    if (image)
                        image.color = new Color(0, 0, 0, 0);

                    if (_icon_lock)
                        _icon_lock.color = new Color(0, 0, 0, 0);

                    if (_icon)
                        _icon.color = new Color(0, 0, 0, 0);
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                    //_hide = false;
                    //UpdateHideLinks();
                }
            }
        }

        private void CreateLines()
        {
            foreach (var item in _linkedNodes)
            {
                if (_links.ContainsKey(item))
                    continue;

                var link = CreateLink(item);
                AddLink(item, link);
                item.AddLink(this, link);
                if (item._hide)
                    link.color = new Color(0, 0, 0, 0);
            }
        }

        private void UpdateLinks()
        {
            foreach (var link in _links)
            {
                if (!link.Key._hide && !_hide)
                    link.Value.color = IsLinkEnabled(link.Key) ? _ndoecolor._unlockedLinkLineColor : _ndoecolor._lockedLinkLineColor;
                else
                    link.Value.color = new Color(0, 0, 0, 0);
                link.Key.UpdateState();
            }
        }

        public void ResetLinks()
        {
            UpdateLinks();
        }
        private void UpdateHideLinks()
        {
            foreach (var link in _links)
            {
                link.Value.color = IsLinkEnabled(link.Key) ? _ndoecolor._unlockedLinkLineColor : _ndoecolor._lockedLinkLineColor;
            }
        }

        private void OnValidate()
        {
            if (_icon != null)
                _icon.sprite = CommonSpriteTable.SkillIcon(_type);
            if (_type != SkillType.Undefined)
                name = _type.ToString();

            Transform[] Transform_ = gameObject.GetComponentsInChildren<Transform>();
            foreach(Transform obj in Transform_)
            {
                if (obj.gameObject.name == "Icon_lock")
                    _icon_lock = obj.gameObject.GetComponent<Image>();
            }

            _icon_lock.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            _icon_lock.gameObject.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
            _icon_lock.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1);
            _icon_lock.gameObject.GetComponent<Image>().sprite = CommonSpriteTable.Lock_Icon;

            var position = gameObject.GetComponent<RectTransform>().anchoredPosition3D;

            if (!_freeposition)
            {
                position.x = position.x / 64 - Mathf.FloorToInt(position.x / 64) < 0.5 ? Mathf.FloorToInt(position.x / 64) * 64 : Mathf.FloorToInt(position.x / 64) * 64 + 64;
                position.y = position.y / 64 - Mathf.FloorToInt(position.y / 64) < 0.5 ? Mathf.FloorToInt(position.y / 64) * 64 : Mathf.FloorToInt(position.y / 64) * 64 + 64;
                gameObject.GetComponent<RectTransform>().anchoredPosition3D = position;
            }
        }

        private void AddLink(SkillTreeNode node, UiLine link)
        {
            _links.Add(node, link);
        }

        private bool IsLinkEnabled(SkillTreeNode targetNode)
        {
			return _state == NodeState.EnabledAndConnected || targetNode._state == NodeState.EnabledAndConnected || _state == NodeState.Enabled && targetNode._state == NodeState.Enabled;
        }

        private UiLine CreateLink(SkillTreeNode node)
        {
            var link = Instantiate(_linkPrefab);
            link.transform.SetParent(transform.parent);
            link.transform.localScale = Vector3.one;

            //GetColor(_ndoecolor);
            link.color = IsLinkEnabled(node) ? _ndoecolor._unlockedLinkLineColor : _ndoecolor._lockedLinkLineColor;

            var begin = transform.localPosition;
            var end = node.transform.localPosition;
            var dir = (end - begin).normalized;

            link.SetPoints(begin + dir*Size, end - dir*node.Size);

            return link;
        }

        public bool Conditional()
        {
            if (!_conditional && !_hide)
                return true;

            foreach(var node in _requireNodes)
            {
                if (node.State == NodeState.Disabled)
                    return false;
            }

            return true;
        }


        private NodeState _state = NodeState.Disabled;
        private readonly Dictionary<SkillTreeNode, UiLine> _links = new Dictionary<SkillTreeNode, UiLine>();
    }
}
