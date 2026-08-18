using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class ExploderAction
{
    public static IDestructionMask DestructionMask { get; } = new ExploderActionDestructionMask();

    private sealed class ExploderActionDestructionMask : IDestructionMask
    {
        [Pure]
        public bool CanDestroyPixel(DihedralTransformation dht, PixelType pixelType)
        {
            // Bombers do not care about arrows, only if the pixel can be destroyed at all!
            // Since other checks will have already taken place, this code is only ever
            // reached when the pixel can definitely be destroyed by a bomber.
            // Therefore, just return true.

            return true;
        }
    }

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        TerrainMasks.ApplyBomberMask(lemming);
        LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathExploder);
        lemming.ParticleTimer = EngineConstants.ParticleFrameCount;

        WalkerAction.TransitionLemmingToAction(lemming, false);

        return false;
    }

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround) => LemmingActionType.ExploderAction.DoMainTransitionActions(lemming, turnAround);
}
