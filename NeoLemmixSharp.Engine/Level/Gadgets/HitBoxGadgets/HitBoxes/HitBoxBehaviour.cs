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
    public static HitBoxBehaviour GetEnumValue(int rawValue)
    {
        var enumValue = (HitBoxBehaviour)rawValue;

        return enumValue switch
        {
            HitBoxBehaviour.None => HitBoxBehaviour.None,
            HitBoxBehaviour.Liquid => HitBoxBehaviour.Liquid,
            HitBoxBehaviour.Updraft => HitBoxBehaviour.Updraft,
            HitBoxBehaviour.Splat => HitBoxBehaviour.Splat,
            HitBoxBehaviour.NoSplat => HitBoxBehaviour.NoSplat,

            _ => Helpers.ThrowUnknownEnumValueException<HitBoxBehaviour>(rawValue)
        };
    }
}
