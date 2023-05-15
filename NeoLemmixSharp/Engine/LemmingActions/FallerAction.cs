namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FallerAction : LemmingAction
{
    public const int NumberOfFallerAnimationFrames = 4;
    public const int MaxFallDistance = 62;

    public static FallerAction Instance { get; } = new();

    private FallerAction()
    {
    }

    protected override int ActionId => 12;
    public override string LemmingActionName => "faller";
    public override int NumberOfAnimationFrames => NumberOfFallerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var currentFallDistanceStep = 0;
        var maxFallDistanceStep = 3; // A lemming falls 3 pixels each frame

        if (false)//if HasTriggerAt(L.LemX, L.LemY, trUpdraft)
        {
            maxFallDistanceStep = 2;
        }

        if (CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
            return true;

        while (currentFallDistanceStep < maxFallDistanceStep &&
               !Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            if (currentFallDistanceStep > 0 &&
                CheckFloaterOrGliderTransition(lemming, currentFallDistanceStep))
                return true;

            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
            currentFallDistanceStep++;
            lemming.DistanceFallen++;
            lemming.TrueDistanceFallen++;

            if (false)//if HasTriggerAt(L.LemX, L.LemY, trUpdraft)
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
            lemming.NextAction = IsFallFatal(lemming)
                ? SplatterAction.Instance
                : WalkerAction.Instance;
        }

        return true;
    }

    private static bool IsFallFatal(Lemming lemming)
    {
        return (!(lemming.IsFloater || lemming.IsGlider)) &&
               (true) && //not HasTriggerAt(L.LemX, L.LemY, trNoSplat)
               ((lemming.DistanceFallen > MaxFallDistance) ||
                false); // HasTriggerAt(L.LemX, L.LemY, trSplat);
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