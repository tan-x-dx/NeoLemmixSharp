using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingRemovalReason
{
    None,
    Exit,
    DeathSplat,
    DeathExploder,
    DeathStoner,
    DeathDrown,
    DeathFire,
    DeathTrap,
    DeathVoid,
    DeathZombie,
    DeathWeasel,
    DeathMetalGrate,
    DeathDismemberment,

    VALUE_MAX
}

public static class LemmingRemovalReasonHelpers
{
    private const uint NumberOfEnumValues = (uint)LemmingRemovalReason.VALUE_MAX;

    public static LemmingRemovalReason GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingRemovalReason>(rawValue, NumberOfEnumValues);
}
