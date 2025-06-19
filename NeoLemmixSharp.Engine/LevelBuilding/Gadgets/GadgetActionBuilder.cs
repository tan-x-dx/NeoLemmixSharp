using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Util;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetActionBuilder
{
    public static void BuildGadgetActions(
        HitBoxData hitBoxData,
        out GadgetAction[] onLemmingEnterActions,
        out GadgetAction[] onLemmingPresentActions,
        out GadgetAction[] onLemmingExitActions)
    {
        onLemmingEnterActions = BuildGadgetActions(hitBoxData.OnLemmingEnterActions);
        onLemmingPresentActions = BuildGadgetActions(hitBoxData.OnLemmingPresentActions);
        onLemmingExitActions = BuildGadgetActions(hitBoxData.OnLemmingExitActions);
    }

    private static GadgetAction[] BuildGadgetActions(GadgetActionData[] gadgetActions)
    {
        var result = Helpers.GetArrayForSize<GadgetAction>(gadgetActions.Length);

        for (int i = 0; i < result.Length; i++)
        {
            var gadgetAction = gadgetActions[i];
            result[i] = CreateGadgetAction(gadgetAction.GadgetActionType, gadgetAction.MiscData);
        }

        return result;
    }

    private static GadgetAction CreateGadgetAction(GadgetActionType gadgetActionType, int miscData) => gadgetActionType switch
    {
        GadgetActionType.ChangeLemmingState => CreateSetLemmingStateAction(miscData),
        GadgetActionType.ChangeLemmingAction => CreateSetLemmingActionAction(miscData),
        GadgetActionType.KillLemming => CreateKillLemmingAction(miscData),
        GadgetActionType.ChangeSkillCount => CreateSkillCountModifierAction(miscData),
        GadgetActionType.ForceFacingDirection => CreateForceFacingDirectionAction(miscData),
        GadgetActionType.LemmingMover => CreateLemmingMoverAction(miscData),
        GadgetActionType.AddLevelTime => CreateAddLevelTimeAction(miscData),
        GadgetActionType.SetGadgetState => SetGadgetStateAction(miscData),

        _ => Helpers.ThrowUnknownEnumValueException<GadgetActionType, GadgetAction>(gadgetActionType)
    };

    private static SetLemmingStateAction CreateSetLemmingStateAction(int miscData)
    {
        var stateChangerId = miscData & 0xffff;
        var stateChanger = new LemmingStateChangerHasher().UnHash(stateChangerId);

        var rawSetStateType = (uint)(miscData >>> 16);
        var setStateType = SetLemmingStateAction.GetEnumValue(rawSetStateType);

        return new SetLemmingStateAction(stateChanger, setStateType);
    }

    private static SetLemmingActionAction CreateSetLemmingActionAction(int miscData)
    {
        var lemmingAction = new LemmingAction.LemmingActionHasher().UnHash(miscData);
        return new SetLemmingActionAction(lemmingAction);
    }

    private static KillLemmingAction CreateKillLemmingAction(int miscData)
    {
        var lemmingRemovalReason = LemmingRemovalReasonHelpers.GetEnumValue((uint)miscData);
        return new KillLemmingAction(lemmingRemovalReason);
    }

    private static SkillCountModifierAction CreateSkillCountModifierAction(int miscData)
    {
        // 16 bits for skill id
        var rawSkillId = miscData & 0xffff;
        var skill = new LemmingSkill.LemmingSkillHasher().UnHash(rawSkillId);

        // 4 bits for tribe data
        // lower 1 bit is "do we care about tribes at all"
        // upper 3 bits is tribe id assuming we do indeed care about tribes
        var rawTribeId = (miscData >>> 16) & 0x0f;
        int? tribeId = (rawTribeId & 1) != 0 ? (rawTribeId >>> 1) : null;

        // 7 bits for numerical value (gets clamped between 0 and 100)
        var value = (miscData >>> 20) & 0x7f;

        return new SkillCountModifierAction(skill, value, tribeId);
    }

    private static ForceFacingDirectionAction CreateForceFacingDirectionAction(int miscData)
    {
        return ForceFacingDirectionAction.ForFacingDirection(miscData);
    }

    private static LemmingMoverAction CreateLemmingMoverAction(int miscData)
    {
        var delta = ReadWriteHelpers.DecodePoint(miscData);
        return new LemmingMoverAction(delta);
    }

    private static AdditionalTimeAction CreateAddLevelTimeAction(int miscData)
    {
        return new AdditionalTimeAction(miscData);
    }

    private static StateTransitionAction SetGadgetStateAction(int miscData)
    {
        var gadgetId = miscData & 0xffff;
        var stateIndex = miscData >>> 16;

        return new StateTransitionAction(gadgetId, stateIndex);
    }
}
