using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Unit.HitPoints;
using UnityEngine;

namespace Combat.Component.Platform
{
    public sealed class FixedPlatform : IWeaponPlatform
    {
        public FixedPlatform(IShip ship, IBody body, float cooldown, float rotation, float basespread, Vector2 MoveCenterPosition, float MoveSpeed, Vector2 MoveCenterRange, IAimingSystem aimingSystem = null)
        {
            _body = body;
            _ship = ship;
            _cooldown = cooldown;
            _aimingSystem = aimingSystem;
            _MoveCenterPosition = MoveCenterPosition;
            _MoveSpeed = MoveSpeed;
            _MoveCenterRange = MoveCenterRange;

            _startrotation = _moverotation = rotation;

            _basespread = basespread;
        }

        public UnitType Type { get { return _ship.Type; } }
        public IBody Body { get { return _body; } }
        public IResourcePoints EnergyPoints { get { return _ship.Stats.Energy; } }
        public bool IsTemporary { get { return false; } }

        public bool IsReady { get { return _timeFromLastShot > _cooldown; } }
        public float Cooldown { get { return Mathf.Clamp01(1f - _timeFromLastShot / _cooldown); } }
        public float CooldownTime { get { return _cooldown; } }

        public float FixedRotation { get { return _body.WorldRotation(); } }
        public float AutoAimingAngle { get { return 0; } }
        public Vector2 MoveCenterPosition { get { return _MoveCenterPosition; } }
        public float MoveSpeed { get { return _MoveSpeed; } }
        public Vector2 MoveCenterRange { get { return _MoveCenterRange; } }
        public float BaseSpread { get { return _basespread; } }

        public void SetView(IView view, UnityEngine.Color color)
        {
            _view = view;
            _color = color;
        }

        public void Aim(float bulletVelocity, float weaponRange, bool relative)
        {
            if (_aimingSystem != null)
                _aimingSystem.Aim(bulletVelocity, weaponRange, relative);
        }

        public void OnShot()
        {
            _timeFromLastShot = 0;
        }

        public void UpdatePhysics(float elapsedTime)
        {
            _timeFromLastShot += elapsedTime;

            if (_MoveSpeed != 0)
            {
                _moverotation += _MoveSpeed * elapsedTime;
                var newposition = new Vector2(Mathf.Cos(_moverotation * Mathf.Deg2Rad) * _MoveCenterRange.x, Mathf.Sin(_moverotation * Mathf.Deg2Rad) * _MoveCenterRange.y);
                var setpositopn = Vector2.zero;
                setpositopn.x = newposition.x * Mathf.Cos(_startrotation * Mathf.Deg2Rad) - newposition.y * Mathf.Sin(_startrotation * Mathf.Deg2Rad);
                setpositopn.y = newposition.y * Mathf.Cos(_startrotation * Mathf.Deg2Rad) + newposition.x * Mathf.Sin(_startrotation * Mathf.Deg2Rad);
                _body.Move(_MoveCenterPosition + setpositopn);
            }

        }

        public void UpdateView(float elapsedTime)
        {
            if (_view != null)
            {
                _view.Color = _color * _ship.Features.Color;
                _view.UpdateView(elapsedTime);
            }
        }

        public void Dispose() {}

        private IView _view;
        private Color _color;
        private float _timeFromLastShot;
        private readonly IShip _ship;
        private readonly float _cooldown;
        private readonly IBody _body;
        private readonly IAimingSystem _aimingSystem;

        public Vector2 _MoveCenterPosition;
        public float _MoveSpeed;
        public Vector2 _MoveCenterRange;

        private float _moverotation;
        private float _startrotation;

        private float _basespread;

    }
}
