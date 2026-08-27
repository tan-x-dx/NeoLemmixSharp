using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class OhNoerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;

        if (lemming.EndOfAnimation)
        {
            LevelScreen.LemmingManager.DeregisterBlocker(lemming);
            var nextAction = lemming.CountDownActionType;
            LemmingAction.TransitionLemmingToAction(lemming, false, nextAction);
            lemming.ClearCountDownAction();
            return !LemmingAction.IsOneTimeAction(nextAction);
        }

        if (PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
            return true;

        var updraftFallDelta = GetUpdraftFallDelta(lemming, in gadgetsNearLemming);

        var lemmingOrientation = lemming.Orientation;
        lemmingPosition = lemmingOrientation.MoveDown(lemmingPosition, EngineConstants.DefaultFallStep + updraftFallDelta.Y);

        return true;
    }

    public static void HandleCountDownTransition(Lemming lemming)
    {
        var currentActionType = lemming.CurrentActionType;

        if (currentActionType == LemmingActionType.NoneAction)
            return;

        if (LemmingAction.IsAirborneAction(currentActionType))
        {
            // If in the air, do the action immediately
            LemmingAction.TransitionLemmingToAction(lemming, false, lemming.CountDownActionType);
            lemming.ClearCountDownAction();
            return;
        }

        TransitionLemmingToAction(lemming, false); // Otherwise start oh-noing!
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.OhNoerAction.DoMainTransitionActions(lemming, turnAround);
}
