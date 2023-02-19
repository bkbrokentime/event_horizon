using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using TMPro;
using Constructor.Modification;
using static ShipStatsShow;

namespace Combat.Component.Systems.Weapons
{
    public class BarrageMachineGun_Swing : SystemBase, IWeapon
    {
        public BarrageMachineGun_Swing(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 999999999;
            _cooldown = MaxCooldown;
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _magazine = weaponStats.Magazine;
            _maxspread = weaponStats.Spread;
            _swingrate = weaponStats.SwingRate;
            if (_spread == -999f)
            {
                _spread = 0;
                _dir = false;
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

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            MaxCooldown = _cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier);
            if (Active && _shots < _magazine && CanBeActivated)
            {
                if (_platform.IsReady && _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier))
                {
                    Shot();
                    _shots++;
                    InvokeTriggers(ConditionType.OnActivate);
                }
            }
            else if (_shots >= _magazine)
            {
                _shots = 0;
                TimeFromLastUse = 0;
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            if (_swingrate > 0)
            {
                if (!_dir)
                {
                    if (_spread >= _maxspread / 2)
                        _dir = true;
                    else
                        _spread += _swingrate * _maxspread * 0.5f;
                }
                else
                {
                    if (_spread <= -_maxspread / 2)
                        _dir = false;
                    else
                        _spread -= _swingrate * _maxspread * 0.5f;
                }
            }
            else
            {
                if (!_dir)
                {
                    if (_spread <= -_maxspread / 2)
                        _dir = true;
                    else
                        _spread += _swingrate * _maxspread * 0.5f;
                }
                else
                {
                    if (_spread >= _maxspread / 2)
                        _dir = false;
                    else
                        _spread -= _swingrate * _maxspread * 0.5f;
                }
            }
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _platform.OnShot();
            _bulletFactory.Create(_platform, 0, _spread, 0, Vector2.zero);
        }

        private struct BulletItem
        {
            public int num;
            public float spread;
            public bool dir;
        }


        private int _shots;
        private readonly int _magazine;
        private float _spread = -999f;
        private bool _dir;
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
