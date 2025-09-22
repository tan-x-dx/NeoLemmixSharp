using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingBehaviourType
{
    SetLemmingState,
    ClearLemmingStates,
    SetLemmingAction,
    KillLemming,
    ForceLemmingFacingDirection,
    NullifyLemmingFallDistance,
    MoveLemming,
    SetLemmingPosition,
    SetLemmingFastForward
}

public static class LemmingBehaviourTypeHelpers
{
    private const int NumberOfEnumValues = 9;

    public static LemmingBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingBehaviourType>(rawValue, NumberOfEnumValues);
}
