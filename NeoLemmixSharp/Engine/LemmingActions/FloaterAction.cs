using System;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FloaterAction : LemmingAction
{
    public const int NumberOfFloaterAnimationFrames = 17;

    public static FloaterAction Instance { get; } = new();

    private readonly int[] _floaterFallTable =
    {
        3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2
    };

    private FloaterAction()
    {
    }

    public override string LemmingActionName => "floater";
    public override int NumberOfAnimationFrames => NumberOfFloaterAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = _floaterFallTable[lemming.AnimationFrame - 1];

        if (false)// if HasTriggerAt(L.LemX, L.LemY, trUpdraft)
        {
            maxFallDistance--;
        }

        var groundPixelDistance = Math.Max(CommonMethods.FindGroundPixel(lemming.Orientation, lemming.LevelPosition), 0);
        if (maxFallDistance > groundPixelDistance)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, groundPixelDistance);
            lemming.NextAction = WalkerAction.Instance;
        }
        else
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, maxFallDistance);
        }

        return true;
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}