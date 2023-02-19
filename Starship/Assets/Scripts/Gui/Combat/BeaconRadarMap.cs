using UnityEngine;
using Combat.Component.Unit;
using Combat.Scene;
using Combat.Unit;
using UnityEngine.UI;

namespace Gui.Combat
{
    public class BeaconRadarMap : MonoBehaviour
    {
        [SerializeField] private Image Image;
        [SerializeField] private Image Background;

        [SerializeField] private Sprite Container;
        [SerializeField] private Sprite Meteorite;
        [SerializeField] private Sprite ShipWreck;
        [SerializeField] private Sprite Minerals;
        [SerializeField] private Sprite MineralsRare;
        [SerializeField] private Sprite Outpost;
        [SerializeField] private Sprite Hive;
        public void Open(IUnit unit, Game.Exploration.ObjectiveType type, IScene scene)
        {
            _scene = scene;
            _unit = unit;
            _type = type;

            Initialize();
            Update();
            gameObject.SetActive(true);
        }

        public IUnit Unit => _unit;

        private void Update()
        {
            if (!_unit.IsActive())
            {
                Close();
                return;
            }

            var itemPosition = _unit.Body.Position;

            var playerposition = _scene.ViewPoint;

            var offset = itemPosition - playerposition;
            var x = offset.x / _scene.Settings.AreaWidth;
            var y = offset.y / _scene.Settings.AreaHeight;
            offset.x = x * _screenSize.x * 0.9f;
            offset.y = y * _screenSize.y * 0.9f;
            Image.enabled = true;
            Background.enabled = true;

            this.RectTransform.localPosition = offset;

            RectTransform.localScale = Vector3.one;
        }

        public void Close()
        {
            _unit = null;

            if (this)
                gameObject.SetActive(false);
        }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private void Initialize()
        {
            _screenSize = RectTransform.parent.GetComponent<RectTransform>().rect.size;
            switch (_type)
            {
                case Game.Exploration.ObjectiveType.Container:
                    {
                        Image.sprite = Container;
                    }
                    break;
                case Game.Exploration.ObjectiveType.Meteorite:
                    {
                        Image.sprite = Meteorite;
                    }
                    break;
                case Game.Exploration.ObjectiveType.ShipWreck:
                    {
                        Image.sprite = ShipWreck;
                    }
                    break;
                case Game.Exploration.ObjectiveType.Minerals:
                    {
                        Image.sprite = Minerals;
                    }
                    break;
                case Game.Exploration.ObjectiveType.MineralsRare:
                    {
                        Image.sprite = MineralsRare;
                    }
                    break;
                case Game.Exploration.ObjectiveType.Outpost:
                    {
                        Image.sprite = Outpost;
                    }
                    break;
                case Game.Exploration.ObjectiveType.Hive:
                    {
                        Image.sprite = Hive;
                    }
                    break;

            }
        }

        private Vector2 _screenSize;
        private RectTransform _rectTransform;
        private IUnit _unit;
        private Game.Exploration.ObjectiveType _type;
        private IScene _scene;
    }
}
