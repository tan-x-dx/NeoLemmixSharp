using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class RotateHalfSkill : LemmingSkill
{
    public static readonly RotateHalfSkill Instance = new();

    private RotateHalfSkill()
        : base(
            LevelConstants.RotateHalfSkillId,
            LevelConstants.RotateHalfSkillName,
            false)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        RotateHalfAction.Instance.TransitionLemmingToAction(lemming, false);
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