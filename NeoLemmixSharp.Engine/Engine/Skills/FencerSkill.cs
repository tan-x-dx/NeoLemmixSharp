using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class FencerSkill : LemmingSkill
{
    public static FencerSkill Instance { get; } = new();

    private FencerSkill()
    {
    }

    public override int Id => 11;
    public override string LemmingSkillName => "fencer";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;
    
    public override bool AssignToLemming(Lemming lemming)
    {
        FencerAction.Instance.TransitionLemmingToAction(lemming, false);
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
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}