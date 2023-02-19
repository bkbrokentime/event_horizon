﻿using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using Constructor.Modification;

namespace Combat.Component.Systems.Weapons
{
    public class ManageableCannon : SystemBase, IWeapon
    {
        public ManageableCannon(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 999999999;
            _cooldown = MaxCooldown;
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;

            _weaponStats = weaponStats;
            _ship = ship;

            Info = new WeaponInfo(WeaponType.Manageable, _spread, bulletFactory, platform);
        }

        public override float ActivationCost { get { return _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier; } }
        public override bool CanBeActivated { get { return base.CanBeActivated && _platform.IsReady && (HasActiveBullet || _platform.EnergyPoints.Value >= _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier); } }
        public override float Cooldown { get { return Mathf.Max(_platform.Cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier), base.Cooldown); } }
        public IBullet ActiveBullet { get { return HasActiveBullet ? _activeBullet : null; } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }

        protected override void OnUpdateView(float elapsedTime) { }

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            MaxCooldown = _cooldown / Mathf.Max(0.0000001f, _ship.Stats.WeaponUpgrade.FireRateMultiplier);
            if (_activeBullet != null)
            {
                if (_activeBullet.State != UnitState.Active)
                {
                    _platform.OnShot();
                    TimeFromLastUse = 0;
                    _activeBullet = null;
                    InvokeTriggers(ConditionType.OnDeactivate);
                }
                else if (!Active)
                {
                    _activeBullet.Detonate();
                }
            }
            else if (Active && CanBeActivated && _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier))
            {
                Shot();
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnDispose()
        {
            //if (HasActiveBullet)
            //    _activeBullet.Detonate();
        }

        private void Shot()
        {
            if (HasActiveBullet)
                _activeBullet.Detonate();

            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);
            _activeBullet = _bulletFactory.Create(_platform, _spread, 0, 0, Vector2.zero);
        }

        private bool HasActiveBullet { get { return _activeBullet.IsActive(); } }

        private IBullet _activeBullet;
        private readonly float _spread;
        private readonly float _cooldown;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
