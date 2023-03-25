using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class LasererAction : ILemmingAction
{
    public const int NumberOfLasererAnimationFrames = 12;

    public static LasererAction Instance { get; } = new();

    private LasererAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "laserer";
    public int NumberOfAnimationFrames => NumberOfLasererAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is LasererAction;
    public override bool Equals(object? obj) => obj is LasererAction;
    public override int GetHashCode() => nameof(LasererAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}