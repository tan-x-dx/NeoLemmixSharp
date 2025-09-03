using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class RotateHalfAction : LemmingAction
{
    public static readonly RotateHalfAction Instance = new();

    private RotateHalfAction()
        : base(
            LemmingActionConstants.RotateHalfActionId,
            LemmingActionConstants.RotateHalfActionName,
            LemmingActionConstants.RotateHalfActionSpriteFileName,
            LemmingActionConstants.RotateHalfAnimationFrames,
            LemmingActionConstants.MaxRotateHalfPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.StandardLemmingBounds)
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

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
