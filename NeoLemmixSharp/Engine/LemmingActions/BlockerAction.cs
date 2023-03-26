using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

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
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is BlockerAction;
    public override bool Equals(object? obj) => obj is BlockerAction;
    public override int GetHashCode() => nameof(BlockerAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        if (!Terrain.GetPixelData(lemming.LevelPosition).IsSolid)
        {
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
        }

        return true;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}