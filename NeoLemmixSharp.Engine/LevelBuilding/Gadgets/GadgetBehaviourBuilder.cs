using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Util;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;
/*
public sealed class GadgetBehaviourBuilder
{
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public GadgetBehaviourBuilder(
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public void BuildGadgetBehaviourIds(
        HitBoxData hitBoxData,
        out int[] onLemmingEnterBehaviourIds,
        out int[] onLemmingPresentBehaviourIds,
        out int[] onLemmingExitBehaviourIds)
    {
        onLemmingEnterBehaviourIds = BuildGadgetBehaviours(hitBoxData.InnateOnLemmingEnterActions);
        onLemmingPresentBehaviourIds = BuildGadgetBehaviours(hitBoxData.InnateOnLemmingPresentActions);
        onLemmingExitBehaviourIds = BuildGadgetBehaviours(hitBoxData.InnateOnLemmingExitActions);
    }

    private int[] BuildGadgetBehaviours(GadgetActionData[] gadgetBehaviourIds)
    {
        if (gadgetBehaviourIds.Length == 0)
            return [];

        var result = new int[gadgetBehaviourIds.Length];

        for (int i = 0; i < result.Length; i++)
        {
            var gadgetAction = gadgetBehaviourIds[i];
            var newLemmingBehaviour = CreateLemmingBehaviour(gadgetAction.GadgetActionType, gadgetAction.MiscData);

            result[i] = newLemmingBehaviour.Id;
            _gadgetBehaviours.Add(newLemmingBehaviour);
        }

        return result;
    }

    private LemmingBehaviour CreateLemmingBehaviour(LemmingBehaviourType gadgetActionType, int miscData) => gadgetActionType switch
    {
        LemmingBehaviourType.ChangeLemmingState => CreateSetLemmingStateBehaviours(miscData),
        LemmingBehaviourType.ChangeLemmingAction => CreateSetLemmingActionBehaviours(miscData),
        LemmingBehaviourType.KillLemming => CreateKillLemmingBehaviours(miscData),
        LemmingBehaviourType.ForceFacingDirection => CreateForceFacingDirectionBehaviours(miscData),
        LemmingBehaviourType.LemmingMover => CreateLemmingMoverBehaviours(miscData),
        //LemmingBehaviourType.ChangeSkillCount => CreateSkillCountModifierBehaviours(miscData),
        //GadgetActionType.AddLevelTime => CreateAddLevelTimeBehaviours(miscData),
        //GadgetActionType.SetGadgetState => SetGadgetStateBehaviours(miscData),

        _ => Helpers.ThrowUnknownEnumValueException<LemmingBehaviourType, LemmingBehaviour>(gadgetActionType)
    };

    private SetLemmingStateBehaviour CreateSetLemmingStateBehaviours(int miscData)
    {
        var stateChangerId = miscData & 0xffff;
        var stateChanger = new LemmingStateChangerHasher().UnHash(stateChangerId);

        var rawSetStateType = (uint)(miscData >>> 16);
        var setStateType = SetLemmingStateBehaviour.GetEnumValue(rawSetStateType);

        return new SetLemmingStateBehaviour(stateChanger, setStateType);
    }

    private SetLemmingActionBehaviour CreateSetLemmingActionBehaviours(int miscData)
    {
        var lemmingAction = new LemmingAction.LemmingActionHasher().UnHash(miscData);
        return new SetLemmingActionBehaviour(lemmingAction);
    }

    private KillLemmingBehaviour CreateKillLemmingBehaviours(int miscData)
    {
        var lemmingRemovalReason = LemmingRemovalReasonHelpers.GetEnumValue((uint)miscData);
        return new KillLemmingBehaviour(lemmingRemovalReason);
    }

    private ForceFacingDirectionBehaviour CreateForceFacingDirectionBehaviours(int miscData)
    {
        return ForceFacingDirectionBehaviour.ForFacingDirection(miscData);
    }

    private LemmingMoverBehaviour CreateLemmingMoverBehaviours(int miscData)
    {
        var delta = ReadWriteHelpers.DecodePoint(miscData);
        return new LemmingMoverBehaviour(delta);
    }

    private static SkillCountModifierBehaviour CreateSkillCountModifierBehaviours(int miscData)
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

        return new SkillCountModifierBehaviour(-1, skill, value, tribeId);
    }

    private AdditionalTimeBehaviour CreateAddLevelTimeBehaviours(int miscData)
    {
        return new AdditionalTimeBehaviour(-1, 0, miscData);
    }

    private StateTransitionBehaviour SetGadgetStateBehaviours(int miscData)
    {
        var gadgetId = miscData & 0xffff;
        var stateIndex = miscData >>> 16;

        return new StateTransitionBehaviour(-1, 0, gadgetId, stateIndex);
    }
}
*/