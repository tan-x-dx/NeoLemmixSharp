using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class WalkerSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingSkillType.WalkerSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        // Important! If a builder just placed a brick and part of the previous brick
        // got removed, he should not fall if turned into a walker!
        var testUp = orientation.MoveUp(lemmingPosition, 1);
        var testRight = orientation.MoveRight(lemmingPosition, lemming.FacingDirection.DeltaX);

        var gadgetTestRegion = new RectangularRegion(lemmingPosition, testRight);
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        if (lemming.CurrentActionType == LemmingActionType.BuilderAction &&
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testUp) &&
            !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, testRight))
        {
            lemmingPosition = testUp;

            WalkerAction.TransitionLemmingToAction(lemming, false);

            return;
        }

        if (lemming.CurrentActionType != LemmingActionType.WalkerAction)
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);

            return;
        }

        // Turn around walking lem, if assigned a walker
        lemming.FacingDirection = lemming.FacingDirection.GetOpposite();

        if (LemmingIsForcedToChangeDirection(in gadgetsNearRegion, lemming))
        {
            // Go one back to cancel the horizontal offset in WalkerAction's update method.
            // unless the Lem will fall down (which is handles already in Transition)
            if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            {
                lemmingPosition = testRight;
            }
        }

        WalkerAction.TransitionLemmingToAction(lemming, false);
    }

    private static bool LemmingIsForcedToChangeDirection(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming)
    {
        var allBlockers = LevelScreen.LemmingManager.AllBlockers;

        if (allBlockers.Count > 0)
        {
            foreach (var blocker in allBlockers)
            {

            }
        }

        // Special treatment if in one-way-field facing the wrong direction
        // see http://www.lemmingsforums.net/index.php?topic=2640.0
        var facingDirectionAsOrientation = lemming.FacingDirection.ConvertToRelativeOrientation(lemming.Orientation);

        foreach (var gadget in gadgetsNearRegion)
        {
            /*Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.ForceDirection, lemmingPosition, facingDirectionAsOrientation.GetOpposite())*/
        }

        return false;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.BlockerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.ShimmierAction;
        yield return LemmingActionType.LasererAction;
        yield return LemmingActionType.ReacherAction;
        yield return LemmingActionType.ShruggerAction;
    }
}
