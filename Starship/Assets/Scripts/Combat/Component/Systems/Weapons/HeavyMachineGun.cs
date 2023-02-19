using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using Constructor.Modification;

namespace Combat.Component.Systems.Weapons
{
    public class HeavyMachineGun : SystemBase, IWeapon
    {
        public HeavyMachineGun(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 999999999;
            _cooldown = MaxCooldown;
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _maxspread = weaponStats.Spread;
            _spread = 0;
            _firerate = 0.2f;

            _weaponStats = weaponStats;
            _ship = ship;

            Info = new WeaponInfo(WeaponType.Common, _spread, bulletFactory, platform);
        }

        public override float ActivationCost { get { return _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.EnergyPoints.Value > _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier; } }
        public override float Cooldown { get { return Mathf.Max(_platform.Cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier), base.Cooldown); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }
        public IBullet ActiveBullet { get { return null; } }


        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            MaxCooldown = _cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier) / _firerate;
            _lastfiretime += elapsedTime;
            if (Active && CanBeActivated)
            {
                if (_platform.IsReady && _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier))
                {
                    if (_lastfiretime < 10f)
                    {
                        if (_spread < _maxspread)
                        {
                            _spread = (_firerate - 0.2f) / 4.8f * _maxspread;
                            if (_spread > _maxspread)
                                _spread = _maxspread;
                        }
                        if (_firerate < 5f)
                        {
                            _firerate += 0.02f;
                        }
                    }
                    else
                    {
                        _spread = 0;
                        _firerate = 0.2f;
                    }
                    Shot();
                    TimeFromLastUse = 0;
                    InvokeTriggers(ConditionType.OnActivate);
                }
            }
            if (_lastfiretime > MaxCooldown * 1.2f)
            {
                _firerate -= elapsedTime * 0.5f;
                if (_firerate < 0.2f)
                    _firerate = 0.2f;
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            _lastfiretime = 0;
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();
            _bulletFactory.Create(_platform, 0, Random.Range(-_spread / 2, _spread / 2), 0, Vector2.zero);
        }

        private readonly int _magazine;
        private float _spread;
        private readonly float _maxspread;
        private float _firerate;
        private float _lastfiretime;
        private readonly float _cooldown;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
