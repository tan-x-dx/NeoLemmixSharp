using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public enum HitBoxBehaviour
{
    None,
    Liquid,
    Updraft,
    Splat,
    NoSplat
}

public static class HitBoxBehaviourHelpers
{
    private const int NumberOfEnumValues = 5;

    public static HitBoxBehaviour GetEnumValue(uint rawValue) => Helpers.GetEnumValue<HitBoxBehaviour>(rawValue, NumberOfEnumValues);
}
