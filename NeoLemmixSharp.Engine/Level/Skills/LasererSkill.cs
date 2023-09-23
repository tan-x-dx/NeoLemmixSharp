using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class LasererSkill : LemmingSkill
{
    public static LasererSkill Instance { get; } = new();

    private LasererSkill()
    {
    }

    public override int Id => Global.LasererSkillId;
    public override string LemmingSkillName => "laserer";
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        LasererAction.Instance.TransitionLemmingToAction(lemming, false);
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
    }
}