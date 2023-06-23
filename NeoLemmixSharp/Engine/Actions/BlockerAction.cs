namespace NeoLemmixSharp.Engine.Actions;

public sealed class BlockerAction : LemmingAction
{
    public const int NumberOfBlockerAnimationFrames = 16;

    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int ActionId => 3;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => NumberOfBlockerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!Terrain.PixelIsSolidToLemming(lemming.LevelPosition, lemming))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }
}