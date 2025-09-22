using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum RelativePositioningType
{
    RelativeToParentGadget,
    Absolute,
}

public static class RelativePositioningTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = 2;

    public static RelativePositioningType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<RelativePositioningType>(rawValue, NumberOfGadgetTypeEnumValues);
}
