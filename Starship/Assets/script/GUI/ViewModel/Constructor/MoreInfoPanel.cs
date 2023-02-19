using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Constructor;

namespace ViewModel
{
	public class MoreInfoPanel : MonoBehaviour
	{

		public void OnMoreInfoButtonClicked(bool isOn)
		{
			gameObject.SetActive(isOn);
		}

    }
}
