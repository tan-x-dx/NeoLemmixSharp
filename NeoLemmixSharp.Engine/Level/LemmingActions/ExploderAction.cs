using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ExploderAction : LemmingAction, IDestructionMask
{
    public static readonly ExploderAction Instance = new();

    private ExploderAction()
        : base(
            LemmingActionConstants.ExploderActionId,
            LemmingActionConstants.ExploderActionName,
            LemmingActionConstants.ExploderActionSpriteFileName,
            LemmingActionConstants.ExploderAnimationFrames,
            LemmingActionConstants.MaxExploderPhysicsFrames,
            LemmingActionConstants.NoPriority,
            LemmingActionBounds.StandardLemmingBounds)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        TerrainMasks.ApplyBomberMask(lemming);
        LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathExplode);
        lemming.ParticleTimer = EngineConstants.ParticleFrameCount;

        WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

        return false;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        // Bombers do not care about arrows, only if the pixel can be destroyed at all!
        // Since other checks will have already taken place, this code is only ever
        // reached when the pixel can definitely be destroyed by a bomber.
        // Therefore, just return true.

        return true;
    }
}
