using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Services.Reources;
using Zenject;
using Gui.Windows;
using Services.Gui;

namespace Gui.Combat
{
    [RequireComponent(typeof(AnimatedWindow))]

    public class RadarMapPanel : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly IScene _scene;

        [Inject]
        private void Initialize()
        {
            _radars.AddRange(transform.Children<RadarMap>());
            _beaconRadars.AddRange(transform.Children<BeaconRadarMap>());
        }
        public void Open()
        {
            GetComponent<AnimatedWindow>().Open();
        }
        public void Close()
        {
            GetComponent<AnimatedWindow>().Close(WindowExitCode.Ok);
        }
        public void Add(IShip ship)
        {
            (_radars.FirstOrDefault(item => !item.gameObject.activeSelf) ?? CreateRadar()).Open(ship, _scene, _resourceLocator);
        }
        public void AddBeacon(IUnit unit, Game.Exploration.ObjectiveType type)
        {
            (_beaconRadars.FirstOrDefault(item => !item.gameObject.activeSelf) ?? CreateBeaconRadar()).Open(unit, type, _scene);
        }
        public void RemoveBeacon(IUnit unit)
        {
            var index = _beaconRadars.FindIndex(item => item.Unit == unit);
            if (index >= 0)
                _beaconRadars[index].Close();
        }
        public bool IsHide { get; set; }

        public void Initialize(IEnumerable<IUnit> items)
        {
            var radarCount = _radars.Count;

            var radarIndex = 0;
            var simpleRadarIndex = 0;
            foreach (var item in items)
            {
                if (item.Type.Class == UnitClass.Ship)
                {
                    if (radarIndex >= radarCount)
                        CreateRadar();

                    _radars[radarIndex].Open((IShip)item, _scene, _resourceLocator);

                    radarIndex++;
                }
            }

            for (var i = radarIndex; i < radarCount; ++i)
                _radars[i].Close();
        }

        private RadarMap CreateRadar()
        {
            var radar = GameObject.Instantiate(_radars[0]);
            radar.RectTransform.SetParent(transform);
            radar.RectTransform.SetParent(transform);
            radar.RectTransform.localScale = Vector3.one;
            _radars.Add(radar);
            return radar;
        }
        private BeaconRadarMap CreateBeaconRadar()
        {
            var radar = GameObject.Instantiate(_beaconRadars[0]);
            radar.RectTransform.SetParent(transform);
            radar.RectTransform.SetParent(transform);
            radar.RectTransform.localScale = Vector3.one;
            _beaconRadars.Add(radar);
            return radar;
        }

        private readonly List<RadarMap> _radars = new List<RadarMap>();
        private readonly List<BeaconRadarMap> _beaconRadars = new List<BeaconRadarMap>();

    }
}
