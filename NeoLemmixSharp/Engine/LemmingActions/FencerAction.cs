using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FencerAction : ILemmingAction
{
    public const int NumberOfFencerAnimationFrames = 16;

    public static FencerAction Instance { get; } = new();

    private FencerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "fencer";
    public int NumberOfAnimationFrames => NumberOfFencerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is FencerAction;
    public override bool Equals(object? obj) => obj is FencerAction;
    public override int GetHashCode() => nameof(FencerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}