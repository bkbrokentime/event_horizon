using System;
using System.Collections.Generic;
using Constructor.Satellites;
using Utils;

namespace Constructor.Ships
{
    public class ArenaShip : BaseShip
    {
        public ArenaShip(IShip ship, float powerMultiplier = 1.0f)
            : base(ship.Model.Clone())
        {
            _name = ship.Name;
            _powerMultiplier = powerMultiplier;
            _components.Assign(ship.Components);
            Satellite_Left_1 = ship.Satellite_Left_1.CreateCopy();
            Satellite_Right_1 = ship.Satellite_Right_1.CreateCopy();
            Satellite_Left_2 = ship.Satellite_Left_2.CreateCopy();
            Satellite_Right_2 = ship.Satellite_Right_2.CreateCopy();
            Satellite_Left_3 = ship.Satellite_Left_3.CreateCopy();
            Satellite_Right_3 = ship.Satellite_Right_3.CreateCopy();
            Satellite_Left_4 = ship.Satellite_Left_4.CreateCopy();
            Satellite_Right_4 = ship.Satellite_Right_4.CreateCopy();
            Satellite_Left_5 = ship.Satellite_Left_5.CreateCopy();
            Satellite_Right_5 = ship.Satellite_Right_5.CreateCopy();
        }

        public override string Name
        {
            get { return _name; }
            set { base.Name = value; }
        }

        public override ShipBuilder CreateBuilder()
        {
            var builder = base.CreateBuilder();

            builder.Bonuses.ArmorPointsMultiplier *= _powerMultiplier;
            builder.Bonuses.ShieldPointsMultiplier *= _powerMultiplier;
            builder.Bonuses.EnergyShieldPointsMultiplier *= _powerMultiplier;
            builder.Bonuses.DamageMultiplier *= _powerMultiplier;
            builder.Bonuses.RammingDamageMultiplier *= _powerMultiplier;

            return builder;
        }

        public override IItemCollection<IntegratedComponent> Components { get { return _components; } }

        private readonly ObservableCollection<IntegratedComponent> _components = new ObservableCollection<IntegratedComponent>();
        private readonly string _name;
        private readonly float _powerMultiplier;
    }
}
