using NeoLemmixSharp.Engine.Gadgets;

namespace NeoLemmixSharp.Engine.Actions;

public sealed class FallerAction : LemmingAction
{
    public const int NumberOfFallerAnimationFrames = 4;
    public const int MaxFallDistance = 62;

    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    public override int Id => 12;
    public override string LemmingActionName => "faller";
    public override int NumberOfAnimationFrames => NumberOfFallerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var currentFallDistanceStep = 0;
        var maxFallDistanceStep = 3; // A lemming falls 3 pixels each frame

        var lemmingPosition = lemming.LevelPosition;

        if (GadgetCollections.Updrafts.TryGetGadgetThatMatchesTypeAndOrientation(lemmingPosition, lemming.Orientation.GetOpposite(), out _))
        {
            maxFallDistanceStep = 2;
        }

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !Terrain.PixelIsSolidToLemming(lemmingPosition, lemming))
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemmingPosition = lemming.Orientation.MoveDown(lemmingPosition, 1);
            lemming.LevelPosition = lemmingPosition;

            currentFallDistanceStep++;
            lemming.DistanceFallen++;
            lemming.TrueDistanceFallen++;

            if (GadgetCollections.Updrafts.TryGetGadgetThatMatchesTypeAndOrientation(lemmingPosition, lemming.Orientation.GetOpposite(), out _))
            {
                lemming.DistanceFallen = 0;
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

        if (lemming.CurrentAction == WalkerAction.Instance ||
            lemming.CurrentAction == BasherAction.Instance)
        {
            lemming.DistanceFallen = 3;
        }
        else if (lemming.CurrentAction == MinerAction.Instance ||
                 lemming.CurrentAction == DiggerAction.Instance)
        {
            lemming.DistanceFallen = 0;
        }
        else if (lemming.CurrentAction == BlockerAction.Instance ||
                 lemming.CurrentAction == JumperAction.Instance ||
                 lemming.CurrentAction == LasererAction.Instance)
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