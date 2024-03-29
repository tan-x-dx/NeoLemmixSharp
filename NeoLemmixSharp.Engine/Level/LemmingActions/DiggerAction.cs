﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
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
    {
    }

    public override int Id => LevelConstants.DiggerActionId;
    public override string LemmingActionName => "digger";
    public override int NumberOfAnimationFrames => LevelConstants.DiggerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        ref var lemmingPosition = ref lemming.LevelPosition;

        if (lemming.IsStartingAction)
        {
            lemming.IsStartingAction = false;
            DigOneRow(lemming, orientation, facingDirection, orientation.MoveUp(lemmingPosition, 1));
            // The first digger cycle is one frame longer!
            // So we need to artificially cancel the very first frame advancement.
            lemming.PhysicsFrame--;
        }

        if (lemming.PhysicsFrame != 0 &&
            lemming.PhysicsFrame != 8)
            return true;

        var continueDigging = DigOneRow(lemming, orientation, facingDirection, lemmingPosition);

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);

        if (terrainManager.PixelIsIndestructibleToLemming(lemming, this, lemmingPosition))
        {
            if (terrainManager.PixelIsSteel(lemmingPosition))
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
        Lemming lemming,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition lemmingPosition)
    {
        var terrainManager = LevelScreen.TerrainManager;
        // The central pixel of the removed row lies at the lemming's position
        var result = false;

        // Two most extreme pixels
        var checkLevelPosition = orientation.Move(lemmingPosition, -4, 0);
        var pixelIsSolid = terrainManager.PixelIsSolidToLemming(lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        checkLevelPosition = orientation.Move(lemmingPosition, 4, 0);
        pixelIsSolid = terrainManager.PixelIsSolidToLemming(lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.Move(lemmingPosition, i, 0);
            pixelIsSolid = terrainManager.PixelIsSolidToLemming(lemming, checkLevelPosition);
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
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowOffset +
                                 Orientation.GetOpposite(orientation).RotNum;
        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}
