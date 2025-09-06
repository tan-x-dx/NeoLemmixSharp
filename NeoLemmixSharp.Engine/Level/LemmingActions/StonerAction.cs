using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StonerAction : LemmingAction
{
    public static readonly StonerAction Instance = new();

    private StonerAction()
        : base(
            LemmingActionConstants.StonerActionId,
            LemmingActionConstants.StonerActionName,
            LemmingActionConstants.StonerActionSpriteFileName,
            LemmingActionConstants.StonerAnimationFrames,
            LemmingActionConstants.MaxStonerPhysicsFrames,
            LemmingActionConstants.NoPriority,
            LemmingActionBounds.StandardLemmingBounds)
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

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);
}
