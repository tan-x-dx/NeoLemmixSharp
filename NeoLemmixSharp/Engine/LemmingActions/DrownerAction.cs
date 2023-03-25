using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class DrownerAction : ILemmingAction
{
    public const int NumberOfDrownerAnimationFrames = 16;

    public static DrownerAction Instance { get; } = new();

    private DrownerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "drowner";
    public int NumberOfAnimationFrames => NumberOfDrownerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is DrownerAction;
    public override bool Equals(object? obj) => obj is DrownerAction;
    public override int GetHashCode() => nameof(DrownerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == NumberOfDrownerAnimationFrames)
        {
            // remove lemming
        }
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}