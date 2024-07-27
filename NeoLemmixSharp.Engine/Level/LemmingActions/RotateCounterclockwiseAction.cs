using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateCounterclockwiseAction : LemmingAction
{
    public static readonly RotateCounterclockwiseAction Instance = new();

    private RotateCounterclockwiseAction()
        : base(
            LevelConstants.RotateCounterclockwiseActionId,
            LevelConstants.RotateCounterclockwiseActionName,
            LevelConstants.RotateCounterclockwiseActionSpriteFileName,
            LevelConstants.RotateCounterclockwiseAnimationFrames,
            LevelConstants.MaxRotateCounterclockwisePhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.LevelPosition;
            var dx = lemming.FacingDirection.DeltaX;
            lemmingPosition = orientation.Move(lemmingPosition, dx * 4, 4);
            lemming.SetOrientation(Orientation.RotateCounterClockwise(orientation));
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}