using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class JumperAction : ILemmingAction
{
    public const int NumberOfJumperAnimationFrames = 13;

    public static JumperAction Instance { get; } = new();

    private JumperAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "jumper";
    public int NumberOfAnimationFrames => NumberOfJumperAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is JumperAction;
    public override bool Equals(object? obj) => obj is JumperAction;
    public override int GetHashCode() => nameof(JumperAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}