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
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var orientation = lemming.Orientation;

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            dy++;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 1));
        var pixel2IsSolid = PositionIsSolidToLemming(in gadgetsNearLemming, lemming, orientation.MoveUp(lemmingPosition, 2));

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.SetNextAction(WalkerAction.Instance);
            return true;
        }

        if ((lemming.AscenderProgress == 4 &&
             pixel1IsSolid &&
             pixel2IsSolid) ||
            (lemming.AscenderProgress >= 5 &&
             pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.AnchorPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        DoMainTransitionActions(lemming, turnAround);

        lemming.AscenderProgress = 0;
    }
}