using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Component.Satellite;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Helpers;
using Combat.Scene;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    class AutoAimingSatelliteController : IController, IAimingSystem
    {
        public AutoAimingSatelliteController(IShip ship, ISatellite satellite, GameObjectHolder gameObject, Vector2[] position, float rotation, float minAngle, float maxAngle, IScene scene) 
        {
            _minAngle = minAngle;
            _maxAngle = maxAngle;
            _defaultRotation = rotation;
            _position = position;
            _scene = scene;
            _ship = ship;
            _Ship = ship;
            _satellite = satellite;
            _gameObject = gameObject;
        }

        public void Aim(float bulletVelocity, float weaponRange, bool relative)
        {
            _bulletVelocity = bulletVelocity;
            _weaponRange = weaponRange;
            _isRelativeVelocity = relative;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            if (_Ship.Stats.SpaceJump)
                _gameObject.IsActive = false;
            else
                _gameObject.IsActive = true;

            if (_ship.State == UnitState.Destroyed)
                _satellite.Destroy();
            else if (_ship.State == UnitState.Inactive)
                _satellite.Vanish();
            else
            {
                _timeFromTargetUpdate += elapsedTime;


                var requiredPosition = GetRequiredPosition();
                var requiredRotation = _ship.Body.WorldRotation() + GetTargetCourse();
                _satellite.MoveTowards(requiredPosition, requiredRotation, _ship.Body.WorldVelocity(), _velocityFactor, _angularVelocityFactor);
            }
        }

        public void Dispose() { }

        private Vector2 GetRequiredPosition()
        {
            var mode = 0;
            var MAX = _Ship.Engine.MaxVelocity;
            var NOW = _Ship.Body.Velocity.magnitude;

            if (MAX <= 5)
                mode = 0;
            else if (MAX <= 15)
            {
                if (NOW <= 5)
                    mode = 0;
                else
                    mode = 1;
            }
            else if (MAX <= 25)
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

            return _ship.Body.WorldPosition() + RotationHelpers.Transform(_position[mode], _ship.Body.WorldRotation());
        }

        private float GetTargetCourse()
        {
            if (_timeFromTargetUpdate > _targetUpdateCooldown || (_timeFromTargetUpdate > _targetFindCooldown && !_target.IsActive()))
            {
                _target = _scene.Ships.GetEnemy(_ship, EnemyMatchingOptions.EnemyForTurret(), _defaultRotation + (_maxAngle - _minAngle) / 2, _maxAngle - _minAngle, _weaponRange);
                _timeFromTargetUpdate = 0;
            }

            if (!_target.IsActive())
                return _defaultRotation;

            var targetPosition = _target.Body.WorldPosition();
            var platformPosition = GetRequiredPosition();
            float rotation;

            if (_bulletVelocity > 0)
            {
                var velocity = _isRelativeVelocity ? _target.Body.WorldVelocity() - _satellite.Body.WorldVelocity() : _target.Body.WorldVelocity();

                Vector2 target;
                float timeInterval;
                if (!Geometry.GetTargetPosition(
                    targetPosition,
                    velocity,
                    platformPosition,
                    _bulletVelocity,
                    out target,
                    out timeInterval))
                {
                    target = targetPosition;
                }

                rotation = RotationHelpers.Angle(platformPosition.Direction(target)) - _ship.Body.WorldRotation();;
            }
            else
            {
                rotation = RotationHelpers.Angle(platformPosition.Direction(targetPosition)) - _ship.Body.WorldRotation();
            }

            if (rotation < _defaultRotation + _minAngle)
                rotation = _defaultRotation;
            else if (rotation > _defaultRotation + _maxAngle)
                rotation = _defaultRotation;

            return rotation;
        }

        private float _bulletVelocity;
        private float _weaponRange;
        private bool _isRelativeVelocity = true;
        private float _timeFromTargetUpdate;

        private IUnit _target;
        private readonly float _minAngle;
        private readonly float _maxAngle;
        private readonly float _defaultRotation;
        private readonly Vector2[] _position;
        private readonly IScene _scene;
        private readonly IUnit _ship;
        private readonly IShip _Ship;
        private readonly ISatellite _satellite;
        private readonly GameObjectHolder _gameObject;

        private const float _targetUpdateCooldown = 1.0f;
        private const float _targetFindCooldown = 0.1f;
        private const float _velocityFactor = 0.75f;
        private const float _angularVelocityFactor = 3.0f;
    }
}
