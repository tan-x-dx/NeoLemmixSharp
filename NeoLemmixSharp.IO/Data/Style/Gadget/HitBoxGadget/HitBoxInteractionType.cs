using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

public enum HitBoxInteractionType
{
    None,
    Liquid,
    Updraft,
    Splat,
    NoSplat
}

public static class HitBoxInteractionTypeHelpers
{
    private const int NumberOfEnumValues = 5;

    public static HitBoxInteractionType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<HitBoxInteractionType>(rawValue, NumberOfEnumValues);
}
