using UnityEngine;
using GameServices.Player;
using Zenject;
using UnityEngine.UI;
using System.Linq;
using Services.Reources;

namespace ViewModel
{
	public class BossPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public RectTransform[] CompletedOnlyControls;
		public RectTransform[] NotCompletedOnlyControls;

		public Image Icon;

		public int boss_level;
		private void OnEnable()
		{
			//boss_level = _motherShip.CurrentStar.boss_level;

			//if (boss_level == 1)
			//{
				var completed = _motherShip.CurrentStar.Boss.IsDefeated;

				foreach (var control in CompletedOnlyControls)
					control.gameObject.SetActive(completed);
				foreach (var control in NotCompletedOnlyControls)
					control.gameObject.SetActive(!completed);

			Icon.sprite = _resourceLocator.GetSprite(_motherShip.CurrentStar.Boss.CreateFleet().Ships.Where(item => item.Model.Category == GameDatabase.Enums.ShipCategory.Flagship).First().Model.IconImage) ?? _resourceLocator.GetSprite(_motherShip.CurrentStar.Boss.CreateFleet().Ships.Where(item => item.Model.Category == GameDatabase.Enums.ShipCategory.Flagship).First().Model.ModelImage);
            //}
            //else if (boss_level == 2)
            //{
            //	var completed2 = _motherShip.CurrentStar.Boss2.IsDefeated;

            //	foreach (var control in CompletedOnlyControls)
            //		control.gameObject.SetActive(completed2);
            //	foreach (var control in NotCompletedOnlyControls)
            //		control.gameObject.SetActive(!completed2);
            //}
            //else if (boss_level == 3)
            //{
            //	var completed3 = _motherShip.CurrentStar.Boss3.IsDefeated;

            //	foreach (var control in CompletedOnlyControls)
            //		control.gameObject.SetActive(completed3);
            //	foreach (var control in NotCompletedOnlyControls)
            //		control.gameObject.SetActive(!completed3);
            //}
        }

		public void StartButtonClicked()
		{
			UnityEngine.Debug.Log("BossPanelViewModel.StartButtonClicked");
			boss_level = _motherShip.CurrentStar.boss_level;

			//if (boss_level == 1)
			//{
				_motherShip.CurrentStar.Boss.Attack();
			//}
			//else if (boss_level == 2)
			//{
			//	_motherShip.CurrentStar.Boss2.Attack();
			//}
			//else if (boss_level == 3)
			//{
			//	_motherShip.CurrentStar.Boss3.Attack();
			//}
		}
	}
}
