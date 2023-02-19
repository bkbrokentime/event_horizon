﻿using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using Combat.Unit;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;


namespace Combat.Component.Systems.Weapons
{
    public class ContinuousCannon : SystemBase, IWeapon
    {
        public ContinuousCannon(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 0;

            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost;
            _spread = weaponStats.Spread;

            _weaponStats = weaponStats;
            _ship = ship;

            Info = new WeaponInfo(WeaponType.Continuous, _spread, bulletFactory, platform);
        }

        public override bool CanBeActivated { get { return base.CanBeActivated && (HasActiveBullet || _platform.IsReady && _platform.EnergyPoints.Value > _energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier * 0.5f); } }
        public override float Cooldown { get { return Mathf.Max(_platform.Cooldown, base.Cooldown); } }

        public WeaponInfo Info { get; private set; }
        public IWeaponPlatform Platform { get { return _platform; } }
        public float PowerLevel { get { return 1.0f; } }
        public IBullet ActiveBullet { get { return HasActiveBullet ? _activeBullet : null; } }

        protected override void OnUpdateView(float elapsedTime) {}

        protected override void OnUpdatePhysics(float elapsedTime)
        {
            if (HasActiveBullet)
            {
                _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);

                if (Active && _platform.EnergyPoints.TryGet(_energyConsumption * _ship.Stats.WeaponUpgrade.EnergyCostMultiplier * elapsedTime))
                {
                    _activeBullet.Lifetime.Restore();
                    InvokeTriggers(ConditionType.OnRemainActive);
                }
                else
                {
                    TimeFromLastUse = 0;
                    InvokeTriggers(ConditionType.OnDeactivate);
                }
            }
            else if (Active && CanBeActivated)
            {
                Shot();
                InvokeTriggers(ConditionType.OnActivate);
            }
        }

        protected override void OnDispose() {}

        private void Shot()
        {
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);

            _activeBullet = _bulletFactory.Create(_platform, _spread, 0, 0, Vector2.zero);
            _activeBullet.Lifetime.Restore();
        }

        private bool HasActiveBullet { get { return _activeBullet.IsActive(); } }

        private IBullet _activeBullet;
        private readonly float _spread;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
