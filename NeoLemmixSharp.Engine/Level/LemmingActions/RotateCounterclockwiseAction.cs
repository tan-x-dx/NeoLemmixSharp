using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateCounterclockwiseAction : LemmingAction
{
    public static readonly RotateCounterclockwiseAction Instance = new();

    private RotateCounterclockwiseAction()
        : base(
            EngineConstants.RotateCounterclockwiseActionId,
            EngineConstants.RotateCounterclockwiseActionName,
            EngineConstants.RotateCounterclockwiseActionSpriteFileName,
            EngineConstants.RotateCounterclockwiseAnimationFrames,
            EngineConstants.MaxRotateCounterclockwisePhysicsFrames,
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
            var dx = lemming.FacingDirection.DeltaX;
            lemmingPosition = orientation.Move(lemmingPosition, dx * 4, 4);
            lemming.SetOrientation(orientation.RotateCounterClockwise());
        }

        return true;
    }
}