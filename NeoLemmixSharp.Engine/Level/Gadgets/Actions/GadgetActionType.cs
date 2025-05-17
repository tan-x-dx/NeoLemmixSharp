using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public enum GadgetActionType
{
    SetLemmingState,
    SetLemmingAction,
    ChangeSkillCount,
    ForceFacingDirection,
    NullifyFallDistance,
    LemmingMover,

    AddLevelTime,
    SetGadgetState
}

public static class GadgetActionTypeHelpers
{
    private const int NumberOfEnumValues = 8;

    public static GadgetActionType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetActionType>(rawValue, NumberOfEnumValues);
}
