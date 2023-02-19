using Combat.Component.Bullet;
using Combat.Component.Platform;
using Combat.Component.Triggers;
using GameDatabase.DataModel;
using UnityEngine;
using Combat.Component.Ship;
using TMPro;
using Constructor.Modification;
using Combat.Factory;
using Zenject;

namespace Combat.Component.Systems.Weapons
{
    public class HailOfBullets_Multishot : SystemBase, IWeapon
    {
        public HailOfBullets_Multishot(IWeaponPlatform platform, WeaponStats weaponStats, Factory.IBulletFactory bulletFactory, int keyBinding, IShip ship)
            : base(keyBinding, weaponStats.ControlButtonIcon, ship)
        {
            MaxCooldown = weaponStats.FireRate > 0.0000001f ? 1f / weaponStats.FireRate : 999999999;
            _cooldown = MaxCooldown;
            _bulletFactory = bulletFactory;
            _platform = platform;
            _energyConsumption = bulletFactory.Stats.EnergyCost * weaponStats.Magazine;
            _spread = weaponStats.Spread;
            _magazine = weaponStats.Magazine;
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
            _platform.Aim(Info.BulletSpeed, Info.Range, Info.IsRelativeVelocity);

            for (var i = 0; i < _magazine; ++i)
            {
                _bulletFactory.Create(_platform, 0, 0, 0, new Vector2((Random.value - 0.5f) * _spread, (Random.value - 0.5f) * _spread));
            }

            _platform.OnShot();
        }

        private readonly int _magazine;
        private readonly float _spread;
        private readonly float _cooldown;
        private readonly float _energyConsumption;
        private readonly IWeaponPlatform _platform;
        private readonly Factory.IBulletFactory _bulletFactory;
        private readonly IShip _ship;
        private readonly WeaponStats _weaponStats;
    }
}
