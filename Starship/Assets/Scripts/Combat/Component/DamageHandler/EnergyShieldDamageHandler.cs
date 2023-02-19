using Combat.Collision;
using Combat.Unit.Auxiliary;

namespace Combat.Component.DamageHandler
{
    public class EnergyShieldDamageHandler : IDamageHandler
    {
        public EnergyShieldDamageHandler(IAuxiliaryUnit shield, float energyConsumption)
        {
            _shield = shield;
            _energyConsumption = energyConsumption;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            impact.ApplyImpulse(_shield.Body);
            impact.RemoveImpulse();

            var parent = _shield.Type.Owner;
            var resistance = parent.Stats.Resistance;
            if (parent == null)
                return CollisionEffect.None;

            var damage = impact.EnergyShieldGetTotalDamage(resistance);

            if (parent.Stats.EnergyShield.TryGet(damage*_energyConsumption))
            {
                impact.EnergyShieldRemoveDamage();
            }
            else
            {
                var energy = parent.Stats.EnergyShield.Value;
                parent.Stats.EnergyShield.Get(energy);
                impact.EnergyShieldRemoveDamage(energy / _energyConsumption, resistance);
                _shield.Enabled = false;
            }

            parent.Affect(impact);

            return CollisionEffect.None;
        }

        public void Dispose()
        {
        }

        private readonly IAuxiliaryUnit _shield;
        private readonly float _energyConsumption;
    }
}
