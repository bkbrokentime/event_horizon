using GameDatabase.Serializable;

namespace GameDatabase.DataModel
{
    public partial class UAVLaunchPlatform
    {
        public UAVLaunchPlatform(UAVLaunchPlatformSerializable serializable, Database.Loader loader, int positionInLayout)
            : this(serializable, loader)
        {
            PositionInLayout = positionInLayout;
        }

        public readonly int PositionInLayout;

        public static readonly UAVLaunchPlatform Empty = new UAVLaunchPlatform();
        private UAVLaunchPlatform() { }
    }

}
