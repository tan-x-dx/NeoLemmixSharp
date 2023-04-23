using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static PlatformerSkill Instance { get; } = new();

    private PlatformerSkill()
    {
    }

    public override int LemmingSkillId => 14;
    public override string LemmingSkillName => "platformer";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return (lemming.CurrentAction == WalkerAction.Instance ||
                lemming.CurrentAction == ShruggerAction.Instance ||
                lemming.CurrentAction == BuilderAction.Instance ||
                lemming.CurrentAction == StackerAction.Instance ||
                lemming.CurrentAction == BasherAction.Instance ||
                lemming.CurrentAction == FencerAction.Instance ||
                lemming.CurrentAction == MinerAction.Instance ||
                lemming.CurrentAction == DiggerAction.Instance ||
                lemming.CurrentAction == LasererAction.Instance) &&
               PlatformerAction.LemmingCanPlatform(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        PlatformerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}