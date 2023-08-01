using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class BuilderSkill : LemmingSkill
{
    public static BuilderSkill Instance { get; } = new();

    private BuilderSkill()
    {
    }

    public override int Id => GameConstants.BuilderSkillId;
    public override string LemmingSkillName => "builder";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool AssignToLemming(Lemming lemming)
    {
        BuilderAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
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