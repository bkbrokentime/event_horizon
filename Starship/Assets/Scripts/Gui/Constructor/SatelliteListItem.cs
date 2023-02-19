using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Constructor
{
    public class SatelliteListItem : MonoBehaviour
    {
        public Image Icon;
        public Text NameText;
        public Text QuantityText;
        public Text SizeText;
        public Text WeaponText;
        public GameObject ButtonsPanel;
        public Text ClickToInstallText;
        public Text CantBeInstalledText;
        public Button InstallOnTheLeftButton;
        public Button InstallOnTheRightButton;
        public Button InstallOnTheLeftButton2;
        public Button InstallOnTheRightButton2;
        public Button InstallOnTheLeftButton3;
        public Button InstallOnTheRightButton3;
        public Button InstallOnTheLeftButton4;
        public Button InstallOnTheRightButton4;
        public Button InstallOnTheLeftButton5;
        public Button InstallOnTheRightButton5;

        public ItemId<SatelliteBuild> BuildId;
        public ItemId<Satellite> Id;

        public void ShowButton(bool active)
        {
            ButtonsPanel.SetActive(active);
        }
    }
}
