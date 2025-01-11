﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FallerAction : LemmingAction
{
    public static readonly FallerAction Instance = new();

    private FallerAction()
        : base(
            EngineConstants.FallerActionId,
            EngineConstants.FallerActionName,
            EngineConstants.FallerActionSpriteFileName,
            EngineConstants.FallerAnimationFrames,
            EngineConstants.MaxFallerPhysicsFrames,
            EngineConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var currentFallDistanceStep = 0;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

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

        LemmingAction nextAction = IsFallFatal(
            in gadgetsNearLemming,
            lemming)
            ? SplatterAction.Instance
            : WalkerAction.Instance;
        lemming.SetNextAction(nextAction);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 2;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    [Pure]
    private static bool IsFallFatal(in GadgetEnumerable gadgetEnumerable, Lemming lemming)
    {
        if (lemming.State.HasSpecialFallingBehaviour)
            return false;

        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        foreach (var gadget in gadgetEnumerable)
        {
            var currentState = gadget.CurrentState;
            var hitBoxRegion = currentState.HitBoxRegion;

            if (!hitBoxRegion.ContainsPoint(anchorPixel) ||
                !hitBoxRegion.ContainsPoint(footPixel))
                continue;

            var filters = currentState.Filters;

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                if (!filter.MatchesLemming(lemming))
                    continue;

                if (filter.HitBoxHint == HitBoxBehaviour.NoSplat)
                    return false;
                if (filter.HitBoxHint == HitBoxBehaviour.Splat)
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
        if (lemming.State.IsFloater &&
            lemming.TrueDistanceFallen > 16 &&
            currentFallDistance == 0)
        {
            FloaterAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.State.IsGlider &&
            (lemming.TrueDistanceFallen > 8 ||
             (lemming.InitialFall &&
              lemming.TrueDistanceFallen > 6)))
        {
            GliderAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        var distanceFallen = GetStartingDistanceFallenFromAction(lemming);

        lemming.DistanceFallen = distanceFallen;
        lemming.TrueDistanceFallen = distanceFallen;

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    [Pure]
    private static int GetStartingDistanceFallenFromAction(Lemming lemming)
    {
        // For Swimmers it's handled by the SwimmerAction as there is no single universal value
        var currentActionId = lemming.CurrentAction.Id;

        return currentActionId switch
        {
            EngineConstants.WalkerActionId or EngineConstants.BasherActionId => 3,
            EngineConstants.MinerActionId or EngineConstants.DiggerActionId => 0,
            EngineConstants.BlockerActionId or EngineConstants.JumperActionId or EngineConstants.LasererActionId => -1,
            _ => 1
        };
    }
}