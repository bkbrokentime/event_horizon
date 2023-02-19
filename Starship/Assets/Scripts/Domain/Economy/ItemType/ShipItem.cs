using System.Text;
using GameServices.Player;
using Services.Localization;
using Constructor.Ships;
using Services.Reources;
using UnityEngine;
using Zenject;
using System.Linq;

namespace Economy.ItemType
{
    public class ShipItem : IItemType
    {
        [Inject]
        public ShipItem(PlayerFleet playerFleet, ILocalization localization, IShip ship, bool premium = false)
        {
            _localization = localization;
            _playerFleet = playerFleet;
            _ship = ship;
            _premium = premium;
        }

        public int Rank { get { return _ship.Experience.Level; } }

        public string Id { get { return "sh" + _ship.Id; } }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = _localization.GetString(_ship.Name);

                return _name;
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    var sb = new StringBuilder();
                    sb.Append(_localization.GetString("$Level"));
                    sb.Append(_localization.GetString(" "));
                    sb.Append(_localization.GetString(_ship.Experience.Level.ToString()));
                    foreach (var mod in Ship.Model.Modifications)
                    {
                        sb.Append("\n");
                        sb.Append(mod.GetDescription(_localization));
                    }

                    _description = sb.ToString();
                }

                return _description;
            }
        }

        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(_ship.Model.ModelImage); }
        public Price Price 
        { get 
            {
                if (!_premium)
                    return Price.Common(_ship.Price());

                int mul = _ship.Model.Layout.CellCount + _ship.Model.SecondLayout.CellCount;
                switch(_ship.Model.Category)
                {
                    case GameDatabase.Enums.ShipCategory.Common:
                        mul *= 2;
                        break;
                    case GameDatabase.Enums.ShipCategory.Rare:
                        mul *= 3;
                        break;
                    case GameDatabase.Enums.ShipCategory.Special:
                    case GameDatabase.Enums.ShipCategory.Hidden:
                        mul *= 4;
                        break;
                    case GameDatabase.Enums.ShipCategory.Flagship:
                        mul *= 5;
                        break;
                    case GameDatabase.Enums.ShipCategory.SuperFlagship:
                        mul *= 15;
                        break;
                    case GameDatabase.Enums.ShipCategory.Starbase:
                        mul *= 8;
                        break;
                    case GameDatabase.Enums.ShipCategory.Drone:
                    default:
                        break; ;
                }
                return Price.Premium(mul / 5); 
            } 
        }
        public Color Color { get { return Color.white; } }
        public ItemQuality Quality { get { return _ship.Model.Quality(); } }

        public void Consume(int amount)
        {
            for (int i = 0; i < amount; ++i)
                _playerFleet.Ships.Add(new CommonShip(_ship.Model, _ship.Components) { Experience = _ship.Experience });
        }

        public void Withdraw(int amount)
        {
            var ship = _playerFleet.Ships.Where(item => item.Id == _ship.Id).First();
            if (ship != null)
            {
                Debug.Log("Find Ship");
                if (_playerFleet.RemoveShip(ship))
                    Debug.Log("Remove Ship Succeed");
                else
                    Debug.Log("Remove Ship Failed");
            }
            else
            {
                Debug.Log("Can not Find Ship");
            }
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return 0; } }

        public IShip Ship { get { return _ship; } }

        private string _name;
        private string _description;
        private readonly IShip _ship;
        private readonly bool _premium;
        private readonly PlayerFleet _playerFleet;
        private readonly ILocalization _localization;
    }
}
