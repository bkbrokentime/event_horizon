using Combat.Collision;
using Combat.Component.Unit;
using Combat.Unit;
using Constructor.Modification;

namespace Combat.Component.DamageHandler
{
    public class BeamDamageHandler : IDamageHandler
    {
        public BeamDamageHandler(IUnit unit)
        {
            _unit = unit;
        }

        public CollisionEffect ApplyDamage(Impact impact)
        {
            var owner = _unit.Type.Owner;

            if (impact.AllDamageData.Repair.Value > 0 && owner.IsActive())
            {
                var newimpact = new Impact(new GameDatabase.Model.AllDamageData(), 0, 0, new Impulse(), CollisionEffect.None);
                newimpact.AllDamageData.Repair = impact.AllDamageData.Repair;
                owner.Affect(newimpact);
            }

            return CollisionEffect.None;
        }

        public void Dispose() { }

        private readonly IUnit _unit;
    }
}
