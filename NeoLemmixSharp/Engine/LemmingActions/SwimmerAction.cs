using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SwimmerAction : ILemmingAction
{
    public const int NumberOfSwimmerAnimationFrames = 8;

    public static SwimmerAction Instance { get; } = new();

    private SwimmerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "swimmer";
    public int NumberOfAnimationFrames => NumberOfSwimmerAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is SwimmerAction;
    public override bool Equals(object? obj) => obj is SwimmerAction;
    public override int GetHashCode() => nameof(SwimmerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}