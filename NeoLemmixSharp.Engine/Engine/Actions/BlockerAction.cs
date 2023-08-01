namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class BlockerAction : LemmingAction
{
    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int Id => GameConstants.BlockerActionId;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => GameConstants.BlockerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!Terrain.PixelIsSolidToLemming(lemming.Orientation, lemming.LevelPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}