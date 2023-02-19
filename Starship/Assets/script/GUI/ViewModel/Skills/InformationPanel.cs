using GameServices.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ViewModel.Skills
{
    public class InformationPanel : MonoBehaviour
    {

        [SerializeField] Text _nameText;
		[SerializeField] Text _descriptionText;
		[SerializeField] Text _unlockAllnameText;
        [SerializeField] Button _unlockButton;
        [SerializeField] Button _unlockAllButton;
		[SerializeField] GameObject _lockedPanel;
		[SerializeField] GameObject _unlockedPanel;
		[SerializeField] GameObject _unlockedAllPanel;

		public void Cleanup()
		{
			if (_defaultText != null)
				_nameText.text = _defaultText;
			if (_defaultText2 != null)
                _unlockAllnameText.text = _defaultText2;

			_descriptionText.gameObject.SetActive(false);

			_unlockButton.gameObject.SetActive(true);
			_unlockButton.interactable = false;
            _unlockAllButton.gameObject.SetActive(false);
            _unlockAllButton.interactable = false;

			_unlockedPanel.gameObject.SetActive(false);
			_lockedPanel.gameObject.SetActive(false);
            _unlockedAllPanel.gameObject.SetActive(false);
		}

		public void Initialize(SkillTreeNode node, bool available, bool unlocked, bool canallunlocked)
        {
            if (_defaultText == null)
                _defaultText = _nameText.text;
            if (_defaultText2 == null)
                _defaultText2 = _unlockAllnameText.text;
			
			_descriptionText.gameObject.SetActive(true);
			_nameText.text = node.Name;
			_descriptionText.text = node.Description;
			_unlockButton.gameObject.SetActive(!unlocked && available);
			_unlockButton.interactable = available;
			_lockedPanel.gameObject.SetActive(!unlocked && !available);
			_unlockedPanel.gameObject.SetActive(unlocked);

            _unlockAllButton.gameObject.SetActive(canallunlocked);
            _unlockAllButton.interactable = canallunlocked;

            _unlockedAllPanel.gameObject.SetActive(canallunlocked);
        }

        private string _defaultText;
        private string _defaultText2;
    }
}
