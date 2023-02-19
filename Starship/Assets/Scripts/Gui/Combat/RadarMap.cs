using UnityEngine;
using Combat.Component.Ship;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Combat.Unit;
using GameDatabase.Enums;
using Services.Reources;
using UnityEngine.UI;

namespace Gui.Combat
{
    public class RadarMap : MonoBehaviour
    {
        [SerializeField] private Image Background;
        [SerializeField] private Color PlayerColor;
        [SerializeField] private Color PlayerDroneColor;
        [SerializeField] private Color AllyColor;
        [SerializeField] private Color AllyDroneColor;
        [SerializeField] private Color EnemyDroneColor;
        [SerializeField] private Color NormalColor;
        [SerializeField] private Color BossColor;
        [SerializeField] private Color SuperBossColor;
        [SerializeField] private Color StarbaseColor;

        [SerializeField] private Sprite PlayerIcon;
        [SerializeField] private Sprite NormalIcon;
        [SerializeField] private Sprite BossIcon;
        [SerializeField] private Sprite StarbaseIcon;
        [SerializeField] private Sprite DroneIcon;



        public void Open(IShip ship, IScene scene, IResourceLocator resourceLocator)
        {
            _scene = scene;
            _ship = ship;

            Initialize(resourceLocator);
            Update();
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_ship.IsActive())
            {
                Close();
                return;
            }

            if (_ship.Stats.IsStealth)
            {
                gameObject.SetActive(false);
                return;
            }
            else
                gameObject.SetActive(true);

            var itemPosition = _ship.Body.Position;

            var playerposition = _scene.ViewPoint;

            var offset = itemPosition - playerposition;
            var x = offset.x / _scene.Settings.AreaWidth;
            var y = offset.y / _scene.Settings.AreaHeight;
            offset.x = x * _screenSize.x * 0.85f;
            offset.y = y * _screenSize.y * 0.85f;
            Background.enabled = true;

            this.RectTransform.localPosition = offset;

            var model = _ship.Specification.Stats;

            RectTransform.localScale =
                model.ShipCategory == ShipCategory.Starbase ? Vector3.one * (2f + _ship.Body.Scale / 20)
                : model.ShipCategory == ShipCategory.SuperFlagship ? Vector3.one * (1.5f + _ship.Body.Scale / 20)
                : model.ShipCategory == ShipCategory.Flagship ? Vector3.one * (1f + _ship.Body.Scale / 25)
                : model.ShipCategory == ShipCategory.Drone ? Vector3.one * (0.25f + _ship.Body.Scale / 25)
                : Vector3.one * (0.5f + _ship.Body.Scale / 20);

            if (_ship.Stats.SpaceJump)
            {
                _background.a += Time.deltaTime;
                if (_background.a >= 1f)
                    _background.a = 0.2f;
                Background.color = _background;
            }
            else
            {
                _background.a = 1f;
                Background.color = _background;
            }

        }

        public void Close()
        {
            _ship = null;

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

        private void Initialize(IResourceLocator resourceLocator)
        {
            var model = _ship.Specification.Stats;
            var isPlayer = _ship.Type.Side == UnitSide.Player;
            var isAlly = _ship.Type.Side.IsAlly(UnitSide.Player);
            var isdrone = _ship.Specification.isdrone;

            if (isPlayer)
            {
                if (model.ShipCategory == ShipCategory.Drone || isdrone)
                {
                    Background.sprite = DroneIcon;
                    Background.color = PlayerDroneColor;
                }
                else
                {
                    Background.sprite = PlayerIcon;
                    Background.color = PlayerColor;
                }
            }
            else
            {
                if (isdrone)
                {
                    Background.sprite = DroneIcon;
                    Background.color = isAlly ? AllyDroneColor : EnemyDroneColor;
                }
                else
                {
                    switch (model.ShipCategory)
                    {
                        case ShipCategory.Starbase:
                            Background.sprite = StarbaseIcon;
                            Background.color = StarbaseColor;
                            break;
                        case ShipCategory.Flagship:
                            Background.sprite = BossIcon;
                            Background.color = isAlly ? AllyColor : BossColor;
                            break;
                        case ShipCategory.SuperFlagship:
                            Background.sprite = BossIcon;
                            Background.color = isAlly ? AllyColor : SuperBossColor;
                            break;
                        case ShipCategory.Drone:
                            Background.sprite = DroneIcon;
                            Background.color = isAlly ? AllyDroneColor : EnemyDroneColor;
                            break;
                        default:
                            Background.sprite = NormalIcon;
                            Background.color = isAlly ? AllyColor : NormalColor;
                            break;
                    }
                }
            }
            _background = Background.color;
            _screenSize = RectTransform.parent.GetComponent<RectTransform>().rect.size;
        }

        private Color _background;
        private Vector2 _screenSize;
        private RectTransform _rectTransform;
        private IShip _ship;
        private IScene _scene;
    }
}
