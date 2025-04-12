using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public static readonly StonerAction Instance = new();

    private StonerAction()
        : base(
            EngineConstants.StonerActionId,
            EngineConstants.StonerActionName,
            EngineConstants.StonerActionSpriteFileName,
            EngineConstants.StonerAnimationFrames,
            EngineConstants.MaxStonerPhysicsFrames,
            EngineConstants.NoPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        TerrainMasks.ApplyStonerMask(lemming);
        NoneAction.Instance.TransitionLemmingToAction(lemming, false);
        LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathExplode);
        lemming.ParticleTimer = EngineConstants.ParticleFrameCount;

        return false;
    }
}