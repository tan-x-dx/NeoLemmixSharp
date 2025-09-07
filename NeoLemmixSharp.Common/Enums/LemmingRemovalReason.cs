using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingRemovalReason
{
    None,
    Exit,
    DeathSplat,
    DeathExplode,
    DeathDrown,
    DeathFire,
    DeathTrap,
    DeathVoid,
    DeathZombie,
    DeathWeasel,
    DeathMetalGrate,
    DeathDismemberment
}

public static class LemmingRemovalReasonHelpers
{
    private const int NumberOfEnumValues = 12;

    public static LemmingRemovalReason GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingRemovalReason>(rawValue, NumberOfEnumValues);
}
