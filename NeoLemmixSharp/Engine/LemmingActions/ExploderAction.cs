using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ExploderAction : ILemmingAction
{
    public const int NumberOfExploderAnimationFrames = 1;

    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "bomber";
    public int NumberOfAnimationFrames => NumberOfExploderAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is ExploderAction;
    public override bool Equals(object? obj) => obj is ExploderAction;
    public override int GetHashCode() => nameof(ExploderAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        return false;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}