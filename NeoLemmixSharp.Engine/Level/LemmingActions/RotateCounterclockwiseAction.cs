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
            LevelConstants.NonWalkerMovementPriority,
            true,
            true)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            lemming.SetOrientation(Orientation.RotateCounterClockwise(lemming.Orientation));
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}