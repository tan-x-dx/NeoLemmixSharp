using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
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
            LevelConstants.ReacherActionId,
            LevelConstants.ReacherActionName,
            LevelConstants.ReacherActionSpriteFileName,
            LevelConstants.ReacherAnimationFrames,
            LevelConstants.MaxReacherPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 14));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        var emptyPixels = GetEmptyPixelCount(in gadgetsNearRegion, lemming, lemmingPosition);

        // Check for terrain in the body to trigger falling down
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // On the first frame, check as well for height 9, as the shimmier may not continue in that case
        if (lemming.PhysicsFrame == 1 &&
            PositionIsSolidToLemming(in gadgetsNearRegion, lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Check whether we can reach the ceiling
        if (emptyPixels <= MovementList[lemming.PhysicsFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1); // Shimmiers are a lot smaller than reachers
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Move upwards
        lemmingPosition = orientation.MoveUp(lemmingPosition, MovementList[lemming.PhysicsFrame]);
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
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            return 0;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            return 1;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            return 2;

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
            return 3;

        return 4;
    }
}