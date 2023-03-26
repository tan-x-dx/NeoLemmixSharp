using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class VaporiserAction : ILemmingAction
{
    public const int NumberOfVaporiserAnimationFrames = 16;

    public static VaporiserAction Instance { get; } = new();

    private VaporiserAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "burner";
    public int NumberOfAnimationFrames => NumberOfVaporiserAnimationFrames;
    public bool IsOneTimeAction => true;

    public bool Equals(ILemmingAction? other) => other is VaporiserAction;
    public override bool Equals(object? obj) => obj is VaporiserAction;
    public override int GetHashCode() => nameof(VaporiserAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}