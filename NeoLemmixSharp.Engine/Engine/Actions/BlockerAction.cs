namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class BlockerAction : LemmingAction
{
    public const int NumberOfBlockerAnimationFrames = 16;

    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int Id => 3;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => NumberOfBlockerAnimationFrames;
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