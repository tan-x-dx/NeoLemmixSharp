using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class RotateHalfAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        if (lemming.EndOfAnimation)
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);
            var orientation = lemming.Orientation;
            ref var lemmingPosition = ref lemming.AnchorPosition;
            lemmingPosition = orientation.MoveUp(lemmingPosition, 8);
            lemming.Orientation = orientation.GetOpposite();
        }

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.RotateHalfAction.DoMainTransitionActions(lemming, turnAround);
}
