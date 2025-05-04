using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;

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

    public static HitBoxType GetEnumValue(int rawValue) => Helpers.GetEnumValue<HitBoxType>(rawValue, NumberOfEnumValues);
}
