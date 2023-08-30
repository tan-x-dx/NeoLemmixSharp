using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class SwimmerSkill : LemmingSkill
{
    public static SwimmerSkill Instance { get; } = new();

    private SwimmerSkill()
    {
    }

    public override int Id => GameConstants.SwimmerSkillId;
    public override string LemmingSkillName => "swimmer";
    public override bool IsPermanentSkill => true;
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
}