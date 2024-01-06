using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static readonly StackerSkill Instance = new();

    private StackerSkill()
    {
    }

    public override int Id => LevelConstants.StackerSkillId;
    public override string LemmingSkillName => "stacker";
    public override bool IsClassicSkill => false;

    public override void AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        lemming.StackLow = !LevelScreen.TerrainManager.PixelIsSolidToLemming(
            lemming,
            lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));

        StackerAction.Instance.TransitionLemmingToAction(lemming, false);
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