using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class ClimberAction
{
    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var physicsFrame = lemming.PhysicsFrame;

        if (physicsFrame <= 3)
            return InitialFrameChecks(lemming, gadgetsNearLemming, dx, orientation, ref lemmingPosition, physicsFrame);

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        lemming.IsStartingAction = false;

        var foundClip = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(-dx, 7)));

        if (physicsFrame == 7 &&
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 7)))
        {
            foundClip = false;
        }

        if (!foundClip)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        if (lemming.IsSlider)
        {
            SliderAction.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        FallerAction.TransitionLemmingToAction(lemming, true);

        return true;
    }

    private static bool InitialFrameChecks(
        Lemming lemming,
        GadgetEnumerable gadgetsNearLemming,
        int dx,
        Orientation orientation,
        ref Point lemmingPosition,
        int physicsFrame)
    {
        var foundClip = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(-dx, 6 + physicsFrame))) ||
                       (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(-dx, 5 + physicsFrame))) &&
                        !lemming.IsStartingAction);

        if (physicsFrame == 0 && // first triggered after 8 frames!
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, new(-dx, 7))))
        {
            foundClip = false;
        }

        if (foundClip)
        {
            // Don't fall below original position on hitting terrain in first cycle
            if (!lemming.IsStartingAction)
            {
                lemmingPosition = orientation.MoveUp(lemmingPosition, 3 - physicsFrame);
            }

            if (lemming.IsSlider)
            {
                lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
                SliderAction.TransitionLemmingToAction(lemming, false);

                return true;
            }

            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.TransitionLemmingToAction(lemming, true);
            lemming.DistanceFallen++; // Least-impact way to fix a fall distance inconsistency. See https://www.lemmingsforums.net/index.php?topic=5794.0

            return true;
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 7 + physicsFrame)))
            return true;

        // if-case prevents too deep bombing, see http://www.lemmingsforums.net/index.php?topic=2620.0
        if (!(lemming.IsStartingAction && physicsFrame == 1))
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, physicsFrame - 2);
            lemming.IsStartingAction = false;
        }

        HoisterAction.TransitionLemmingToAction(lemming, false);

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.ClimberAction.DoMainTransitionActions(lemming, turnAround);
}
