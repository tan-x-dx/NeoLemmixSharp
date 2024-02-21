using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public static readonly StonerAction Instance = new();

    private StonerAction()
    {
    }

    public override int Id => LevelConstants.StonerActionId;
    public override string LemmingActionName => "stoner";
    public override int NumberOfAnimationFrames => LevelConstants.StonerAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        TerrainMasks.ApplyStonerMask(lemming);
        NoneAction.Instance.TransitionLemmingToAction(lemming, false);
        LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathExplode);
        lemming.ParticleTimer = LevelConstants.ParticleFrameCount;

        return false;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 5;
}