using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
    {
    }

    public override int Id => LevelConstants.SwimmerSkillId;
    public override string LemmingSkillName => "swimmer";
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.IsSwimmer && ActionIsAssignable(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSwimmer = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
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
        var isSwimmer = lemmingState.IsSwimmer;
        lemmingState.IsSwimmer = !isSwimmer;
    }

    public bool IsApplied(LemmingState lemmingState)
    {
        return lemmingState.IsSwimmer;
    }
}