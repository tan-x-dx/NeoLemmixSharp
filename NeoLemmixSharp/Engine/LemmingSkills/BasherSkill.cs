using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BasherSkill : LemmingSkill
{
    public BasherSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 0;
    public override string LemmingSkillName => "basher";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        BasherAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}