using GameDatabase.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class ShipInfoViewModel : MonoBehaviour
	{
		public Toggle Toggle;
		public Button Button;
		public Image Icon;
		public Text LevelText;
		public Text NameText;
	    public Text ClassText;
	    public Text EnhanceLevelText;
		public RectTransform LevelPanel;
		public RectTransform EnhanceLevelPanel;
		public LayoutGroup ClassPanel;
        public Slider ConditionSlider;

		public void SetLevel(int level)
		{
			LevelText.text = level > 0 ? level.ToString() : "0";
			if (LevelPanel != null)
				LevelPanel.gameObject.SetActive(level > 0);
		}
		public void SetEnhanceLevel(EnhancementLevel level)
		{
			if (EnhanceLevelPanel != null)
				EnhanceLevelPanel.gameObject.SetActive(level > 0);
			else
				return;
			EnhanceLevelText.text = level > 0 ? ((int)level).ToString() : "0";
		}

		public void SetClass(DifficultyClass shipClass)
		{
			if (shipClass <= DifficultyClass.Default)
			{
				ClassPanel.gameObject.SetActive(false);
				return;
			}

			ClassPanel.gameObject.SetActive(true);
			int index = 0;

            var classtype = (int)shipClass / 5;
            var classtypenum = (int)shipClass % 5;

			foreach (Transform child in ClassPanel.transform)
			{
				var image = child.GetComponent<Image>();
				if (image == null)
					continue;

				image.gameObject.SetActive(index < (int)shipClass);
				if (shipClass > DifficultyClass.Class5 && shipClass < DifficultyClass.Class51)
					image.color = index < classtypenum ? Colors[classtype] : Colors[classtype - 1];
				else if (shipClass <= DifficultyClass.Class5)
					image.color = Colors[0];
				else if (shipClass >= DifficultyClass.Class51)
					image.color = Colors[10];
				index++;
			}

		}
        private Color[] Colors = new Color[] {
		new Color(0.25f,1f,0.25f,0.75f),
		new Color(0.625f,1f,0.25f,0.75f),
		new Color(1f,1f,0.25f,0.75f),
		new Color(1f,0.625f,0.25f,0.75f),
		new Color(1f,0.25f,0.25f,0.75f),
		new Color(1f,0.25f,0.625f,0.75f),
		new Color(1f,0.25f,1f,0.75f),
		new Color(0.625f,0.25f,1f,0.75f),
		new Color(0.25f,0.25f,1f,0.75f),
		new Color(0.25f,0.625f,1f,0.75f),
		
		};

    }
}
