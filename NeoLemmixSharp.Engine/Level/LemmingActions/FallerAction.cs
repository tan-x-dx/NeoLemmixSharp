using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class FallerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var currentFallDistanceStep = 0;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        var updraftFallDelta = GetUpdraftFallDelta(lemming, in gadgetsNearLemming);
        var maxFallDistanceStep = EngineConstants.DefaultFallStep + updraftFallDelta.Y;

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        ref var distanceFallen = ref lemming.DistanceFallen;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !PositionIsSolidToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            currentFallDistanceStep++;
            distanceFallen++;
            lemming.TrueDistanceFallen++;

            updraftFallDelta = GetUpdraftFallDelta(lemming, in gadgetsNearLemming);

            if (updraftFallDelta.Y < 0)
            {
                distanceFallen = 0;
            }
            else if (updraftFallDelta.Y > 0)
            {
                distanceFallen = Math.Min(distanceFallen, EngineConstants.MaxFallDistance / 2);
            }
        }

        distanceFallen = Math.Min(distanceFallen, EngineConstants.MaxFallDistance + 1);
        lemming.TrueDistanceFallen = Math.Min(lemming.TrueDistanceFallen, EngineConstants.MaxFallDistance + 1);

        if (currentFallDistanceStep >= maxFallDistanceStep)
            return true;

        var nextAction = IsFallFatal(
            in gadgetsNearLemming,
            lemming)
            ? LemmingActionType.SplatterAction
            : LemmingActionType.WalkerAction;
        lemming.SetNextActionType(nextAction);

        return true;
    }

    [Pure]
    private static bool IsFallFatal(in GadgetEnumerable gadgetEnumerable, Lemming lemming)
    {
        if (lemming.HasSpecialFallingBehaviour)
            return false;

        var anchorPixel = lemming.AnchorPosition;
        var footPixel = lemming.FootPosition;
        var orientation = lemming.Orientation;

        foreach (var gadget in gadgetEnumerable)
        {
            if (!gadget.ContainsEitherPoint(orientation, anchorPixel, footPixel))
                continue;

            var filters = gadget.CurrentState.Filters;

            foreach (var filter in filters)
            {
                if (!filter.MatchesLemming(lemming))
                    continue;

                if (filter.HitBoxBehaviour == HitBoxInteractionType.NoSplat)
                    return false;
                if (filter.HitBoxBehaviour == HitBoxInteractionType.Splat)
                    return true;
            }
        }

        return lemming.DistanceFallen > EngineConstants.MaxFallDistance;
    }

    [Pure]
    private static bool CheckFloaterOrGliderTransition(
        Lemming lemming,
        int currentFallDistance)
    {
        if (lemming.IsFloater &&
            lemming.TrueDistanceFallen > 16 &&
            currentFallDistance == 0)
        {
            FloaterAction.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (!lemming.IsGlider)
            return false;

        if (lemming.TrueDistanceFallen <= 8 &&
            (!lemming.InitialFall ||
             lemming.TrueDistanceFallen <= 6))
            return false;

        GliderAction.TransitionLemmingToAction(lemming, false);
        return true;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var distanceFallen = GetStartingDistanceFallenFromAction(lemming);

        lemming.DistanceFallen = distanceFallen;
        lemming.TrueDistanceFallen = distanceFallen;

        LemmingActionType.FallerAction.DoMainTransitionActions(lemming, turnAround);
    }

    [Pure]
    private static int GetStartingDistanceFallenFromAction(Lemming lemming)
    {
        // For Swimmers it's handled by the SwimmerAction as there is no single universal value
        var currentActionType = lemming.CurrentActionType;

        return currentActionType switch
        {
            LemmingActionType.WalkerAction => 3,
            LemmingActionType.BlockerAction => -1,
            LemmingActionType.BasherAction => 3,
            LemmingActionType.MinerAction => 0,
            LemmingActionType.DiggerAction => 0,
            LemmingActionType.JumperAction => -1,
            LemmingActionType.LasererAction => -1,
            _ => 1
        };
    }
}
