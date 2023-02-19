using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GameModel;

namespace ViewModel
{
	public class SelectShipPanelItemViewModel : ShipInfoViewModel
	{
		public RectTransform DisabledIcon;
		public RectTransform ActiveIcon;
		public Text ConditionText;
	}
}
