using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using TMPro;
using Constructor.Modification;

namespace Combat.Component.Systems.Weapons
{
    public class BarrageMultishot_SwingBilateral : SystemBase, IWeapon
    {
        public BarrageMultishot_SwingBilateral(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 999999999;
            _cooldown = MaxCooldown;
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost * weaponStats.Magazine;
            _magazine = weaponStats.Magazine;
            _maxspread = weaponStats.Spread;
            _swingrate = weaponStats.SwingRate;
            if (_spread == -999f)
            {
                if (_magazine % 2 == 0)
                {
                    _spread = 0;
                    bulletItems = new BulletItem[_magazine];
                    for (int i = 0; i < _magazine; i++)
                    {
                        bulletItems[i].num = i;
                        bulletItems[i].spread = -_maxspread / 2 + i / 2 * 2 * _maxspread / _magazine;
                        bulletItems[i].dir = i % 2 == 0;
                    }
                }
                else
                {
                    _spread = 0;
                    bulletItems = new BulletItem[_magazine];
                    for (int i = 0; i < _magazine - 1; i++)
                    {
                        bulletItems[i].num = i;
                        bulletItems[i].spread = -_maxspread / 2 + i / 2 * 2 * _maxspread / (_magazine - 1);
                        bulletItems[i].dir = i % 2 == 0;
                    }
                    bulletItems[_magazine - 1].num = _magazine - 1;
                    bulletItems[_magazine - 1].spread = 0;
                    bulletItems[_magazine - 1].dir = true;
                }
            }
            _weaponStats = weaponStats;
            _ship = ship;

            Info = new WeaponInfo(WeaponType.BarrageCannon, _spread, bulletFactory, platform);
        }

        public override float ActivationCost { get { return _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.IsReady && _platform.EnergyPoints.Value >= _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier; } }
        public override float Cooldown { get { return Mathf.Max(_platform.Cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier), base.Cooldown); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }
        public IBullet ActiveBullet { get { return null; } }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            MaxCooldown = _cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier);
            if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier))
            {
                Shot();
                TimeFromLastUse = 0;
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnDispose() { }

        private void Shot()
        {
            if (_magazine % 2 == 0)
            {
                for (var i = 0; i < _magazine; i++)
                {
                    if (!bulletItems[i].dir)
                    {
                        if (_swingrate > 0)
                        {
                            if (bulletItems[i].spread >= _maxspread / 2)
                                bulletItems[i].spread = -_maxspread / 2;
                            else
                                bulletItems[i].spread += _swingrate * _maxspread * 0.5f;
                        }
                        else
                        {
                            if (bulletItems[i].spread <= -_maxspread / 2)
                                bulletItems[i].spread = _maxspread / 2;
                            else
                                bulletItems[i].spread += _swingrate * _maxspread * 0.5f;
                        }
                        _bulletFactory.Create(Platform, 0, bulletItems[i].spread, 0, Vector2.zero);
                    }
                    else
                    {
                        if (_swingrate > 0)
                        {
                            if (bulletItems[i].spread <= -_maxspread / 2)
                                bulletItems[i].spread = _maxspread / 2;
                            else
                                bulletItems[i].spread -= _swingrate * _maxspread * 0.5f;
                        }
                        else
                        {
                            if (bulletItems[i].spread >= _maxspread / 2)
                                bulletItems[i].spread = -_maxspread / 2;
                            else
                                bulletItems[i].spread -= _swingrate * _maxspread * 0.5f;
                        }
                        _bulletFactory.Create(Platform, 0, bulletItems[i].spread, 0, Vector2.zero);
                    }
                }
            }
            else
            {
                for (var i = 0; i < _magazine - 1; i++)
                {
                    if (!bulletItems[i].dir)
                    {
                        if (_swingrate > 0)
                        {
                            if (bulletItems[i].spread >= _maxspread / 2)
                                bulletItems[i].spread = -_maxspread / 2;
                            else
                                bulletItems[i].spread += _swingrate * _maxspread * 0.5f;
                        }
                        else
                        {
                            if (bulletItems[i].spread <= -_maxspread / 2)
                                bulletItems[i].spread = _maxspread / 2;
                            else
                                bulletItems[i].spread += _swingrate * _maxspread * 0.5f;
                        }
                        _bulletFactory.Create(Platform, 0, bulletItems[i].spread, 0, Vector2.zero);
                    }
                    else
                    {
                        if (_swingrate > 0)
                        {
                            if (bulletItems[i].spread <= -_maxspread / 2)
                                bulletItems[i].spread = _maxspread / 2;
                            else
                                bulletItems[i].spread -= _swingrate * _maxspread * 0.5f;
                        }
                        else
                        {
                            if (bulletItems[i].spread >= _maxspread / 2)
                                bulletItems[i].spread = -_maxspread / 2;
                            else
                                bulletItems[i].spread -= _swingrate * _maxspread * 0.5f;
                        }
                        _bulletFactory.Create(Platform, 0, bulletItems[i].spread, 0, Vector2.zero);
                    }
                }
                _bulletFactory.Create(Platform, 0, 0, 0, Vector2.zero);
            }
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();
        }


        private struct BulletItem
        {
            public int num;
            public float spread;
            public bool dir;
        }

        private BulletItem[] bulletItems;

        private readonly int _magazine;
        private float _spread = -999f;
        private readonly float _maxspread;
        private readonly float _swingrate;
        private readonly float _cooldown;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
