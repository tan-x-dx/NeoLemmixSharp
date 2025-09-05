using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public static readonly AscenderAction Instance = new();

    private AscenderAction()
        : base(
            LemmingActionConstants.AscenderActionId,
            LemmingActionConstants.AscenderActionName,
            LemmingActionConstants.AscenderActionSpriteFileName,
            LemmingActionConstants.AscenderAnimationFrames,
            LemmingActionConstants.MaxAscenderPhysicsFrames,
            LemmingActionConstants.NonWalkerMovementPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;
        ref var ascenderProgress = ref lemming.Data.AscenderProgress;
        var orientation = lemming.Data.Orientation;

        var dy = 0;
        while (dy < 2 &&
               ascenderProgress < 5 &&
               PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            dy++;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            ascenderProgress++;
        }

        var pixel1IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1));
        var pixel2IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 2));

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.SetNextAction(WalkerAction.Instance);
            return true;
        }

        if ((ascenderProgress == 4 &&
             pixel1IsSolid &&
             pixel2IsSolid) ||
            (ascenderProgress >= 5 &&
             pixel1IsSolid))
        {
            var dx = lemming.Data.FacingDirection.DeltaX;
            lemming.Data.AnchorPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.Data.AscenderProgress = 0;
    }
}
