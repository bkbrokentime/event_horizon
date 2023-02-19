using Combat.Manager;
using GameServices;
using Gui.Windows;
using Services.Gui;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Combat
{
    public class CombatMenu : MonoBehaviour
    {
        [SerializeField] private Button _nextEnemyButton;
        [SerializeField] private Button _nextAllyButton;
        [SerializeField] private Button _changeShipButton;

        [SerializeField] private Button _hideradarmap;
        [SerializeField] private Button _openradarmap;

        [SerializeField] private RadarMapPanel _radarmap;

        [Inject]
        private void Initialize(IMessenger messenger, CombatManager manager)
        {
            _manager = manager;
            messenger.AddListener<int>(EventType.EnemyShipCountChanged, OnEnemyShipCountChanged);
            messenger.AddListener<int>(EventType.PlayerShipCountChanged, OnAllyShipCountChanged);
            UpdateRadarMap();
        }

        public void Open()
        {
            GetComponent<IWindow>().Open();
        }

        public void InitializeWindow()
        {
            _nextEnemyButton.gameObject.SetActive(_manager.CanCallNextEnemy());
            _nextEnemyButton.interactable = true;
            _nextAllyButton.gameObject.SetActive(_manager.CanCallNextAlly());
            _nextAllyButton.interactable = true;
            _changeShipButton.gameObject.SetActive(_manager.CanChangeShip());
        }

        public void ExitButtonClicked()
        {
            _manager.Surrender();
        }

        public void NextEnemyButtonClicled()
        {
            _manager.CallNextEnemy();
        }
        public void NextAllyButtonClicled()
        {
            _manager.CallNextAlly();
        }

        public void ChangeShipButtonClicked()
        {
            _manager.ChangeShip();
        }

        public void KillThemAll()
        {
            _manager.KillAllEnemies();
        }

        public void HideRaderMap()
        {
            _radarmap.Close();
            _radarmap.IsHide = true;
            UpdateRadarMap();
        }

        public void OpenRaderMap()
        {
            _radarmap.Open();
            _radarmap.IsHide = false;
            UpdateRadarMap();
        }

        public void UpdateRadarMap()
        {
            _hideradarmap.gameObject.SetActive(!_radarmap.IsHide);
            _openradarmap.gameObject.SetActive(_radarmap.IsHide);
        }

        private void OnEnemyShipCountChanged(int count)
        {
            if (!gameObject.activeSelf)
                return;
            
            _nextEnemyButton.interactable = _manager.CanCallNextEnemy();
        }
        private void OnAllyShipCountChanged(int count)
        {
            if (!gameObject.activeSelf)
                return;

            _nextAllyButton.interactable = _manager.CanCallNextAlly();
        }

        private CombatManager _manager;
    }
}
