using NeoLemmixSharp.Rendering;
using System;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FloaterAction : ILemmingAction
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

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "floater";
    public int NumberOfAnimationFrames => NumberOfFloaterAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is FloaterAction;
    public override bool Equals(object? obj) => obj is FloaterAction;
    public override int GetHashCode() => nameof(FloaterAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
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

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }

}