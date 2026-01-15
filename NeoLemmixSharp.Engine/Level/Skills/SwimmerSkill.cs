using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingState;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingState
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
        : base(
            LemmingSkillConstants.SwimmerSkillId,
            LemmingSkillConstants.SwimmerSkillName)
    {
    }

    public StateType LemmingStateType => StateType.SwimmerState;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSwimmer = true;
        if (lemming.CurrentActionId == LemmingActionConstants.DrownerActionId)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(DrownerAction.Instance);
        result.UnionWith(ActionsThatCanBeAssignedPermanentSkill);

        return result;
    }

    public void SetLemmingState(LemmingState lemmingState, bool status)
    {
        lemmingState.IsSwimmer = status;
    }

    public void ToggleLemmingState(LemmingState lemmingState)
    {
        lemmingState.IsSwimmer = !lemmingState.IsSwimmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsSwimmer;
    }
}
