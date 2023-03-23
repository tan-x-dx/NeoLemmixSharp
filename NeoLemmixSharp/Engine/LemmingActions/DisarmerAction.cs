using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DisarmerAction : ILemmingAction
{
    public const int NumberOfDisarmerAnimationFrames = 16;

    public static DisarmerAction Instance { get; } = new();

    private DisarmerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "disarmer";
    public int NumberOfAnimationFrames => NumberOfDisarmerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is DisarmerAction;
    public override bool Equals(object? obj) => obj is DisarmerAction;
    public override int GetHashCode() => nameof(DisarmerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
    }

    public void OnTransitionToAction(Lemming lemming)
    {
    }
}