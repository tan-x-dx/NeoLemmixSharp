using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static readonly StackerSkill Instance = new();

    private StackerSkill()
        : base(
            LemmingSkillType.StackerSkill,
            LemmingSkillConstants.StackerSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(
            lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX),
            out var gadgetsNearRegion);

        lemming.StackLow = !PositionIsSolidToLemming(
            in gadgetsNearRegion,
            lemming,
            lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX));

        StackerAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
