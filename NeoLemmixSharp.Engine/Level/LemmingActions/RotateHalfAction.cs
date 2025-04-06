using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateHalfAction : LemmingAction
{
    public static readonly RotateHalfAction Instance = new();

    private RotateHalfAction()
        : base(
            EngineConstants.RotateHalfActionId,
            EngineConstants.RotateHalfActionName,
            EngineConstants.RotateHalfActionSpriteFileName,
            EngineConstants.RotateHalfAnimationFrames,
            EngineConstants.MaxRotateHalfPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.AnchorPosition;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 8);
            lemming.SetOrientation(orientation.GetOpposite());
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;
}