using Combat.Component.Ship;
using Combat.Unit;
using Gui.Controls;
using Gui.Windows;
using Services.Gui;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using ViewModel;
using Zenject;

namespace Gui.Combat
{
    [RequireComponent(typeof(AnimatedWindow))]
    public class ShipStatsPanel : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private ProgressBar _armorPoints;
        [SerializeField] private ProgressBar _shieldPoints;
        [SerializeField] private ProgressBar _energyPoints;
        [SerializeField] private ProgressBar _energyshieldPoints;

        [SerializeField] private ProgressBar _nonepoints_1;
        [SerializeField] private ProgressBar _nonepoints_2;

        [SerializeField] private Image _icon;
        [SerializeField] private SelectShipPanelItemViewModel _shipItem;

        [SerializeField] private GameObject _fireResistIcon;
        [SerializeField] private GameObject _energyResistIcon;
        [SerializeField] private GameObject _kineticResistIcon;
        [SerializeField] private GameObject _quantumResistIcon;

        [SerializeField] private Text _fireResistText;
        [SerializeField] private Text _energyResistText;
        [SerializeField] private Text _kineticResistText;
        [SerializeField] private Text _quantumResistText;

        [SerializeField] private Text _shieldfireResistText;
        [SerializeField] private Text _shieldenergyResistText;
        [SerializeField] private Text _shieldkineticResistText;
        [SerializeField] private Text _shieldquantumResistText;

        [SerializeField] private Text _energyshieldfireResistText;
        [SerializeField] private Text _energyshieldenergyResistText;
        [SerializeField] private Text _energyshieldkineticResistText;
        [SerializeField] private Text _energyshieldquantumResistText;


        public void Close()
        {
            GetComponent<AnimatedWindow>().Close(WindowExitCode.Ok);
        }

        public void Open(IShip ship)
        {
            if (!ship.IsActive())
                return;

            GetComponent<AnimatedWindow>().Open();

            if (_ship == ship)
                return;

            _ship = ship;

            if (_icon)
                _icon.sprite = _resourceLocator.GetSprite(ship.Specification.Stats.IconImage) ?? _resourceLocator.GetSprite(ship.Specification.Stats.ModelImage);

            _shipItem.SetLevel(ship.Specification.Type.Level);
            _shipItem.SetEnhanceLevel(ship.Specification.Type.Enhance);
            _shipItem.SetClass(ship.Specification.Type.Class);

            UpdateResistance();

            _hasShield = _ship.Stats.Shield.Exists;
            _hasEnergyShield = _ship.Stats.EnergyShield.Exists;
            _hasArmor = _ship.Stats.Armor.Exists;

            _shieldPoints.gameObject.SetActive(_hasShield);
            _energyshieldPoints.gameObject.SetActive(_hasEnergyShield);
            
            _nonepoints_1.gameObject.SetActive(!_hasShield);
            _nonepoints_2.gameObject.SetActive(!_hasEnergyShield);


            _armorPoints.gameObject.SetActive(_hasArmor);
        }

        private void UpdateResistance()
        {
            var resistance = _ship.Stats.Resistance;
            var weaponupgrade = _ship.Stats.WeaponUpgrade;

            if (_fireResistIcon != null)
            {
                var active = resistance.Heat >= -100 && resistance.Heat != 0;
                _fireResistText.gameObject.SetActive(active);
                if (active)
                    _fireResistText.text = Mathf.RoundToInt(resistance.Heat * 100) + "%";

                var active_s = resistance.ShieldHeat >= -100 && resistance.ShieldHeat != 0;
                _shieldfireResistText.gameObject.SetActive(active_s);
                if (active_s)
                    _shieldfireResistText.text = Mathf.RoundToInt(resistance.ShieldHeat * 100) + "%";

                var active_es = resistance.EnergyShieldHeat >= -100 && resistance.EnergyShieldHeat != 0;
                _energyshieldfireResistText.gameObject.SetActive(active_es);
                if (active_es)
                    _energyshieldfireResistText.text = Mathf.RoundToInt(resistance.EnergyShieldHeat * 100) + "%";

                //if (_fireResistIcon.gameObject.activeInHierarchy != true)
                    _fireResistIcon.gameObject.SetActive(active || active_s || active_es);
            }

            if (_energyResistIcon != null)
            {
                var active = resistance.Energy >= -100 && resistance.Energy != 0;
                _energyResistText.gameObject.SetActive(active);
                if (active)
                    _energyResistText.text = Mathf.RoundToInt(resistance.Energy * 100) + "%";

                var active_s = resistance.ShieldEnergy >= -100 && resistance.ShieldEnergy != 0;
                _shieldenergyResistText.gameObject.SetActive(active_s);
                if (active_s)
                    _shieldenergyResistText.text = Mathf.RoundToInt(resistance.ShieldEnergy * 100) + "%";

                var active_es = resistance.EnergyShieldEnergy >= -100 && resistance.EnergyShieldEnergy != 0;
                _energyshieldenergyResistText.gameObject.SetActive(active_es);
                if (active_es)
                    _energyshieldenergyResistText.text = Mathf.RoundToInt(resistance.EnergyShieldEnergy * 100) + "%";

                //if (_energyResistIcon.gameObject.activeInHierarchy != true)
                    _energyResistIcon.gameObject.SetActive(active || active_s || active_es);
            }

            if (_kineticResistIcon != null)
            {
                var active = resistance.Kinetic >= -100 && resistance.Kinetic != 0;
                _kineticResistText.gameObject.SetActive(active);
                if (active)
                    _kineticResistText.text = Mathf.RoundToInt(resistance.Kinetic * 100) + "%";

                var active_s = resistance.ShieldKinetic >= -100 && resistance.ShieldKinetic != 0;
                _shieldkineticResistText.gameObject.SetActive(active_s);
                if (active_s)
                    _shieldkineticResistText.text = Mathf.RoundToInt(resistance.ShieldKinetic * 100) + "%";

                var active_es = resistance.EnergyShieldKinetic >= -100 && resistance.EnergyShieldKinetic != 0;
                _energyshieldkineticResistText.gameObject.SetActive(active_es);
                if (active_es)
                    _energyshieldkineticResistText.text = Mathf.RoundToInt(resistance.EnergyShieldKinetic * 100) + "%";

                //if (_kineticResistIcon.gameObject.activeInHierarchy != true)
                    _kineticResistIcon.gameObject.SetActive(active || active_s || active_es);
            }

            if (_quantumResistIcon != null)
            {
                var active = resistance.Quantum >= -100 && resistance.Quantum != 0;
                _quantumResistText.gameObject.SetActive(active);
                if (active)
                    _quantumResistText.text = Mathf.RoundToInt(resistance.Quantum * 100) + "%";

                var active_s = resistance.ShieldQuantum >= -100 && resistance.ShieldQuantum != 0;
                _shieldquantumResistText.gameObject.SetActive(active_s);
                if (active_s)
                    _shieldquantumResistText.text = Mathf.RoundToInt(resistance.ShieldQuantum * 100) + "%";

                var active_es = resistance.EnergyShieldQuantum >= -100 && resistance.EnergyShieldQuantum != 0;
                _energyshieldquantumResistText.gameObject.SetActive(active_es);
                if (active_es)
                    _energyshieldquantumResistText.text = Mathf.RoundToInt(resistance.EnergyShieldQuantum * 100) + "%";

                //if (_quantumResistIcon.gameObject.activeInHierarchy != true)
                    _quantumResistIcon.gameObject.SetActive(active || active_s || active_es);
            }


        }

        private void Update()
        {
            if (!_ship.IsActive())
            {
                Close();
                return;
            }

            _updateResistanceCooldown -= Time.deltaTime;
            if (_updateResistanceCooldown <= 0)
            {
                _updateResistanceCooldown = 0.5f;
                UpdateResistance();
            }
/*
            var total = 0f;
            if (_hasArmor) total += _ship.Stats.Armor.MaxValue;
            if (_hasShield) total += _ship.Stats.Shield.MaxValue;
*/

            var armor = _hasArmor ? _ship.Stats.Armor.Value : 0;
            var shield = _hasShield ? _ship.Stats.Shield.Value : 0;
            var energyshield = _hasEnergyShield ? _ship.Stats.EnergyShield.Value : 0;

            if (_hasArmor)
            {
                _armorPoints.Y0 = 0;
                _armorPoints.Y1 = armor / _ship.Stats.Armor.MaxValue;
                _armorPoints.SetAllDirty();
            }
            if (_hasShield)
            {
                _shieldPoints.Y0 = 0;
                _shieldPoints.Y1 = shield / _ship.Stats.Shield.MaxValue;
                _shieldPoints.SetAllDirty();
            }
            if (_hasEnergyShield)
            {
                _energyshieldPoints.Y0 = 0;
                _energyshieldPoints.Y1 = energyshield / _ship.Stats.EnergyShield.MaxValue;
                _energyshieldPoints.SetAllDirty();
            }

            var energy = _ship.Stats.Energy.Percentage;
            if (!Mathf.Approximately(_energyPoints.Y1, energy))
            {
                _energyPoints.Y1 = energy;
                _energyPoints.SetAllDirty();
            }
        }

        private float _updateResistanceCooldown;
        private bool _hasShield;
        private bool _hasEnergyShield;
        private bool _hasArmor;
        private IShip _ship;
    }
}
