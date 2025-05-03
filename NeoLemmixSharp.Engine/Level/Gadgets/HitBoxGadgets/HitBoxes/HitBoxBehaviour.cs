using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

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

    public static HitBoxBehaviour GetEnumValue(int rawValue) => Helpers.GetEnumValue<HitBoxBehaviour>(rawValue, NumberOfEnumValues);
}
