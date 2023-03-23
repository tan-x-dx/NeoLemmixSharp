using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ShimmierAction : ILemmingAction
{
    public const int NumberOfShimmierAnimationFrames = 20;

    public static ShimmierAction Instance { get; } = new();

    private ShimmierAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "shimmier";
    public int NumberOfAnimationFrames => NumberOfShimmierAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ShimmierAction;
    public override bool Equals(object? obj) => obj is ShimmierAction;
    public override int GetHashCode() => nameof(ShimmierAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}