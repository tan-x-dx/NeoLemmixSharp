using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public enum GadgetActionType
{
    SetLemmingState,
    SetLemmingAction,
    ChangeSkillCount,
    ForceFacingDirection,
    LemmingMover,

    AddLevelTime,
    SetGadgetState
}

public static class GadgetActionTypeHelpers
{
    private const int NumberOfEnumValues = 7;

    public static GadgetActionType GetEnumValue(int rawValue) => Helpers.GetEnumValue<GadgetActionType>(rawValue, NumberOfEnumValues);
}
