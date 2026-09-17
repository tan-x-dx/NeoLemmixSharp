using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class StackerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingSkillType.StackerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
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

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.LasererAction;
    }
}
