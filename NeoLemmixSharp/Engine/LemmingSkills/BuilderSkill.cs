using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class BuilderSkill : LemmingSkill
{
    public static BuilderSkill Instance { get; } = new();

    private BuilderSkill()
    {
    }

    public override int LemmingSkillId => 3;
    public override string LemmingSkillName => "builder";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        BuilderAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}