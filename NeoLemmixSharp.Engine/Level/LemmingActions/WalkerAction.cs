using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class WalkerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
        var dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (dy < 0 &&
            lemming.IsSlider &&
            DehoisterAction.LemmingCanDehoist(lemming, true, in gadgetsNearLemming))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            DehoisterAction.TransitionLemmingToAction(lemming, true);
            return true;
        }

        if (dy > EngineConstants.MaxStepUp)
        {
            if (lemming.IsClimber)
            {
                ClimberAction.TransitionLemmingToAction(lemming, false);
            }
            else
            {
                lemming.FacingDirection = lemming.FacingDirection.GetOpposite();
                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            }
        }
        else if (dy > 2)
        {
            AscenderAction.TransitionLemmingToAction(lemming, false);
            lemmingPosition = orientation.MoveUp(lemmingPosition, 2);
        }
        else if (dy >= 0)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, dy);
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming, lemmingPosition, in gadgetsNearLemming);

        if (dy < -3)
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, 4);
            FallerAction.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (dy >= 0)
            return true;

        lemmingPosition = orientation.MoveUp(lemmingPosition, dy);

        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LevelScreen.GadgetManager.GetAllGadgetsNearPosition(lemming.AnchorPosition, out var gadgetsNearRegion);

        if (PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemming.AnchorPosition))
        {
            LemmingActionType.WalkerAction.DoMainTransitionActions(lemming, turnAround);
            return;
        }

        FallerAction.TransitionLemmingToAction(lemming, turnAround);
    }
}
