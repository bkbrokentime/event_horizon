using Gui.Combat;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.Exploration
{
    public class CombatMenu : MonoBehaviour
    {
        [SerializeField] private Button _hideradarmap;
        [SerializeField] private Button _openradarmap;

        [SerializeField] private RadarMapPanel _radarmap;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void ExitButtonClicked()
        {
            _messenger.Broadcast(EventType.Surrender);
        }

        public void KillThemAll()
        {
            _messenger.Broadcast(EventType.KillAllEnemies);
        }
        public void HideRaderMap()
        {
            _radarmap.Close();
            _radarmap.IsHide = false;
            _hideradarmap.gameObject.SetActive(_radarmap.IsHide);
            _openradarmap.gameObject.SetActive(!_radarmap.IsHide);
        }

        public void OpenRaderMap()
        {
            _radarmap.Open();
            _radarmap.IsHide = true;
            _hideradarmap.gameObject.SetActive(_radarmap.IsHide);
            _openradarmap.gameObject.SetActive(!_radarmap.IsHide);
        }

        private IMessenger _messenger;
    }
}
