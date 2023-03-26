using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class StonerAction : ILemmingAction
{
    public const int NumberOfStonerAnimationFrames = 16;

    public static StonerAction Instance { get; } = new();

    private StonerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "stoner";
    public int NumberOfAnimationFrames => NumberOfStonerAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is StonerAction;
    public override bool Equals(object? obj) => obj is StonerAction;
    public override int GetHashCode() => nameof(StonerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}