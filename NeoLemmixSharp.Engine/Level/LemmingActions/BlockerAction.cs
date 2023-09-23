using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BlockerAction : LemmingAction
{
    public static BlockerAction Instance { get; } = new();

    private BlockerAction()
    {
    }

    public override int Id => Global.BlockerActionId;
    public override string LemmingActionName => "blocker";
    public override int NumberOfAnimationFrames => Global.BlockerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => Global.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (!Global.TerrainManager.PixelIsSolidToLemming(lemming, lemming.LevelPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        Global.LemmingManager.RegisterBlocker(lemming);

        base.TransitionLemmingToAction(lemming, turnAround);
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -7;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 11;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;

    public static bool BlockerMatches(
        Lemming blocker,
        Lemming lemming,
        LevelPosition anchorPosition,
        LevelPosition footPosition)
    {
        Debug.Assert(blocker.CurrentAction == Instance);

        return false;
    }
}