using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DehoisterAction : ILemmingAction
{
    public const int NumberOfDehoisterAnimationFrames = 7;

    public static DehoisterAction Instance { get; } = new();

    private DehoisterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "dehoister";
    public int NumberOfAnimationFrames => NumberOfDehoisterAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is DehoisterAction;
    public override bool Equals(object? obj) => obj is DehoisterAction;
    public override int GetHashCode() => nameof(DehoisterAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}