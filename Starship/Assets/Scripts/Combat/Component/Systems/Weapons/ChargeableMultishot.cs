using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using Combat.Factory;
using Constructor.Modification;

namespace Combat.Component.Systems.Weapons
{
    public class ChargeableMultishot : SystemBase, IWeapon
    {
        public ChargeableMultishot(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost * weaponStats.Magazine;
            _spread = weaponStats.Spread;
            _chargeTotalTime = 1.0f / weaponStats.FireRate;
            _maxmagazine = weaponStats.Magazine;
            _magazine = 0;

            _weaponStats = weaponStats;
            _ship = ship;

            Info = new WeaponInfo(WeaponType.RequiredCharging, _spread, bulletFactory, platform);
        }

        public override bool CanBeActivated { get { return !_ship.Stats.SpaceJump && (_chargeTime > 0 || (_platform.IsReady && _platform.EnergyPoints.Value >= _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier * 0.5f)); } }
        public override float Cooldown { get { return _platform.Cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return Mathf.Clamp01(_chargeTime * _ship.Stats.WeaponUpgrade.FireRateMultiplier / _chargeTotalTime); } }
        public IBullet ActiveBullet { get { return null; } }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (Active && CanBeActivated && _chargeTime > 0 && (_chargeTime * _ship.Stats.WeaponUpgrade.FireRateMultiplier > _chargeTotalTime || _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier * elapsedTime / _chargeTotalTime)))
            {
                _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
                _chargeTime += elapsedTime;
                UpdatePower();
            }
            else if (_chargeTime > 0)
            {
                if (_chargeTime > 0.1f) Shot();
                _chargeTime = 0;
                InvokeTriggers(ConditionType.OnDeactivate);
            }
            else if (Active && CanBeActivated)
            {
                InvokeTriggers(ConditionType.OnActivate);
                _chargeTime += elapsedTime;
                UpdatePower();
            }
            else if (HasActiveBullet && Info.BulletType == BulletType.Direct)
            {
                _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {

            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();

            for (var i = 0; i < _magazine; ++i)
                _activeBullet = _bulletFactory.Create(_platform, 0, (Random.value - 0.5f) * _spread, 0, Vector2.zero);

            InvokeTriggers(ConditionType.OnDischarge);
        }

        private void UpdatePower()
        {
            _bulletFactory.Stats.PowerLevel = PowerLevel;
            _magazine = Mathf.FloorToInt(_maxmagazine * 2.0f * PowerLevel);
        }

        private bool HasActiveBullet { get { return _activeBullet.IsActive(); } }

        private float _chargeTime;
        private readonly int _maxmagazine;
        private  int _magazine;

        private IBullet _activeBullet;
        private readonly float _chargeTotalTime;
        private readonly float _spread;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
