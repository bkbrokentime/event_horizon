using Combat.Component.Body;
using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Ship.Effects;
using Combat.Component.Stats;
using Combat.Component.Systems;
using Combat.Component.Triggers;
using UnityEngine;

namespace Combat.Unit.Ship.Effects.Special
{
    public class ShipSpaceJumpEffect : IShipEffect, IEngineModification, ISystemsModification
    {
        public ShipSpaceJumpEffect(float cooldown,GameObject gameObject,GameObject gameObject2, Combat.Component.Ship.Ship ship, params IUnitEffect[] effects)
        {
            _cooldown = cooldown;
            foreach (var item in effects)
                _triggers.Add(item);

            _ship = ship;
            _gameobject = gameObject;
            _gameobject2 = gameObject2;
            _endposition = gameObject.transform.position;
            _startposition = _endposition - RotationHelpers.Direction(_ship.Body.WorldRotation()) * 100 * _cooldown;
            _triggers.Invoke(ConditionType.OnActivate);
        }

        public bool IsAlive { get { return _elapsedTime < _cooldown; } }

        public bool CanActivateSystem(ISystem system) { return false; }
        public void OnSystemActivated(ISystem system) {}

        public bool TryApplyModification(ref EngineData data)
        {
            if (!IsAlive)
                return false;

            data.Throttle = 0;
            data.HasCourse = false;
            data.TurnRate = 0;
            data.Propulsion = 0;
            return true;
        }

        public void UpdatePhysics(IShip ship, float elapsedTime)
        {
            if (!IsAlive)
                return;

            _elapsedTime += elapsedTime;

            if (_elapsedTime < _cooldown)
            {
                _triggers.UpdatePhysics(elapsedTime);

                _gameobject.transform.position = _startposition + (_endposition - _startposition) * _elapsedTime / _cooldown;
                _gameobject.SetActive(true);
            }
            else
            {
                _triggers.Invoke(ConditionType.OnDeactivate);
                //ship.Vanish();
                _ship.Stats.SpaceJump = false;
                _gameobject.transform.position = _endposition;
                GameObject.Destroy(_gameobject);
                _gameobject2.SetActive(true);
                _ship.Body.ApplyAcceleration(RotationHelpers.Direction(_ship.Body.WorldRotation()) * _ship.Specification.Stats.EngineMAXPower * 0.3f);
            }
        }

        public void UpdateView(IShip ship, float elapsedTime)
        {
            _triggers.UpdateView(elapsedTime);
        }

        public void Dispose()
        {
            _triggers.Dispose();
        }

        public IEngineModification EngineModification { get { return this; } }
        public IFeaturesModification FeaturesModification { get { return null; } }
        public ISystemsModification SystemsModification { get { return this; } }
        public IStatsModification StatsModification { get { return null; } }
        public IStatsWeaponModification StatsWeaponModification { get { return null; } }
        public IStatsAttenuationModification StatsAttenuationModifications { get { return null; } }
        public IUnitAction UnitAction { get { return null; } }

        private float _elapsedTime;
        private readonly float _cooldown;
        private readonly UnitTriggers _triggers = new UnitTriggers();

        private Combat.Component.Ship.Ship _ship;
        private GameObject _gameobject;
        private GameObject _gameobject2;
        private readonly Vector2 _startposition;
        private readonly Vector2 _endposition;
    }
}
