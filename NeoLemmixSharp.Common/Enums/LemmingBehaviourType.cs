using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Enums;

public enum LemmingBehaviourType
{
    SetLemmingState,
    ClearLemmingStates,
    SetLemmingAction,
    SetLemmingTribe,
    SkillCountChange,
    KillLemming,
    ForceLemmingFacingDirection,
    NullifyLemmingFallDistance,
    MoveLemming,
    SetLemmingPosition,
    SetLemmingFastForward,

    VALUE_MAX
}

public static class LemmingBehaviourTypeHelpers
{
    private const int NumberOfEnumValues = (int)LemmingBehaviourType.VALUE_MAX;

    public static LemmingBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingBehaviourType>(rawValue, NumberOfEnumValues);
}
