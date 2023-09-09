using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

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
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!TerrainManager.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        LemmingManager.RegisterBlocker(lemming);

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
}