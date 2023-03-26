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
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is DrownerAction;
    public override bool Equals(object? obj) => obj is DrownerAction;
    public override int GetHashCode() => nameof(DrownerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // remove lemming
        }

        return false;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}