using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetActionBuilder
{
    public static void ReadGadgetActions(
        HitBoxData hitBoxData,
        out GadgetAction[] onLemmingEnterActions,
        out GadgetAction[] onLemmingPresentActions,
        out GadgetAction[] onLemmingExitActions)
    {
        onLemmingEnterActions = ReadGadgetActionSection(hitBoxData.OnLemmingEnterActions);
        onLemmingPresentActions = ReadGadgetActionSection(hitBoxData.OnLemmingPresentActions);
        onLemmingExitActions = ReadGadgetActionSection(hitBoxData.OnLemmingExitActions);
    }

    private static GadgetAction[] ReadGadgetActionSection(GadgetActionData[] gadgetActions)
    {
        var result = CollectionsHelper.GetArrayForSize<GadgetAction>(gadgetActions.Length);

        for (int i = 0; i < result.Length; i++)
        {
            var gadgetAction = gadgetActions[i];
            result[i] = CreateGadgetAction(gadgetAction.GadgetActionType, gadgetAction.MiscData);
        }

        return result;
    }

    private static GadgetAction CreateGadgetAction(GadgetActionType gadgetActionType, int miscData) => gadgetActionType switch
    {
        GadgetActionType.SetLemmingState => CreateSetLemmingStateAction(miscData),
        GadgetActionType.SetLemmingAction => CreateSetLemmingActionAction(miscData),
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

    private static SkillCountModifierAction CreateSkillCountModifierAction(int miscData)
    {
        var rawSkillId = miscData & 0x0f;
        var skill = new LemmingSkill.LemmingSkillHasher().UnHash(rawSkillId);

        var rawTribeId = (miscData >>> 4) & 0x0f;
        int? tribeId = (rawTribeId & 1) != 0 ? (rawTribeId >>> 1) : null;
        var value = (miscData >>> 7) & 0x7f;
        var isDelta = ((miscData >>> 8) & 1) != 0;

        return new SkillCountModifierAction(skill, value, tribeId, isDelta);
    }

    private static ForceFacingDirectionAction CreateForceFacingDirectionAction(int miscData)
    {
        var facingDirection = new FacingDirection(miscData);
        return new ForceFacingDirectionAction(facingDirection);
    }

    private static LemmingMoverAction CreateLemmingMoverAction(int miscData)
    {
        var dx = (miscData & 0xffff) - 0x10000;
        var dy = (miscData >>> 16) - 0x10000;

        return new LemmingMoverAction(new Point(dx, dy));
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
