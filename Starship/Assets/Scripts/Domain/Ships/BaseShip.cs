using System;
using System.Linq;
using Constructor.Satellites;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Maths;
using Utils;

namespace Constructor.Ships
{
    public abstract class BaseShip : IShip
    {
        protected BaseShip(IShipModel model)
        {
            _model = model ?? throw new System.ArgumentException("ShipWrapper is null");
            _experience = 0;
        }

        public ItemId<Ship> Id => _model.Id;

        public virtual string Name { get => _model.OriginalName; set => throw new InvalidOperationException(); }

        public IShipModel Model => _model;
        public ShipColorScheme ColorScheme => _colorScheme;

        public abstract IItemCollection<IntegratedComponent> Components { get; }

        public ISatellite Satellite_Left_1
        {
            get => _Satellite_Left_1;
            set
            {
                _Satellite_Left_1 = value;
                DataChanged = true;
            }
        }

        public ISatellite Satellite_Right_1
        {
            get => _Satellite_Right_1;
            set
            {
                _Satellite_Right_1 = value;
                DataChanged = true;
            }
        }
        public ISatellite Satellite_Left_2
        {
            get => _Satellite_Left_2;
            set
            {
                _Satellite_Left_2 = value;
                DataChanged = true;
            }
        }

        public ISatellite Satellite_Right_2
        {
            get => _Satellite_Right_2;
            set
            {
                _Satellite_Right_2 = value;
                DataChanged = true;
            }
        }
        public ISatellite Satellite_Left_3
        {
            get => _Satellite_Left_3;
            set
            {
                _Satellite_Left_3 = value;
                DataChanged = true;
            }
        }

        public ISatellite Satellite_Right_3
        {
            get => _Satellite_Right_3;
            set
            {
                _Satellite_Right_3 = value;
                DataChanged = true;
            }
        }
        public ISatellite Satellite_Left_4
        {
            get => _Satellite_Left_4;
            set
            {
                _Satellite_Left_4 = value;
                DataChanged = true;
            }
        }

        public ISatellite Satellite_Right_4
        {
            get => _Satellite_Right_4;
            set
            {
                _Satellite_Right_4 = value;
                DataChanged = true;
            }
        }
        public ISatellite Satellite_Left_5
        {
            get => _Satellite_Left_5;
            set
            {
                _Satellite_Left_5 = value;
                DataChanged = true;
            }
        }

        public ISatellite Satellite_Right_5
        {
            get => _Satellite_Right_5;
            set
            {
                _Satellite_Right_5 = value;
                DataChanged = true;
            }
        }

        public virtual DifficultyClass ExtraThreatLevel => DifficultyClass.Default;
        public EnhancementLevel ExtraEnhanceLevel { get; set; }

        public Experience Experience
        {
            get => _experience;
            set
            {
                _experience = value;
                DataChanged = true;
            }
        }

        public virtual ShipBuilder CreateBuilder()
        {
            var builder = new ShipBuilder(this);
            var scale = Experience.PowerMultiplier;
            builder.Bonuses.ArmorPointsMultiplier *= scale;
            builder.Bonuses.ShieldPointsMultiplier *= scale;
            builder.Bonuses.EnergyShieldPointsMultiplier *= scale;
            builder.Bonuses.DamageMultiplier *= scale;
            builder.Bonuses.RammingDamageMultiplier *= scale;

            if (Satellite_Left_1 != null)
                builder.AddSatellite(Satellite_Left_1, CompanionLocation.Left);
            if (Satellite_Right_1 != null)
                builder.AddSatellite(Satellite_Right_1, CompanionLocation.Right);
            if (Satellite_Left_2 != null)
                builder.AddSatellite(Satellite_Left_2, CompanionLocation.Left_2);
            if (Satellite_Right_2 != null)
                builder.AddSatellite(Satellite_Right_2, CompanionLocation.Right_2);
            if (Satellite_Left_3 != null)
                builder.AddSatellite(Satellite_Left_3, CompanionLocation.Left_3);
            if (Satellite_Right_3 != null)
                builder.AddSatellite(Satellite_Right_3, CompanionLocation.Right_3);
            if (Satellite_Left_4 != null)
                builder.AddSatellite(Satellite_Left_4, CompanionLocation.Left_4);
            if (Satellite_Right_4 != null)
                builder.AddSatellite(Satellite_Right_4, CompanionLocation.Right_4);
            if (Satellite_Left_5 != null)
                builder.AddSatellite(Satellite_Left_5, CompanionLocation.Left_5);
            if (Satellite_Right_5 != null)
                builder.AddSatellite(Satellite_Right_5, CompanionLocation.Right_5);

            return builder;
        }

        public bool DataChanged
        {
            get
            {
                if (_dataChanged)
                    return true;
                if (Satellite_Left_1 != null && Satellite_Left_1.DataChanged)
                    return true;
                if (Satellite_Right_1 != null && Satellite_Right_1.DataChanged)
                    return true;
                if (Satellite_Left_2 != null && Satellite_Left_2.DataChanged)
                    return true;
                if (Satellite_Right_2 != null && Satellite_Right_2.DataChanged)
                    return true;
                if (Satellite_Left_3 != null && Satellite_Left_3.DataChanged)
                    return true;
                if (Satellite_Right_3 != null && Satellite_Right_3.DataChanged)
                    return true;
                if (Satellite_Left_4 != null && Satellite_Left_4.DataChanged)
                    return true;
                if (Satellite_Right_4 != null && Satellite_Right_4.DataChanged)
                    return true;
                if (Satellite_Left_5 != null && Satellite_Left_5.DataChanged)
                    return true;
                if (Satellite_Right_5 != null && Satellite_Right_5.DataChanged)
                    return true;
                if (_model.DataChanged)
                    return true;
                if (_colorScheme.IsChanged)
                    return true;

                return false;
            }
            set
            {
                _dataChanged = value;
                if (_dataChanged)
                    return;

                if (Satellite_Left_1 != null)
                    Satellite_Left_1.DataChanged = false;
                if (Satellite_Right_1 != null)
                    Satellite_Right_1.DataChanged = false;
                if (Satellite_Left_2 != null)
                    Satellite_Left_2.DataChanged = false;
                if (Satellite_Right_2 != null)
                    Satellite_Right_2.DataChanged = false;
                if (Satellite_Left_3 != null)
                    Satellite_Left_3.DataChanged = false;
                if (Satellite_Right_3 != null)
                    Satellite_Right_3.DataChanged = false;
                if (Satellite_Left_4 != null)
                    Satellite_Left_4.DataChanged = false;
                if (Satellite_Right_4 != null)
                    Satellite_Right_4.DataChanged = false;
                if (Satellite_Left_5 != null)
                    Satellite_Left_5.DataChanged = false;
                if (Satellite_Right_5 != null)
                    Satellite_Right_5.DataChanged = false;

                _model.DataChanged = false;
                _colorScheme.IsChanged = false;
            }
        }

        public int RemoveInvalidComponents(IGameItemCollection<ComponentInfo> inventory)
        {
            var layout = new ShipLayout(Model.Layout, Model.Barrels, Enumerable.Empty<IntegratedComponent>());
            var index = 0;
            var components = (IItemCollection<IntegratedComponent>)Components.Where(item => item.Layout == 0);
            var count = 0;

            while (index < components.Count)
            {
                var component = components[index];
                if (layout.InstallComponent(component.Info,component.Layout, component.X, component.Y) >= 0)
                {
                    index++;
                    continue;
                }

                components.RemoveAt(index);
                inventory.Add(component.Info);
                count++;
            }

            var Secondlayout = new ShipLayout(Model.SecondLayout, null, Enumerable.Empty<IntegratedComponent>());
            var Secondindex = 0;
            var Secondcomponents = (IItemCollection<IntegratedComponent>)Components.Where(item => item.Layout == 1);
            var Secondcount = 0;
            while (Secondindex < Secondcomponents.Count)
            {
                var Secondcomponent = Secondcomponents[index];
                if (Secondlayout.InstallComponent(Secondcomponent.Info, Secondcomponent.Layout, Secondcomponent.X, Secondcomponent.Y) >= 0)
                {
                    Secondindex++;
                    continue;
                }

                Secondcomponents.RemoveAt(index);
                inventory.Add(Secondcomponent.Info);
                Secondcount++;
            }
            return count + Secondcount;
        }

        private Experience _experience;
        private ISatellite _Satellite_Left_1;
        private ISatellite _Satellite_Right_1;
        private ISatellite _Satellite_Left_2;
        private ISatellite _Satellite_Right_2;
        private ISatellite _Satellite_Left_3;
        private ISatellite _Satellite_Right_3;
        private ISatellite _Satellite_Left_4;
        private ISatellite _Satellite_Right_4;
        private ISatellite _Satellite_Left_5;
        private ISatellite _Satellite_Right_5;
        private bool _dataChanged;
        private readonly ShipColorScheme _colorScheme = new ShipColorScheme();
        private readonly IShipModel _model;
    }
}
