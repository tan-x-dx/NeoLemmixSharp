using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class AscenderAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        ref var ascenderProgress = ref lemming.AscenderProgress;
        var orientation = lemming.Orientation;

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
            lemming.SetNextActionType(LemmingActionType.WalkerAction);
            return true;
        }

        if ((ascenderProgress == 4 &&
             pixel1IsSolid &&
             pixel2IsSolid) ||
            (ascenderProgress >= 5 &&
             pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.AnchorPosition = orientation.MoveLeft(lemmingPosition, dx);
            FallerAction.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingActionType.AscenderAction.DoMainTransitionActions(lemming, turnAround);

        lemming.AscenderProgress = 0;
    }
}
