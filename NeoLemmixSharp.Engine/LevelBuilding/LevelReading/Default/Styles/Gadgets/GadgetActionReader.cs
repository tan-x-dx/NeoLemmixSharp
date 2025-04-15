using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

public static class GadgetActionReader
{
    public static void ReadGadgetActions(
        RawFileData rawFileData,
        out IGadgetAction[] onLemmingEnterActions,
        out IGadgetAction[] onLemmingPresentActions,
        out IGadgetAction[] onLemmingExitActions)
    {
        onLemmingEnterActions = ReadGadgetActionSection(rawFileData, 0);
        onLemmingPresentActions = ReadGadgetActionSection(rawFileData, 1);
        onLemmingExitActions = ReadGadgetActionSection(rawFileData, 2);
    }

    private static IGadgetAction[] ReadGadgetActionSection(RawFileData rawFileData, int expectedIdentifierByte)
    {
        int identifierByte = rawFileData.Read8BitUnsignedInteger();
        LevelReadingException.ReaderAssert(identifierByte == expectedIdentifierByte, "Invalid gadget action byte");

        int numberOfItemsInSection = rawFileData.Read8BitUnsignedInteger();
        var result = CollectionsHelper.GetArrayForSize<IGadgetAction>(numberOfItemsInSection);

        var i = 0;
        while (i < result.Length)
        {
            int rawGadgetActionType = rawFileData.Read8BitUnsignedInteger();
            var gadgetActionType = GadgetActionTypeHelpers.GetGadgetActionType(rawGadgetActionType);

            int miscData = rawFileData.Read32BitSignedInteger();

            result[i++] = CreateGadgetAction(gadgetActionType, miscData);
        }

        return result;
    }

    private static IGadgetAction CreateGadgetAction(GadgetActionType gadgetActionType, int miscData) => gadgetActionType switch
    {
        GadgetActionType.SetLemmingState => CreateSetLemmingStateAction(miscData),
        GadgetActionType.SetLemmingAction => CreateSetLemmingActionAction(miscData),
        GadgetActionType.ChangeSkillCount => CreateSkillCountModifierAction(miscData),
        GadgetActionType.ForceFacingDirection => CreateForceFacingDirectionAction(miscData),
        GadgetActionType.LemmingMover => CreateLemmingMoverAction(miscData),
        GadgetActionType.AddLevelTime => CreateAddLevelTimeAction(miscData),
        GadgetActionType.SetGadgetState => SetGadgetStateAction(miscData),

        _ => Helpers.ThrowUnknownEnumValueException<GadgetActionType, IGadgetAction>((int)gadgetActionType)
    };

    private static SetLemmingStateAction CreateSetLemmingStateAction(int miscData)
    {
        var stateChangerId = miscData & 0xffff;
        var stateChanger = new LemmingStateChangerHasher().UnHash(stateChangerId);

        var rawSetStateType = miscData >>> 16;
        var setStateType = SetLemmingStateAction.GetSetStateType(rawSetStateType);

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

        var rawTeamId = (miscData >>> 4) & 0x0f;
        int? teamId = (rawTeamId & 1) != 0 ? (rawTeamId >>> 1) : null;
        var value = (miscData >>> 7) & 0x7f;
        var isDelta = ((miscData >>> 8) & 1) != 0;

        return new SkillCountModifierAction(skill, value, teamId, isDelta);
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

