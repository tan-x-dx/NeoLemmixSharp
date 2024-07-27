using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class JumperSkill : LemmingSkill
{
    public static readonly JumperSkill Instance = new();

    private JumperSkill()
        : base(
            LevelConstants.JumperSkillId,
            LevelConstants.JumperSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        JumperAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return DiggerAction.Instance;
        yield return BuilderAction.Instance;
        yield return BasherAction.Instance;
        yield return MinerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return FencerAction.Instance;
        yield return ClimberAction.Instance;
        yield return SliderAction.Instance;
        yield return LasererAction.Instance;
    }
}