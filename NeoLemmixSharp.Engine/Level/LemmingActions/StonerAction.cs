using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public static readonly StonerAction Instance = new();

    private StonerAction()
        : base(
            LevelConstants.StonerActionId,
            LevelConstants.StonerActionName,
            LevelConstants.StonerAnimationFrames,
            LevelConstants.MaxStonerPhysicsFrames,
            LevelConstants.NoPriority,
            true,
            false)
    {
    }

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