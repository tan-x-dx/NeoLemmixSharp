using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FallerAction : LemmingAction
{
    public static readonly FallerAction Instance = new();

    private FallerAction()
    {
    }

    public override int Id => LevelConstants.FallerActionId;
    public override string LemmingActionName => "faller";
    public override int NumberOfAnimationFrames => LevelConstants.FallerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        var terrainManager = LevelScreen.TerrainManager;
        var currentFallDistanceStep = 0;
        var maxFallDistanceStep = LevelConstants.DefaultFallStep;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetSet = gadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

        foreach (var gadget in gadgetSet)
        {
            if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                continue;

            if (gadget.Orientation == Orientation.GetOpposite(orientation))
            {
                maxFallDistanceStep = 2;
            }
        }

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !terrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

            currentFallDistanceStep++;
            lemming.DistanceFallen++;
            lemming.TrueDistanceFallen++;

            gadgetSet = gadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

            foreach (var gadget in gadgetSet)
            {
                if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                    continue;

                if (gadget.Orientation == Orientation.GetOpposite(orientation))
                {
                    lemming.DistanceFallen = 0;
                }
            }
        }

        lemming.DistanceFallen = Math.Min(lemming.DistanceFallen, LevelConstants.MaxFallDistance + 1);
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

        return !(lemming.State.IsFloater || lemming.State.IsGlider) &&
               gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(lemming, NoSplatGadgetBehaviour.Instance) &&
               (lemming.DistanceFallen > LevelConstants.MaxFallDistance ||
                gadgetManager.HasGadgetWithBehaviourAtLemmingPosition(lemming, SplatGadgetBehaviour.Instance));
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
}