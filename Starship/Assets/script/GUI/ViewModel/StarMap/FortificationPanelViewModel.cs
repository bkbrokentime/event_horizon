using UnityEngine;
using GameServices.Player;
using Zenject;

namespace ViewModel
{
	public class FortificationPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;

		public RectTransform[] CompletedOnlyControls;
		public RectTransform[] NotCompletedOnlyControls;
		
		private void OnEnable()
		{
            var completed = _motherShip.CurrentStar.Fortification.IsDefeated;
            foreach (var control in CompletedOnlyControls)
                control.gameObject.SetActive(completed);
            foreach (var control in NotCompletedOnlyControls)
                control.gameObject.SetActive(!completed);
        }		
		
		public void StartButtonClicked()
		{
			UnityEngine.Debug.Log("RuinsPanelViewModel.StartButtonClicked");
            _motherShip.CurrentStar.Fortification.Attack();
		}
	}
}
