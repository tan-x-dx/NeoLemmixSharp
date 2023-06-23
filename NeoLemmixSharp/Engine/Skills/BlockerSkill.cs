using NeoLemmixSharp.Engine.Actions;

namespace NeoLemmixSharp.Engine.Skills;

public sealed class BlockerSkill : LemmingSkill
{
    public BlockerSkill(int originalNumberOfSkillsAvailable) : base(originalNumberOfSkillsAvailable)
    {
    }

    public override int LemmingSkillId => 1;
    public override string LemmingSkillName => "blocker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return (true) && // CheckForOverlappingField
               lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        BlockerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}