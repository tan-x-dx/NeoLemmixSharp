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
            EngineConstants.ReacherActionId,
            EngineConstants.ReacherActionName,
            EngineConstants.ReacherActionSpriteFileName,
            EngineConstants.ReacherAnimationFrames,
            EngineConstants.MaxReacherPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

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
        if (lemming.PhysicsFrame == 1 &&
            PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        var movementList = MovementList;

        // Check whether we can reach the ceiling
        if (emptyPixels <= movementList[lemming.PhysicsFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1); // Shimmiers are a lot smaller than reachers
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        // Move upwards
        lemmingPosition = orientation.MoveUp(lemmingPosition, movementList[lemming.PhysicsFrame]);
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
        in GadgetEnumerable gadgetsNearLemming,
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        var orientation = lemming.Orientation;
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
}