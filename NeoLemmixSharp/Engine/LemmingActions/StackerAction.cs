using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class StackerAction : ILemmingAction
{
    public const int NumberOfStackerAnimationFrames = 8;

    public static StackerAction Instance { get; } = new();

    private StackerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "stacker";
    public int NumberOfAnimationFrames => NumberOfStackerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is StackerAction;
    public override bool Equals(object? obj) => obj is StackerAction;
    public override int GetHashCode() => nameof(StackerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
        lemming.NumberOfBricksLeft = 8;
    }
}