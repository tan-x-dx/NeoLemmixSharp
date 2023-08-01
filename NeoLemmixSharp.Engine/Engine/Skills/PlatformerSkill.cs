using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static PlatformerSkill Instance { get; } = new();

    private PlatformerSkill()
    {
    }

    public override int Id => GameConstants.PlatformerSkillId;
    public override string LemmingSkillName => "platformer";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) &&
               PlatformerAction.LemmingCanPlatform(lemming, lemming.Orientation);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        PlatformerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}