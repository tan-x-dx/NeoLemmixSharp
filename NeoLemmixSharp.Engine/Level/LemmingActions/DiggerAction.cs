using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DiggerAction : LemmingAction, IDestructionMask
{
    public static readonly DiggerAction Instance = new();

    private DiggerAction()
        : base(
            LemmingActionType.DiggerAction,
            LemmingActionConstants.DiggerActionName,
            LemmingActionConstants.DiggerActionSpriteFileName,
            LemmingActionConstants.DiggerAnimationFrames,
            LemmingActionConstants.MaxDiggerPhysicsFrames,
            CursorSelectionPriority.NonPermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        var dht = lemming.DihedralTransformation;
        ref var lemmingPosition = ref lemming.AnchorPosition;

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(
                in gadgetsNearLemming,
                lemming,
                dht,
                dht.Orientation.MoveUp(lemmingPosition, 1));
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.PhysicsFrame--;
        }

        if (lemming.PhysicsFrame != 0 &&
            lemming.PhysicsFrame != 8)
            return true;

        var continueDigging = DigOneRow(
            in gadgetsNearLemming,
            lemming,
            dht,
            lemmingPosition);

        lemmingPosition = dht.Orientation.MoveDown(lemmingPosition, 1);

        if (PositionIsIndestructibleToLemming(in gadgetsNearLemming, lemming, this, lemmingPosition))
        {
            if (PositionIsSteelToLemming(in gadgetsNearLemming, lemming, lemmingPosition))
            {
                //CueSoundEffect(SFX_HITS_STEEL, L.Position);
            }

            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (continueDigging)
            return true;

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);

        return true;
    }

    private bool DigOneRow(
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming,
        DihedralTransformation dht,
        Point lemmingPosition)
    {
        var terrainManager = LevelScreen.TerrainManager;

        // The central pixel of the removed row lies at the lemming's position

        // Two most extreme pixels
        var checkLevelPosition = dht.Orientation.MoveLeft(lemmingPosition, 4);
        var pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(dht, this, checkLevelPosition);
        }

        checkLevelPosition = dht.Orientation.MoveRight(lemmingPosition, 4);
        pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(dht, this, checkLevelPosition);
        }

        var result = false;
        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = dht.Orientation.MoveRight(lemmingPosition, i);
            pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
            if (pixelIsSolid)
            {
                terrainManager.ErasePixel(dht, this, checkLevelPosition);
                result = true;
            }
        }

        // Delete these pixels from the terrain layer
        // ?? if not IsSimulating then fRenderInterface.RemoveTerrain(PosX - 4, PosY, 9, 1); ??
        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround) => DoMainTransitionActions(lemming, turnAround);

    [Pure]
    public bool CanDestroyPixel(DihedralTransformation dht, PixelType pixelType)
    {
        var pixelTypeInt = (uint)pixelType;
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset +
                                 dht.Orientation.GetOpposite().RotNum;

        return ((pixelTypeInt >>> oppositeArrowShift) & 1U) == 0U;
    }
}
