using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class SliderAction
{
    private const int MaxYCheckOffset = 7;

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var currentActionType = lemming.CurrentActionType;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        if (!SliderTerrainChecks(lemming, orientation, MaxYCheckOffset, in gadgetsNearLemming) &&
            currentActionType == LemmingActionType.DrownerAction)
            return false;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        return SliderTerrainChecks(lemming, orientation, MaxYCheckOffset, in gadgetsNearLemming) ||
               currentActionType != LemmingActionType.DrownerAction;
    }

    public static bool SliderTerrainChecks(
        Lemming lemming,
        Orientation orientation,
        int maxYOffset,
        in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        var lemmingDehoistPosition = lemming.DehoistPin;
        var dx = lemming.FacingDirection.DeltaX;

        var hasPixelAtLemmingPosition = SliderHasPixelAt(in gadgetsNearLemming, lemmingPosition);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(in gadgetsNearLemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            WalkerAction.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(in gadgetsNearLemming, orientation.MoveUp(lemmingPosition, Math.Min(maxYOffset, MaxYCheckOffset))))
        {
            FallerAction.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        foreach (var gadget in gadgetsNearLemming)
        {
            /* if (gadget.GadgetBehaviour != WaterGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                 continue;*/

            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            if (lemming.IsSwimmer)
            {
                SwimmerAction.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_SWIMMING, L.Position); ??
            }
            else
            {
                DrownerAction.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_DROWNING, L.Position); ??
            }
            //water.OnLemmingInHitBox(lemming);

            return true;
        }

        var leftPos = orientation.MoveLeft(lemmingPosition, dx);
        if (!SliderHasPixelAt(in gadgetsNearLemming, leftPos))
            return true;

        lemmingPosition = leftPos;
        WalkerAction.TransitionLemmingToAction(lemming, true);
        return false;

        bool SliderHasPixelAt(
            in GadgetEnumerable gadgetsNearLemming1,
            Point testPosition)
        {
            return PositionIsSolidToLemming(in gadgetsNearLemming1, lemming, testPosition) ||
                   (orientation.MatchesHorizontally(testPosition, lemming.AnchorPosition) &&
                    orientation.MatchesVertically(testPosition, lemmingDehoistPosition) &&
                    PositionIsSolidToLemming(in gadgetsNearLemming1, lemming, orientation.MoveDown(testPosition, 1)));
        }
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new Point(-1, -1);

        LemmingActionType.SliderAction.DoMainTransitionActions(lemming, turnAround);
    }
}
