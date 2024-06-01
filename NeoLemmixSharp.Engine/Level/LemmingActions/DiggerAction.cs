using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DiggerAction : LemmingAction, IDestructionMask
{
    public static readonly DiggerAction Instance = new();

    private DiggerAction()
        : base(
            LevelConstants.DiggerActionId,
            LevelConstants.DiggerActionName,
            LevelConstants.DiggerAnimationFrames,
            LevelConstants.MaxDiggerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority,
            false,
            false)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetTestRegion = new LevelPositionPair(
            orientation.Move(lemmingPosition, 4, 1),
            orientation.Move(lemmingPosition, -4, -1));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(gadgetTestRegion);

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(
                in gadgetsNearRegion,
                lemming,
                orientation,
                facingDirection,
                orientation.MoveUp(lemmingPosition, 1));
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.PhysicsFrame--;
        }

        if (lemming.PhysicsFrame != 0 &&
            lemming.PhysicsFrame != 8)
            return true;

        var continueDigging = DigOneRow(
            in gadgetsNearRegion,
            lemming,
            orientation,
            facingDirection,
            lemmingPosition);

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        if (PositionIsIndestructibleToLemming(gadgetsNearRegion, lemming, this, lemmingPosition))
        {
            if (PositionIsSteelToLemming(gadgetsNearRegion, lemming, lemmingPosition))
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

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -5;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 6;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;

    private bool DigOneRow(
        in GadgetSet gadgetsNearRegion,
        Lemming lemming,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition lemmingPosition)
    {
        var terrainManager = LevelScreen.TerrainManager;
        // The central pixel of the removed row lies at the lemming's position
        var result = false;

        // Two most extreme pixels
        var checkLevelPosition = orientation.MoveLeft(lemmingPosition, 4);
        var pixelIsSolid = PositionIsSolidToLemming(gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        checkLevelPosition = orientation.MoveRight(lemmingPosition, 4);
        pixelIsSolid = PositionIsSolidToLemming(gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.MoveRight(lemmingPosition, i);
            pixelIsSolid = PositionIsSolidToLemming(gadgetsNearRegion, lemming, checkLevelPosition);
            if (pixelIsSolid)
            {
                terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
                result = true;
            }
        }

        // Delete these pixels from the terrain layer
        // ?? if not IsSimulating then fRenderInterface.RemoveTerrain(PosX - 4, PosY, 9, 1); ??
        return result;
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset +
                                 ((2 + orientation.RotNum) & 3);
        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}
