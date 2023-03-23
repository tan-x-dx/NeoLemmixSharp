using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class OhNoerAction : ILemmingAction
{
    public const int NumberOfOhNoerAnimationFrames = 16;

    public static OhNoerAction Instance { get; } = new();

    private OhNoerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "ohnoer";
    public int NumberOfAnimationFrames => NumberOfOhNoerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is OhNoerAction;
    public override bool Equals(object? obj) => obj is OhNoerAction;
    public override int GetHashCode() => nameof(OhNoerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}