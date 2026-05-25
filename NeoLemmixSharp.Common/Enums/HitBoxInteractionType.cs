using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum HitBoxInteractionType
{
    None,
    Liquid,
    Updraft,
    Splat,
    NoSplat,

    VALUE_MAX
}

public static class HitBoxInteractionTypeHelpers
{
    private const int NumberOfEnumValues = (int)HitBoxInteractionType.VALUE_MAX;

    public static HitBoxInteractionType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<HitBoxInteractionType>(rawValue, NumberOfEnumValues);
}
