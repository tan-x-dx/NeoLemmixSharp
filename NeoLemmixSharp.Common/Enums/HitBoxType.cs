using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum HitBoxType
{
    Empty,
    ResizableRectangular,
    Rectangular,
    PointSet,

    VALUE_MAX
}

public static class HitBoxTypeHelpers
{
    private const int NumberOfEnumValues = (int)HitBoxType.VALUE_MAX;

    public static HitBoxType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<HitBoxType>(rawValue, NumberOfEnumValues);
}
