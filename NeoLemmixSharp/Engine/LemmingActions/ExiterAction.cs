using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ExiterAction : ILemmingAction
{
    public const int NumberOfExiterAnimationFrames = 8;

    public static ExiterAction Instance { get; } = new();

    private ExiterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "exiter";
    public int NumberOfAnimationFrames => NumberOfExiterAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ExiterAction;
    public override bool Equals(object? obj) => obj is ExiterAction;
    public override int GetHashCode() => nameof(ExiterAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}