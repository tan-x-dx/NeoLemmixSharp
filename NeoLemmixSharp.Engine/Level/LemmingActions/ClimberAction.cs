using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ClimberAction : LemmingAction
{
    public static readonly ClimberAction Instance = new();

    private ClimberAction()
        : base(
            LemmingActionConstants.ClimberActionId,
            LemmingActionConstants.ClimberActionName,
            LemmingActionConstants.ClimberActionSpriteFileName,
            LemmingActionConstants.ClimberAnimationFrames,
            LemmingActionConstants.MaxClimberPhysicsFrames,
            EngineConstants.PermanentSkillPriority,
            LemmingActionBounds.ClimberActionBounds)
    {
    }

    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var physicsFrame = lemming.PhysicsFrame;

        if (physicsFrame <= 3)
            return InitialFrameChecks(lemming, gadgetsNearLemming, dx, orientation, lemmingPosition, physicsFrame);

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        lemming.IsStartingAction = false;

        var foundClip = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, -dx, 7));

        if (physicsFrame == 7 &&
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 7)))
        {
            foundClip = false;
        }

        if (!foundClip)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        if (lemming.State.IsSlider)
        {
            SliderAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        FallerAction.Instance.TransitionLemmingToAction(lemming, true);

        return true;
    }

    private static bool InitialFrameChecks(
        Lemming lemming,
        GadgetEnumerable gadgetsNearLemming,
        int dx,
        Orientation orientation,
        Point lemmingPosition,
        int physicsFrame)
    {
        var foundClip = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, -dx, 6 + physicsFrame)) ||
                       (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, -dx, 5 + physicsFrame)) &&
                        !lemming.IsStartingAction);

        if (physicsFrame == 0 && // first triggered after 8 frames!
            !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.Move(lemmingPosition, -dx, 7)))
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

            if (lemming.State.IsSlider)
            {
                lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
                SliderAction.Instance.TransitionLemmingToAction(lemming, false);

                return true;
            }

            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
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

        HoisterAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    public override Point GetFootPosition(Lemming lemming, Point anchorPosition)
    {
        return lemming.Orientation.MoveLeft(anchorPosition, lemming.FacingDirection.DeltaX);
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
