using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill, ILemmingStateChanger
{
    public static readonly SwimmerSkill Instance = new();

    private SwimmerSkill()
        : base(
            EngineConstants.SwimmerSkillId,
            EngineConstants.SwimmerSkillName)
    {
    }

    public StateChangerType LemmingStateChangerType => StateChangerType.SwimmerStateChanger;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.State.HasLiquidAffinity && ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        lemming.State.IsSwimmer = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
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