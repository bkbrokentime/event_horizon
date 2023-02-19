using Economy.ItemType;
using GameDatabase.Enums;

namespace Model
{
    //public enum ItemQuality
    //{
    //    Low5,
    //    Low4,
    //    Low3,
    //    Low2,
    //    Low1,
    //    Common,
    //    Medium,
    //    High,
    //    Perfect,
    //    Epic,
    //    Legend,
    //}

    public static class QualityTypeExtension
    {
        public static string Name(this ItemQuality type)
        {
            switch (type)
            {
                case ItemQuality.Low5:
                    return "$QualityLow5";
                case ItemQuality.Low4:
                    return "$QualityLow4";
                case ItemQuality.Low3:
                    return "$QualityLow3";
                case ItemQuality.Low2:
                    return "$QualityLow2";
                case ItemQuality.Low1:
                    return "$QualityLow1";
                case ItemQuality.Common:
                    return "$QualityCommon";
                case ItemQuality.Medium:
                    return "$QualityMedium";
                case ItemQuality.High:
                    return "$QualityHigh";
                case ItemQuality.Perfect:
                    return "$QualityPerfect";
                case ItemQuality.Epic:
                    return "$QualityEpic";
                case ItemQuality.Legend:
                    return "$QualityLegend";
                default:
                    return "";
            }
        }
    }
}
