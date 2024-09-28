using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FallerAction : LemmingAction
{
    public static readonly FallerAction Instance = new();

    private FallerAction()
        : base(
            LevelConstants.FallerActionId,
            LevelConstants.FallerActionName,
            LevelConstants.FallerActionSpriteFileName,
            LevelConstants.FallerAnimationFrames,
            LevelConstants.MaxFallerPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var currentFallDistanceStep = 0;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var updraftFallDelta = GetUpdraftFallDelta(lemming);
        var maxFallDistanceStep = LevelConstants.DefaultFallStep + updraftFallDelta.Y;

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        var gadgetTestRegion = new LevelPositionPair(
            lemmingPosition,
            orientation.MoveDown(lemmingPosition, LevelConstants.DefaultFallStep + 1));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion, out var gadgetsNearRegion);

        ref var distanceFallen = ref lemming.DistanceFallen;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !PositionIsSolidToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            currentFallDistanceStep++;
            distanceFallen++;
            lemming.TrueDistanceFallen++;

            updraftFallDelta = GetUpdraftFallDelta(lemming);

            if (updraftFallDelta.Y < 0)
            {
                distanceFallen = 0;
            }
            else if (updraftFallDelta.Y > 0)
            {
                distanceFallen = Math.Min(distanceFallen, LevelConstants.MaxFallDistance / 2);
            }
        }

        distanceFallen = Math.Min(distanceFallen, LevelConstants.MaxFallDistance + 1);
        lemming.TrueDistanceFallen = Math.Min(lemming.TrueDistanceFallen, LevelConstants.MaxFallDistance + 1);

        if (currentFallDistanceStep >= maxFallDistanceStep)
            return true;

        LemmingAction nextAction = IsFallFatal(
            in gadgetsNearRegion,
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
    private static bool IsFallFatal(in GadgetSet gadgetSet, Lemming lemming)
    {
        if (lemming.State.HasSpecialFallingBehaviour)
            return false;

        var anchorPixel = lemming.LevelPosition;
        var footPixel = lemming.FootPosition;

        foreach (var gadget in gadgetSet)
        {
            if (!gadget.MatchesPosition(anchorPixel) &&
                !gadget.MatchesPosition(footPixel))
                continue;

            if (gadget.GadgetBehaviour == NoSplatGadgetBehaviour.Instance)
                return false;

            if (gadget.GadgetBehaviour == SplatGadgetBehaviour.Instance)
                return true;
        }

        return lemming.DistanceFallen > LevelConstants.MaxFallDistance;
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
            LevelConstants.WalkerActionId or LevelConstants.BasherActionId => 3,
            LevelConstants.MinerActionId or LevelConstants.DiggerActionId => 0,
            LevelConstants.BlockerActionId or LevelConstants.JumperActionId or LevelConstants.LasererActionId => -1,
            _ => 1
        };
    }

    [Pure]
    [SkipLocalsInit]
    public static LevelPosition GetUpdraftFallDelta(Lemming lemming)
    {
        // Subroutine of other LevelAction methods.
        // Use a dummy scratch space span to prevent data from being overridden.
        // Prevents weird bugs!
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];
        LevelScreen.GadgetManager.GetAllGadgetsAtLemmingPosition(scratchSpace, lemming, out var gadgetsNearPosition);

        if (gadgetsNearPosition.Count == 0)
            return new LevelPosition();

        var lemmingOrientation = lemming.Orientation;

        Span<int> draftDirectionDeltas = stackalloc int[4];
        draftDirectionDeltas.Clear();

        foreach (var gadget in gadgetsNearPosition)
        {
            if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;

            var deltaRotNum = (gadget.Orientation.RotNum - lemmingOrientation.RotNum) & 3;

            draftDirectionDeltas[deltaRotNum] = 1;
        }

        var dx = draftDirectionDeltas[LevelConstants.RightOrientationRotNum] -
                 draftDirectionDeltas[LevelConstants.LeftOrientationRotNum];

        var dy = draftDirectionDeltas[LevelConstants.UpOrientationRotNum] -
                 draftDirectionDeltas[LevelConstants.DownOrientationRotNum];

        return new LevelPosition(dx, dy);
    }
}