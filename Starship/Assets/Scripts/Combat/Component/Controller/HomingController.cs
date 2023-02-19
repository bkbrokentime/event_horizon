using Combat.Component.Body;
using Combat.Component.Unit;
using Combat.Scene;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class HomingController : IController
    {
        public HomingController(IUnit unit, float maxVelocity, float maxAngularVelocity, float acceleration, float maxRange, IScene scene,bool Intelligen=false)
        {
            _unit = unit;
            _scene = scene;
            _maxVelocity = maxVelocity;
            _maxAngularVelocity = maxAngularVelocity;
            _acceleration = acceleration;
            _maxRange = maxRange;
            _intelligent = Intelligen;
        }

        public void Dispose() {}

        public void UpdatePhysics(float elapsedTime)
        {
            if (_unit.Body.Parent != null)
                return;

            _timeFromLastUpdate += elapsedTime;

            if (!_target.IsActive() || _timeFromLastUpdate > _targetUpdateCooldown)
            {
                _target = _scene.Ships.GetEnemy(_unit, 0f, _maxRange * 1.3f, 15f, false, false);
                _timeFromLastUpdate = 0;
            }

            var requiredAngularVelocity = 0f;
            if (_target.IsActive())
            {
                if (_intelligent)
                {
                    var T_X = _target.Body.WorldPosition().x;//a
                    var T_Y = _target.Body.WorldPosition().y;//b
                    var U_X = _unit.Body.WorldPosition().x;//c
                    var U_Y = _unit.Body.WorldPosition().y;//d
                    var T_VX = _target.Body.Velocity.x;
                    var T_VY = _target.Body.Velocity.y;
                    var T_V = T_VX * T_VX + T_VY * T_VY;
                    var U_V = _unit.Body.Velocity.magnitude * _unit.Body.Velocity.magnitude;

                    var F_A = (T_X - U_X) * (T_X - U_X) + (T_Y - U_Y) * (T_Y - U_Y);
                    var F_B = 2 * (T_X - U_X) * T_VX + 2 * (T_Y - U_Y) * T_VY;
                    var F_C = T_V - U_V;

                    var F_Delta = F_B * F_B - 4 * F_A * F_C;
                    var T = F_Delta > 0 ? 2 * F_A / (-F_B + Mathf.Sqrt(F_Delta)) : -1;

                    var distance = Vector2.Distance(_unit.Body.WorldPosition(), _target.Body.WorldPosition());
                    var deltatime = T > 0 ? T
                        : Vector2.Angle(_target.Body.WorldPosition().Direction(_unit.Body.WorldPosition()), _target.Body.Velocity) > 90
                        ? distance / Mathf.Abs(_unit.Body.Velocity.magnitude - _target.Body.Velocity.magnitude)
                        : distance / Mathf.Abs(_unit.Body.Velocity.magnitude + _target.Body.Velocity.magnitude);

                    var direction = deltatime > 20 ? _unit.Body.WorldPosition().Direction(_target.Body.WorldPosition()) : _unit.Body.WorldPosition().Direction(_target.Body.WorldPosition() + _target.Body.Velocity * deltatime);



                    var target = RotationHelpers.Angle(direction);
                    var rotation = _unit.Body.WorldRotation();
                    var delta = Mathf.DeltaAngle(rotation, target);
                    requiredAngularVelocity = delta > 5 ? _maxAngularVelocity : delta < -5 ? -_maxAngularVelocity : 0f;
                }
                else
                {
                    var direction = _unit.Body.WorldPosition().Direction(_target.Body.WorldPosition());
                    var target = RotationHelpers.Angle(direction);
                    var rotation = _unit.Body.WorldRotation();
                    var delta = Mathf.DeltaAngle(rotation, target);
                    requiredAngularVelocity = delta > 5 ? _maxAngularVelocity : delta < -5 ? -_maxAngularVelocity : 0f;
                }

            }
            _unit.Body.ApplyAngularAcceleration(requiredAngularVelocity - _unit.Body.AngularVelocity);

            UpdateVelocity(elapsedTime);
        }

        private void UpdateVelocity(float deltaTime)
        {
            var forward = RotationHelpers.Direction(_unit.Body.Rotation);
            var velocity = _unit.Body.Velocity;
            var forwardVelocity = Vector2.Dot(velocity, forward);
            if (forwardVelocity >= _maxVelocity)
                return;

            var requiredVelocity = Mathf.Max(forwardVelocity, _maxVelocity) * forward;
            var dir = (requiredVelocity - velocity).normalized;

            _unit.Body.ApplyAcceleration(dir * _acceleration * deltaTime);
        }

        private float _timeFromLastUpdate;
        private IUnit _target;
        private readonly IUnit _unit;
        private readonly IScene _scene;
        private readonly float _maxVelocity;
        private readonly float _maxAngularVelocity;
        private readonly float _acceleration;
        private readonly float _maxRange;
        private const float _targetUpdateCooldown = 1.0f;
        private readonly bool _intelligent;
    }
}
