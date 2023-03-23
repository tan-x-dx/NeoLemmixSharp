using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class BlockerAction : ILemmingAction
{
    public const int NumberOfBlockerAnimationFrames = 16;

    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "blocker";
    public int NumberOfAnimationFrames => NumberOfBlockerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is BlockerAction;
    public override bool Equals(object? obj) => obj is BlockerAction;
    public override int GetHashCode() => nameof(BlockerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}