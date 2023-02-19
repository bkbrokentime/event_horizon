using System.ComponentModel;
using GameDatabase.Enums;

namespace Constructor
{
    public enum ComponentQuality
    {
        N5,
        N4,
        N3,
        N2,
        N1,
        P0,
        P1,
        P2,
        P3,
        P4,
        P5,
    }

    public static class ComponentQualityExtensions
    {
        public static ComponentQuality Randomize(this ComponentQuality quality, System.Random random)
        {
            var min = quality <= ComponentQuality.N5 ? ComponentQuality.N5 : quality - 1;
            var max = quality >= ComponentQuality.P5 ? ComponentQuality.P5 : quality + 1;
            return (ComponentQuality)random.SquareRange((int) min, (int) max);
        }

        public static int GetLevel(this ComponentQuality quality, int baseLevel)
        {
            switch (quality)
            {
                case ComponentQuality.N5:
                    return 5*baseLevel/10;
                case ComponentQuality.N4:
                    return 6*baseLevel/10;
                case ComponentQuality.N3:
                    return 7*baseLevel/10;
                case ComponentQuality.N2:
                    return 8*baseLevel/10;
                case ComponentQuality.N1:
                    return 9*baseLevel/10;
                case ComponentQuality.P0:
                    return baseLevel;
                case ComponentQuality.P1:
                    return 12*baseLevel/10 + 50;
                case ComponentQuality.P2:
                    return 15*baseLevel/10 + 100;
                case ComponentQuality.P3:
                    return 18*baseLevel/10 + 150;
                case ComponentQuality.P4:
                    return 24*baseLevel/10 + 200;
                case ComponentQuality.P5:
                    return 30*baseLevel/10 + 250;
                default:
                    throw new InvalidEnumArgumentException("quality", (int)quality, typeof(ComponentQuality));
            }
        }

        public static ComponentQuality FromLevel(int level, int baseLevel)
        {
            if (level >= GetLevel(ComponentQuality.P5, baseLevel))
                return ComponentQuality.P5;
            if (level >= GetLevel(ComponentQuality.P4, baseLevel))
                return ComponentQuality.P4;
            if (level >= GetLevel(ComponentQuality.P3, baseLevel))
                return ComponentQuality.P3;
            if (level >= GetLevel(ComponentQuality.P2, baseLevel))
                return ComponentQuality.P2;
            if (level >= GetLevel(ComponentQuality.P1, baseLevel))
                return ComponentQuality.P1;

            if (level <= GetLevel(ComponentQuality.N5, baseLevel))
                return ComponentQuality.N5;
            if (level <= GetLevel(ComponentQuality.N4, baseLevel))
                return ComponentQuality.N4;
            if (level <= GetLevel(ComponentQuality.N3, baseLevel))
                return ComponentQuality.N3;
            if (level <= GetLevel(ComponentQuality.N2, baseLevel))
                return ComponentQuality.N2;
            if (level <= GetLevel(ComponentQuality.N1, baseLevel))
                return ComponentQuality.N1;

            return ComponentQuality.P0;
        }

        public static ModificationQuality ToModificationQuality(this ComponentQuality quality)
        {
            switch (quality)
            {
                case ComponentQuality.N5:
                    return ModificationQuality.N5;
                case ComponentQuality.N4:
                    return ModificationQuality.N4;
                case ComponentQuality.N3:
                    return ModificationQuality.N3;
                case ComponentQuality.N2:
                    return ModificationQuality.N2;
                case ComponentQuality.N1:
                    return ModificationQuality.N1;
                case ComponentQuality.P1:
                    return ModificationQuality.P1;
                case ComponentQuality.P2:
                    return ModificationQuality.P2;
                case ComponentQuality.P3:
                    return ModificationQuality.P3;
                case ComponentQuality.P4:
                    return ModificationQuality.P4;
                case ComponentQuality.P5:
                    return ModificationQuality.P5;
                default:
                    throw new InvalidEnumArgumentException("quality", (int)quality, typeof(ComponentQuality));
            }
        }
    }
}
