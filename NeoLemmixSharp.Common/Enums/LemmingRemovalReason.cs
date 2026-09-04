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

    public static bool IsDeath(this LemmingRemovalReason reason)
    {
        return reason is LemmingRemovalReason.DeathSplat or
                         LemmingRemovalReason.DeathExploder or
                         LemmingRemovalReason.DeathStoner or
                         LemmingRemovalReason.DeathDrown or
                         LemmingRemovalReason.DeathFire or
                         LemmingRemovalReason.DeathTrap or
                         LemmingRemovalReason.DeathVoid or
                         LemmingRemovalReason.DeathZombie or
                         LemmingRemovalReason.DeathWeasel or
                         LemmingRemovalReason.DeathMetalGrate or
                         LemmingRemovalReason.DeathDismemberment;
    }
}
