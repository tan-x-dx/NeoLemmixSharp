using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ClimberAction : ILemmingAction
{
    public const int NumberOfClimberAnimationFrames = 8;

    public static ClimberAction Instance { get; } = new();

    private ClimberAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "climber";
    public int NumberOfAnimationFrames => NumberOfClimberAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is ClimberAction;
    public override bool Equals(object? obj) => obj is ClimberAction;
    public override int GetHashCode() => nameof(ClimberAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }

}