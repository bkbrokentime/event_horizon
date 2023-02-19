using Combat.Component.Body;
using Combat.Component.Satellite;
using Combat.Component.Ship;
using Combat.Helpers;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class SatelliteControllser : IController
    {
        public SatelliteControllser(IShip ship, ISatellite satellite, GameObjectHolder gameObject, Vector2[] position, float rotation)
        {
            _ship = ship;
            _satellite = satellite;
            _gameObject = gameObject;
            _position = position;
            _rotation = rotation;
        }

        public virtual void UpdatePhysics(float elapsedTime)
        {
            if (_ship.Stats.SpaceJump)
                _gameObject.IsActive = false;
            else
                _gameObject.IsActive = true;


            if (_ship.State == UnitState.Destroyed)
                _satellite.Destroy();
            else if (_ship.State == UnitState.Inactive)
                _satellite.Vanish();
            else
            {
                var mode = 0;
                var MAX = _ship.Engine.MaxVelocity;
                var NOW = _ship.Body.Velocity.magnitude;

                if(MAX<=5)
                    mode = 0;
                else if(MAX<=15)
                {
                    if (NOW <= 5)
                        mode = 0;
                    else
                        mode = 1;
                }
                else if(MAX<=25)
                {
                    if (NOW <= 5)
                        mode = 0;
                    else if (NOW <= 15)
                        mode = 1;
                    else
                        mode = 2;
                }
                else
                {
                    if (NOW <= 5)
                        mode = 0;
                    else if (NOW <= 15)
                        mode = 1;
                    else if (NOW <= 25)
                        mode = 2;
                    else
                        mode = 3;
                }

                var requiredPosition = _ship.Body.WorldPosition() + RotationHelpers.Transform(_position[mode], _ship.Body.WorldRotation());
                var requiredRotation = _ship.Body.WorldRotation() + _rotation;
                    _satellite.MoveTowards(requiredPosition, requiredRotation, _ship.Body.WorldVelocity(), _velocityFactor, _angularVelocityFactor);
          }
        }

        public void Dispose() { }

        private readonly float _rotation;
        private readonly Vector2[] _position;
        private readonly IShip _ship;
        private readonly ISatellite _satellite;
        private readonly GameObjectHolder _gameObject;

        private const float _velocityFactor = 0.75f;
        private const float _angularVelocityFactor = 3.0f;
    }
}
