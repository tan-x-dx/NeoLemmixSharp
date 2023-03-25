using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class FloaterAction : ILemmingAction
{
    public const int NumberOfFloaterAnimationFrames = 17;

    public static FloaterAction Instance { get; } = new();

    private FloaterAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "floater";
    public int NumberOfAnimationFrames => NumberOfFloaterAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is FloaterAction;
    public override bool Equals(object? obj) => obj is FloaterAction;
    public override int GetHashCode() => nameof(FloaterAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }

}