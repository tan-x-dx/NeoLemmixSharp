using NeoLemmixSharp.Engine.Actions;

namespace NeoLemmixSharp.Engine.Skills;

public sealed class DiggerSkill : LemmingSkill
{
    public DiggerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 6;
    public override string LemmingSkillName => "digger";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return (lemming.CurrentAction == WalkerAction.Instance ||
                lemming.CurrentAction == ShruggerAction.Instance ||
                lemming.CurrentAction == PlatformerAction.Instance ||
                lemming.CurrentAction == BuilderAction.Instance ||
                lemming.CurrentAction == StackerAction.Instance ||
                lemming.CurrentAction == BasherAction.Instance ||
                lemming.CurrentAction == FencerAction.Instance ||
                lemming.CurrentAction == MinerAction.Instance ||
                lemming.CurrentAction == LasererAction.Instance)
               && (true); // HasIndestructibleAt ;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        DiggerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}