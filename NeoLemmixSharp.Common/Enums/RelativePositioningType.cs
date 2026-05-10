using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum RelativePositioningType
{
    RelativeToParentGadget,
    Absolute,

    VALUE_MAX
}

public static class RelativePositioningTypeHelpers
{
    private const int NumberOfGadgetTypeEnumValues = (int)RelativePositioningType.VALUE_MAX;

    public static RelativePositioningType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<RelativePositioningType>(rawValue, NumberOfGadgetTypeEnumValues);
}
