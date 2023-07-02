using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class SwimmerSkill : LemmingSkill
{
    public SwimmerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 19;
    public override string LemmingSkillName => "swimmer";
    public override bool IsPermanentSkill => true;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return !lemming.IsSwimmer && (lemming.CurrentAction.CanBeAssignedPermanentSkill || lemming.CurrentAction == DrownerAction.Instance);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        lemming.IsSwimmer = true;
        if (lemming.CurrentAction == DrownerAction.Instance)
        {
            SwimmerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}