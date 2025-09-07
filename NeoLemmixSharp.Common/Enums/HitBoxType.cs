using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum HitBoxType
{
    Empty,
    ResizableRectangular,
    Rectangular,
    PointSet
}

public static class HitBoxTypeHelpers
{
    private const int NumberOfEnumValues = 4;

    public static HitBoxType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<HitBoxType>(rawValue, NumberOfEnumValues);
}
