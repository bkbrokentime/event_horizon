using Constructor.Satellites;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Utils;

namespace Constructor.Ships
{
    public interface IShip
    {
        ItemId<Ship> Id { get; }
        string Name { get; set; }
        ShipColorScheme ColorScheme { get; }

        IShipModel Model { get; }

        IItemCollection<IntegratedComponent> Components { get; }
        ISatellite Satellite_Left_1 { get; set; }
        ISatellite Satellite_Right_1 { get; set; }
        ISatellite Satellite_Left_2 { get; set; }
        ISatellite Satellite_Right_2 { get; set; }
        ISatellite Satellite_Left_3 { get; set; }
        ISatellite Satellite_Right_3 { get; set; }
        ISatellite Satellite_Left_4 { get; set; }
        ISatellite Satellite_Right_4 { get; set; }
        ISatellite Satellite_Left_5 { get; set; }
        ISatellite Satellite_Right_5 { get; set; }

        DifficultyClass ExtraThreatLevel { get; }
        EnhancementLevel ExtraEnhanceLevel { get; }

        Maths.Experience Experience { get; set; }

        ShipBuilder CreateBuilder();

        bool DataChanged { get; set; }

        int RemoveInvalidComponents(IGameItemCollection<ComponentInfo> inventory);
    }
}
