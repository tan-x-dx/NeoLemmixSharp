using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public enum LemmingBehaviourType
{
    ChangeLemmingState,
    ChangeLemmingAction,
    KillLemming,
    ForceFacingDirection,
    NullifyFallDistance,
    LemmingMover,

    //ChangeSkillCount,
    //AddLevelTime,
    //SetGadgetState
}

public static class LemmingBehaviourTypeHelpers
{
    private const int NumberOfEnumValues = 6;

    public static LemmingBehaviourType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<LemmingBehaviourType>(rawValue, NumberOfEnumValues);
}
