using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class PlatformerAction : ILemmingAction
{
    public const int NumberOfPlatformerAnimationFrames = 16;

    public static PlatformerAction Instance { get; } = new();

    private PlatformerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "platformer";
    public int NumberOfAnimationFrames => NumberOfPlatformerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is PlatformerAction;
    public override bool Equals(object? obj) => obj is PlatformerAction;
    public override int GetHashCode() => nameof(PlatformerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
        lemming.NumberOfBricksLeft = LemmingConstants.StepsMax;
        lemming.ConstructivePositionFreeze = false;
    }
}