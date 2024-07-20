using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BuilderSkill : LemmingSkill
{
    public static readonly BuilderSkill Instance = new();

    private BuilderSkill()
        : base(
            LevelConstants.BuilderSkillId,
            LevelConstants.BuilderSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        BuilderAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return LasererAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
    }
}