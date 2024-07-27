using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static readonly StackerSkill Instance = new();

    private StackerSkill()
        : base(
            LevelConstants.StackerSkillId,
            LevelConstants.StackerSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllGadgetsForPosition(lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));

        lemming.StackLow = !LemmingAction.PositionIsSolidToLemming(
            in gadgetsNearRegion,
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