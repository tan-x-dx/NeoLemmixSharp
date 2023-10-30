using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BlockerSkill : LemmingSkill
{
    public static BlockerSkill Instance { get; } = new();

    private BlockerSkill()
    {
    }

    public override int Id => LevelConstants.BlockerSkillId;
    public override string LemmingSkillName => "blocker";
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) && LevelConstants.LemmingManager.CanAssignBlocker(lemming);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        BlockerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}