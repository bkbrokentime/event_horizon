using System.Linq;
using Combat.Ai;
using Galaxy;
using Galaxy.StarContent;
using GameDatabase.Enums;
using Services.Localization;
using Services.Messenger;
using Services.Reources;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Star = Galaxy.Star;


namespace Gui.StarMap
{
    public class StarSystemObjectItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;

        [SerializeField] private Sprite SpaceStationIcon;
        [SerializeField] private Sprite StarBaseIcon;
        [SerializeField] private Sprite BeaconIcon;
        [SerializeField] private Sprite PandemicIcon;
        [SerializeField] private Color PandemicIconColor;
        [SerializeField] private Sprite WormholeIcon;
        [SerializeField] private Color WormholeIconColor;

        public void Initialize(Galaxy.Star star, StarObjectType objectType, IMessenger messenger, ILocalization localization, IResourceLocator resourceLocator)
        {
            _messenger = messenger;
            _objectType = objectType;
            if (objectType == StarObjectType.Boss)
            {
                _name.text = localization.GetString("$Object" + StarObjectType.Boss);
                _name.text += "  x " + star.boss_level;
            }
            else
                _name.text = localization.GetString("$Object" + objectType);
            _icon.color = Color.white;

            switch (objectType)
            {
                case StarObjectType.Event:
                    _icon.sprite = BeaconIcon;
                    break;
                case StarObjectType.StarBase:
                    _icon.sprite = StarBaseIcon;
                    _icon.color = star.Region.Faction.Color;
                    break;
                case StarObjectType.Hive:
                    _icon.sprite = PandemicIcon;
                    _icon.color = PandemicIconColor;
                    break;
                case StarObjectType.Boss:
                    var ship = star.Boss.CreateFleet().Ships.Where(item => item.Model.Category == ShipCategory.Flagship).First();
                    //if(star.boss_level==1)
                    _icon.sprite = resourceLocator.GetSprite(ship.Model.IconImage) ?? resourceLocator.GetSprite(ship.Model.ModelImage);
                    break;
                //case StarObjectType.Boss2:
                //    var ship2 = star.Boss2.CreateFleet().Ships.First();
                //    _icon.sprite = resourceLocator.GetSprite(ship2.Model.IconImage) ?? resourceLocator.GetSprite(ship2.Model.ModelImage);
                //    break;
                //case StarObjectType.Boss3:
                //    var ship3 = star.Boss3.CreateFleet().Ships.First();
                //    _icon.sprite = resourceLocator.GetSprite(ship3.Model.IconImage) ?? resourceLocator.GetSprite(ship3.Model.ModelImage);
                //    break;
                case StarObjectType.Wormhole:
                    _icon.sprite = WormholeIcon;
                    _icon.color = WormholeIconColor;
                    break;
                default:
                    _icon.sprite = SpaceStationIcon;
                    break;
            }
        }

        public void OnButtonClicked()
        {
            UnityEngine.Debug.Log("starsystemobj.OnButtonClicked");
            if (_objectType == StarObjectType.Boss)
                _messenger.Broadcast<StarObjectType>(EventType.ArrivedToObject, StarObjectType.Boss);
            else
                _messenger.Broadcast<StarObjectType>(EventType.ArrivedToObject, _objectType);

        }

        private StarObjectType _objectType;
        private IMessenger _messenger;
    }
}
