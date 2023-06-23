using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class LasererSkill : LemmingSkill
{
    public LasererSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 12;
    public override string LemmingSkillName => "laserer";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        LasererAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}