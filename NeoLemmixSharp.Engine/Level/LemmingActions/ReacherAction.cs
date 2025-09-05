using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ReacherAction : LemmingAction
{
    public static readonly ReacherAction Instance = new();

    private static ReadOnlySpan<int> MovementList =>
    [
        0, 3, 2, 2, 1, 1, 1, 0
    ];

    private ReacherAction()
        : base(
            LemmingActionConstants.ReacherActionId,
            LemmingActionConstants.ReacherActionName,
            LemmingActionConstants.ReacherActionSpriteFileName,
            LemmingActionConstants.ReacherAnimationFrames,
            LemmingActionConstants.MaxReacherPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.ReacherActionBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Data.Orientation;
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;

        var emptyPixels = GetEmptyPixelCount(in gadgetsNearLemming, lemming, lemmingPosition);

        // Check for terrain in the body to trigger falling down
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // On the first frame, check as well for height 9, as the shimmier may not continue in that case
        if (lemming.Data.PhysicsFrame == 1 &&
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        var movementList = MovementList;

        // Check whether we can reach the ceiling
        if (emptyPixels <= movementList[lemming.Data.PhysicsFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1); // Shimmiers are a lot smaller than reachers
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Move upwards
        lemmingPosition = orientation.MoveUp(lemmingPosition, movementList[lemming.Data.PhysicsFrame]);
        if (lemming.Data.PhysicsFrame == 7)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    private static int GetEmptyPixelCount(
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        Point lemmingPosition)
    {
        var orientation = lemming.Data.Orientation;
        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 10)))
            return 0;

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 11)))
            return 1;

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 12)))
            return 2;

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 13)))
            return 3;

        return 4;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
