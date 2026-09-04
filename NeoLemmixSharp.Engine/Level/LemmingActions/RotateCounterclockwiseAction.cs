using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class RotateCounterclockwiseAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.AnchorPosition;
            var dx = lemming.FacingDirection.DeltaX;
            lemmingPosition = orientation.Move(lemmingPosition, new(dx * 4, 4));
            lemming.Orientation = orientation.RotateCounterClockwise();
        }

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.RotateCounterclockwiseAction.DoMainTransitionActions(lemming, turnAround);
}
