using UnityEngine;
using UnityEngine.UI;
using Constructor;
using System.Linq;
using Constructor.Model;
using DataModel.Technology;
using Economy;
using GameDatabase.Enums;
using GameServices.Database;
using GameServices.Gui;
using GameServices.Player;
using GameServices.Research;
using Gui.Constructor;
using Services.Localization;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class ComponentGIFIconViewModel : MonoBehaviour
	{
		public Image iamge;
		public Sprite[] icons;
		public bool gif;

        private void Update()
        {
            if (gif && icons.Length > 0)
            {
                if (gif && icons.Length > 0)
                {
                    if (_lasttime < 0)
                    {
                        _lasttime = _time;
                        iamge.sprite = icons[_count++];
                        if (_count >= icons.Length)
                            _count = 0;
                    }
                    else
                        _lasttime -= Time.deltaTime;
                }
            }
        }
        private float _time = 0.2f;
        private float _lasttime = 0.2f;
        private int _count = 0;

    }
}
