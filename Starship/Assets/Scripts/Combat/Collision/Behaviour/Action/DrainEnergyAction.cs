﻿using Combat.Collision.Manager;
using Combat.Component.Unit;
using GameDatabase.Enums;

namespace Combat.Collision.Behaviour.Action
{
    public class DrainEnergyAction : ICollisionAction
    {
        public DrainEnergyAction(float energyDrain, BulletImpactType impactType)
        {
            _impactType = impactType;
            _energyDrain = energyDrain;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (_impactType == BulletImpactType.DamageOverTime)
            {
                targetImpact.AllDamageData.EnergyDrain.UpDateDamage(_energyDrain * collisionData.TimeInterval);
            }
            else
            {
                if (!collisionData.IsNew || !_isAlive)
                    return;

                targetImpact.AllDamageData.EnergyDrain.UpDateDamage(_energyDrain);
                _isAlive = _impactType == BulletImpactType.HitAllTargets;
            }
        }

        public void Dispose() { }

        private bool _isAlive = true;
        private readonly float _energyDrain;
        private readonly BulletImpactType _impactType;
    }
}
