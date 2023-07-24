using NeoLemmixSharp.Engine.Engine.Gadgets;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class FallerAction : LemmingAction
{
    public const int NumberOfFallerAnimationFrames = 4;
    public const int MaxFallDistance = 62;

    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    public override int Id => 17;
    public override string LemmingActionName => "faller";
    public override int NumberOfAnimationFrames => NumberOfFallerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var currentFallDistanceStep = 0;
        var maxFallDistanceStep = 3; // A lemming falls 3 pixels each frame

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        if (GadgetCollections.Updrafts.TryGetGadgetThatMatchesTypeAndOrientation(lemmingPosition, orientation.GetOpposite(), out var updraft))
        {
            maxFallDistanceStep = 2;
            updraft.OnLemmingInHitBox(lemming);
        }

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !Terrain.PixelIsSolidToLemming(orientation, lemmingPosition))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;

            currentFallDistanceStep++;
            lemming.DistanceFallen++;
            lemming.TrueDistanceFallen++;

            if (GadgetCollections.Updrafts.TryGetGadgetThatMatchesTypeAndOrientation(lemmingPosition, orientation.GetOpposite(), out updraft))
            {
                lemming.DistanceFallen = 0;
                updraft.OnLemmingInHitBox(lemming);
            }
        }

        if (lemming.DistanceFallen > MaxFallDistance)
        {
            lemming.DistanceFallen = MaxFallDistance + 1;
        }

        if (lemming.TrueDistanceFallen > MaxFallDistance)
        {
            lemming.TrueDistanceFallen = MaxFallDistance + 1;
        }

        if (currentFallDistanceStep < maxFallDistanceStep)
        {
            lemming.SetNextAction(IsFallFatal(lemming)
                ? SplatterAction.Instance
                : WalkerAction.Instance);
        }

        return true;
    }

    private static bool IsFallFatal(Lemming lemming)
    {
        return !(lemming.IsFloater || lemming.IsGlider) &&
               false/*!Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.NoSplat, lemming.LevelPosition, lemming.Orientation)*/ &&
               (lemming.DistanceFallen > MaxFallDistance ||
                false/*Terrain.HasGadgetThatMatchesTypeAndOrientation(GadgetType.Splat, lemming.LevelPosition, lemming.Orientation)*/);
    }

    private static bool CheckFloaterOrGliderTransition(
        Lemming lemming,
        int currentFallDistance)
    {
        if (lemming.IsFloater &&
            lemming.TrueDistanceFallen > 16 &&
            currentFallDistance == 0)
        {
            FloaterAction.Instance.TransitionLemmingToAction(lemming, false);
            return true;
        }

        if (lemming.IsGlider &&
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
        // for Swimming it's set in HandleSwimming as there is no single universal value
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