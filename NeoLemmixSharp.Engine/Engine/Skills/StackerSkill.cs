using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static StackerSkill Instance { get; } = new();

    private StackerSkill()
    {
    }

    public override int Id => 10;
    public override string LemmingSkillName => "stacker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        lemming.StackLow = !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX), lemming);

        StackerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}