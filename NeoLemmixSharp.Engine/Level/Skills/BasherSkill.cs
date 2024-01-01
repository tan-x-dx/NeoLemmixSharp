using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BasherSkill : LemmingSkill
{
    public static readonly BasherSkill Instance = new();

    private BasherSkill()
    {
    }

    public override int Id => LevelConstants.BasherSkillId;
    public override string LemmingSkillName => "basher";
    public override bool IsClassicSkill => true;

    public override void AssignToLemming(Lemming lemming)
    {
        BasherAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}