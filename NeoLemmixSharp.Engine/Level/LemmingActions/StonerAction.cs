using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class StonerAction
{
    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        TerrainMasks.ApplyStonerMask(lemming);
        NoneAction.TransitionLemmingToAction(lemming, false);
        LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathStoner);
        lemming.ParticleTimer = EngineConstants.ParticleFrameCount;

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.StonerAction.DoMainTransitionActions(lemming, turnAround);
}
