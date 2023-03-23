using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DiggerAction : ILemmingAction
{
    public const int NumberOfDiggerAnimationFrames = 16;

    public static DiggerAction Instance { get; } = new();

    private DiggerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "digger";
    public int NumberOfAnimationFrames => NumberOfDiggerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is DiggerAction;
    public override bool Equals(object? obj) => obj is DiggerAction;
    public override int GetHashCode() => nameof(DiggerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}