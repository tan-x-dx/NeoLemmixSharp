using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SplatterAction : ILemmingAction
{
    public const int NumberOfSplatterAnimationFrames = 16;

    public static SplatterAction Instance { get; } = new();

    private SplatterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "splatter";
    public int NumberOfAnimationFrames => NumberOfSplatterAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is SplatterAction;
    public override bool Equals(object? obj) => obj is SplatterAction;
    public override int GetHashCode() => nameof(SplatterAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}