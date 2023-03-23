using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ReacherAction : ILemmingAction
{
    public const int NumberOfReacherAnimationFrames = 8;

    public static ReacherAction Instance { get; } = new();

    private ReacherAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "reacher";
    public int NumberOfAnimationFrames => NumberOfReacherAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ReacherAction;
    public override bool Equals(object? obj) => obj is ReacherAction;
    public override int GetHashCode() => nameof(ReacherAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}