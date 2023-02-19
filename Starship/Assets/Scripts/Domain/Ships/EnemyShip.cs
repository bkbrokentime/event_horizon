using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Utils;

namespace Constructor.Ships
{
    public class EnemyShip : BaseShip
    {
        public EnemyShip(ShipBuild data) 
            : base(new ShipModel(data.Ship, data.BuildFaction))
        {
            
            _components.Assign(data.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase));
            _extraThreatLevel = data.DifficultyClass;
            //ship.NoEnhancementLevel? ship.DefaultEnhancementLevel: (EnhancementLevel)UnityEngine.Random.Range((int)EnhancementLevel.Level1, (int)EnhancementLevel.Level100)
            //_extraEnhanceLevel = data.NoEnhancementLevel ? data.DefaultEnhancementLevel : (EnhancementLevel)UnityEngine.Random.Range((int)EnhancementLevel.Level1, (int)EnhancementLevel.Level100);
        }

        public override DifficultyClass ExtraThreatLevel { get { return _extraThreatLevel; } }
        //public override EnhancementLevel ExtraEnhanceLevel { get { return _extraEnhanceLevel; } }

        public override IItemCollection<IntegratedComponent> Components { get { return _components.AsReadOnly(); } }

        public override ShipBuilder CreateBuilder()
        {
            var builder = base.CreateBuilder();

            if (ExtraThreatLevel != DifficultyClass.Default || ExtraEnhanceLevel != EnhancementLevel.Default)
                builder.Converter = new EnemyComponentConverter(Experience.Level, new System.Random((int)Experience));

            return builder;
        }

        private readonly ObservableCollection<IntegratedComponent> _components = new ObservableCollection<IntegratedComponent>();
        private readonly DifficultyClass _extraThreatLevel;
        //private readonly EnhancementLevel _extraEnhanceLevel;
    }
}
