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
    public static GadgetActionType GetGadgetGadgetActionType(int rawValue)
    {
        var enumValue = (GadgetActionType)rawValue;

        return enumValue switch
        {
            GadgetActionType.SetLemmingState => GadgetActionType.SetLemmingState,
            GadgetActionType.SetLemmingAction => GadgetActionType.SetLemmingAction,
            GadgetActionType.ChangeSkillCount => GadgetActionType.ChangeSkillCount,
            GadgetActionType.ForceFacingDirection => GadgetActionType.ForceFacingDirection,
            GadgetActionType.LemmingMover => GadgetActionType.LemmingMover,
            GadgetActionType.AddLevelTime => GadgetActionType.AddLevelTime,
            GadgetActionType.SetGadgetState => GadgetActionType.SetGadgetState,

            _ => Helpers.ThrowUnknownEnumValueException<GadgetActionType>(rawValue)
        };
    }
}
