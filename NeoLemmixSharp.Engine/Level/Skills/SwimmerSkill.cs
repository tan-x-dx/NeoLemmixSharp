using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class SwimmerSkill : LemmingSkill
{
    public static SwimmerSkill Instance { get; } = new();

    private SwimmerSkill()
    {
    }

    public override int Id => Global.SwimmerSkillId;
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