using System.Runtime.CompilerServices;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

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
            LevelConstants.NonWalkerMovementPriority,
            false,
            true)
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
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        ref var distanceFallen = ref lemming.DistanceFallen;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !PositionIsSolidToLemming(gadgetsNearRegion, lemming, lemmingPosition))
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

    private static bool IsFallFatal(in GadgetSet gadgetSet, Lemming lemming)
    {
        if (lemming.State.IsFloater || lemming.State.IsGlider)
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
        var distanceFallen = GetDistanceFallen(lemming);

        lemming.DistanceFallen = distanceFallen;
        lemming.TrueDistanceFallen = distanceFallen;

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    private static int GetDistanceFallen(Lemming lemming)
    {
        // For Swimmers it's handled by the SwimmerAction as there is no single universal value
        var currentAction = lemming.CurrentAction;

        if (currentAction == WalkerAction.Instance ||
            currentAction == BasherAction.Instance)
            return 3;

        if (currentAction == MinerAction.Instance ||
            currentAction == DiggerAction.Instance)
            return 0;

        if (currentAction == BlockerAction.Instance ||
            currentAction == JumperAction.Instance ||
            currentAction == LasererAction.Instance)
            return -1;

        return 1;
    }
    
    [SkipLocalsInit]
    public static LevelPosition GetUpdraftFallDelta(Lemming lemming)
    {
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];
        var gadgetsNearPosition = LevelScreen.GadgetManager.GetAllGadgetsAtLemmingPosition(scratchSpace, lemming);

        if (gadgetsNearPosition.Count == 0)
            return new LevelPosition();

        var lemmingOrientation = lemming.Orientation;

        var draftUp = false;
        var draftDown = false;
        var draftLeft = false;
        var draftRight = false;

        foreach (var gadget in gadgetsNearPosition)
        {
            if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;

            var gadgetOrientation = gadget.Orientation;

            if (gadgetOrientation == lemmingOrientation)
            {
                draftDown = true;
            }

            if (gadgetOrientation == Orientation.RotateClockwise(lemmingOrientation))
            {
                draftLeft = true;
            }

            if (gadgetOrientation == Orientation.GetOpposite(lemmingOrientation))
            {
                draftUp = true;
            }

            if (gadgetOrientation == Orientation.RotateCounterClockwise(lemmingOrientation))
            {
                draftRight = true;
            }
        }

        var dx = 0;
        if (draftLeft)
        {
            dx--;
        }

        if (draftRight)
        {
            dx++;
        }

        var dy = 0;
        if (draftUp)
        {
            dy--;
        }

        if (draftDown)
        {
            dy++;
        }

        return new LevelPosition(dx, dy);
    }
}