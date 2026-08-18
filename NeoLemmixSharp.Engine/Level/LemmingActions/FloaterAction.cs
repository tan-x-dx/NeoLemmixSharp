using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class FloaterAction
{
    private static ReadOnlySpan<int> FloaterFallTable => [3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2];

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var maxFallDistance = FloaterFallTable[lemming.PhysicsFrame - 1];

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        var updraftFallDelta = GetUpdraftFallDelta(lemming, in gadgetsNearLemming);

        maxFallDistance += updraftFallDelta.Y;

        lemmingPosition = orientation.MoveRight(lemmingPosition, updraftFallDelta.X);

        var groundPixelDistance = Math.Min(FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming), 0);
        if (maxFallDistance > -groundPixelDistance)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, groundPixelDistance);
            lemming.SetNextActionType(LemmingActionType.WalkerAction);
        }
        else
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, maxFallDistance);
        }

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.FloaterAction.DoMainTransitionActions(lemming, turnAround);
}
