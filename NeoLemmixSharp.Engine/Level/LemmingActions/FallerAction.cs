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
            LevelConstants.FallerAnimationFrames,
            LevelConstants.MaxFallerPhysicsFrames,
            LevelConstants.NonWalkerMovementPriority,
            false,
            true)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var currentFallDistanceStep = 0;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var updraftFallDelta = GetUpdraftFallDelta(lemming);
        var maxFallDistanceStep = LevelConstants.DefaultFallStep + updraftFallDelta.Y;

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        ref var distanceFallen = ref lemming.DistanceFallen;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
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

        LemmingAction nextAction = IsFallFatal(lemming)
            ? SplatterAction.Instance
            : WalkerAction.Instance;
        lemming.SetNextAction(nextAction);

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -4;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 2;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 0;

    private static bool IsFallFatal(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;

        if (lemming.State.IsFloater || lemming.State.IsGlider)
            return false;

        if (gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(lemming, NoSplatGadgetBehaviour.Instance))
            return false;

        return lemming.DistanceFallen > LevelConstants.MaxFallDistance ||
               gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(lemming, SplatGadgetBehaviour.Instance);
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

    public static LevelPosition GetUpdraftFallDelta(Lemming lemming)
    {
        var gadgetsNearPosition = LevelScreen.GadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

        if (gadgetsNearPosition.Count == 0)
            return new LevelPosition();

        var lemmingOrientation = lemming.Orientation;

        var hasUpdraft = false;
        var hasDowndraft = false;
        var draftLeft = false;
        var draftRight = false;

        foreach (var gadget in gadgetsNearPosition)
        {
            if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;

            var gadgetOrientation = gadget.Orientation;

            if (gadgetOrientation == lemmingOrientation)
            {
                hasDowndraft = true;
            }

            if (gadgetOrientation == Orientation.RotateClockwise(lemmingOrientation))
            {
                draftLeft = true;
            }

            if (gadgetOrientation == Orientation.GetOpposite(lemmingOrientation))
            {
                hasUpdraft = true;
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
        if (hasUpdraft)
        {
            dy--;
        }

        if (hasDowndraft)
        {
            dy++;
        }

        return new LevelPosition(dx, dy);
    }
}