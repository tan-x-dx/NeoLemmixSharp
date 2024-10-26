using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
        : base(
            LevelConstants.SwimmerSkillId,
            LevelConstants.SwimmerSkillName)
    {
    }

    public int LemmingStateChangerId => LemmingStateChangerHelper.SwimmerStateChangerId;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity &&
               ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSwimmer = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        return ActionsThatCanBeAssignedPermanentSkill().Append(DrownerAction.Instance);
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