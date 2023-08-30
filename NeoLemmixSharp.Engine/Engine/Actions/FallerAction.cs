using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class FallerAction : LemmingAction
{
    private const int MaxFallDistance = 62;

    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    public override int Id => GameConstants.FallerActionId;
    public override string LemmingActionName => "faller";
    public override int NumberOfAnimationFrames => GameConstants.FallerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var currentFallDistanceStep = 0;
        var maxFallDistanceStep = 3; // A lemming falls 3 pixels each frame

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var allGadgets = Gadgets.AllGadgets;
        var idEnumerator = Gadgets.GetAllGadgetIdsForPosition(lemmingPosition);

        while (idEnumerator.MoveNext())
        {
            var gadgetId = idEnumerator.Current;
            var gadget = allGadgets[gadgetId];

            if (gadget.Type != GadgetType.Updraft)
                continue;
            if (!gadget.MatchesLemming(lemming))
                continue;

            if (gadget.Orientation == orientation.GetOpposite())
            {
                maxFallDistanceStep = 2;
            }
        }

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !Terrain.PixelIsSolidToLemming(lemming, lemmingPosition))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;

            currentFallDistanceStep++;
            lemming.DistanceFallen++;
            lemming.TrueDistanceFallen++;

            idEnumerator = Gadgets.GetAllGadgetIdsForPosition(lemmingPosition);

            while (idEnumerator.MoveNext())
            {
                var gadgetId = idEnumerator.Current;
                var gadget = allGadgets[gadgetId];

                if (gadget.Type != GadgetType.Updraft)
                    continue;
                if (!gadget.MatchesLemming(lemming))
                    continue;

                if (gadget.Orientation == orientation.GetOpposite())
                {
                    lemming.DistanceFallen = 0;
                }
            }
        }

        lemming.DistanceFallen = Math.Min(lemming.DistanceFallen, MaxFallDistance + 1);
        lemming.TrueDistanceFallen = Math.Min(lemming.TrueDistanceFallen, MaxFallDistance + 1);

        if (currentFallDistanceStep >= maxFallDistanceStep)
            return true;

        LemmingAction nextAction = IsFallFatal(lemming)
            ? SplatterAction.Instance
            : WalkerAction.Instance;
        lemming.SetNextAction(nextAction);

        return true;
    }

    private static bool IsFallFatal(Lemming lemming)
    {
        return !(lemming.State.IsFloater || lemming.State.IsGlider) &&
               Gadgets.HasGadgetOfTypeAtPosition(lemming.LevelPosition, GadgetType.NoSplat) &&
               (lemming.DistanceFallen > MaxFallDistance ||
                Gadgets.HasGadgetOfTypeAtPosition(lemming.LevelPosition, GadgetType.Splat));
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
        // For Swimmers it's handled by the SwimmerAction as there is no single universal value
        var currentAction = lemming.CurrentAction;

        if (currentAction == WalkerAction.Instance ||
            currentAction == BasherAction.Instance)
        {
            lemming.DistanceFallen = 3;
        }
        else if (currentAction == MinerAction.Instance ||
                 currentAction == DiggerAction.Instance)
        {
            lemming.DistanceFallen = 0;
        }
        else if (currentAction == BlockerAction.Instance ||
                 currentAction == JumperAction.Instance ||
                 currentAction == LasererAction.Instance)
        {
            lemming.DistanceFallen = -1;
        }
        else
        {
            lemming.DistanceFallen = 1;
        }

        lemming.TrueDistanceFallen = lemming.DistanceFallen;

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}