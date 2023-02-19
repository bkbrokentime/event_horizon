using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class RandomRocketController : IController
    {
        public RandomRocketController(IUnit unit, float maxVelocity, float acceleration)
        {
            _unit = unit;
            _maxVelocity = maxVelocity;
            _acceleration = acceleration;
            _rotation = 0;
            _dir = Mathf.Sign(Random.value - 0.5f);
            _changedirtime = _maxchangedirtime;
        }

        public void Dispose() { }

        public void UpdatePhysics(float elapsedTime)
        {
            if (_unit.Body.Parent != null)
                return;

            UpdateVelocity(elapsedTime);
        }

        private void UpdateVelocity(float deltaTime)
        {
            var forward = RotationHelpers.Direction(_unit.Body.Rotation);
            var velocity = _unit.Body.Velocity;
            var forwardVelocity = Vector2.Dot(velocity, forward);

            var requiredVelocity = Mathf.Max(forwardVelocity, _maxVelocity) * forward;
            var dir = (requiredVelocity - velocity).normalized;

            _unit.Body.ApplyAcceleration(dir * _acceleration * deltaTime);

            _rotation = Random.Range(0.5f, 1.5f) * _acceleration * deltaTime * _dir;
            _changedirtime -= deltaTime;
            if(_changedirtime < 0)
            {
                _changedirtime = _maxchangedirtime;
                _dir = Mathf.Sign(Random.value - 0.5f);
            }
            _unit.Body.Turn(_unit.Body.Rotation + _rotation);
        }

        private readonly IUnit _unit;
        private readonly float _maxVelocity;
        private readonly float _acceleration;

        private float _dir;
        private float _rotation;
        private float _changedirtime;
        private float _maxchangedirtime=0.2f;
    }
}
