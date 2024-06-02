using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ReacherAction : LemmingAction
{
    public static readonly ReacherAction Instance = new();

    private readonly int[] _movementList =
    [
        0, 3, 2, 2, 1, 1, 1, 0
    ];

    private ReacherAction()
        : base(
            LevelConstants.ReacherActionId,
            LevelConstants.ReacherActionName,
            LevelConstants.ReacherAnimationFrames,
            LevelConstants.MaxReacherPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority,
            true,
            true)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 14));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        var emptyPixels = GetEmptyPixelCount(in gadgetsNearRegion, lemming, lemmingPosition);

        // Check for terrain in the body to trigger falling down
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // On the first frame, check as well for height 9, as the shimmier may not continue in that case
        if (lemming.PhysicsFrame == 1 &&
            PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Check whether we can reach the ceiling
        if (emptyPixels <= _movementList[lemming.PhysicsFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1); // Shimmiers are a lot smaller than reachers
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Move upwards
        lemmingPosition = orientation.MoveUp(lemmingPosition, _movementList[lemming.PhysicsFrame]);
        if (lemming.PhysicsFrame == 7)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static int GetEmptyPixelCount(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        var orientation = lemming.Orientation;
        lemmingPosition = orientation.MoveUp(lemmingPosition, 10);
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
            return 0;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
            return 1;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
            return 2;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
            return 3;

        return 4;
    }
}