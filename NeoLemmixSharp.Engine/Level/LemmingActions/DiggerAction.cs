﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class DiggerAction : LemmingAction, IDestructionMask
{
    public static readonly DiggerAction Instance = new();

    private DiggerAction()
        : base(
            LevelConstants.DiggerActionId,
            LevelConstants.DiggerActionName,
            LevelConstants.DiggerActionSpriteFileName,
            LevelConstants.DiggerAnimationFrames,
            LevelConstants.MaxDiggerPhysicsFrames,
            LevelConstants.NonPermanentSkillPriority)
    {
    }

    [SkipLocalsInit]
    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var facingDirection = lemming.FacingDirection;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        var gadgetTestRegion = new LevelRegion(
            orientation.Move(lemmingPosition, 4, 1),
            orientation.Move(lemmingPosition, -4, -1));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

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

        if (PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, this, lemmingPosition))
        {
            if (PositionIsSteelToLemming(in gadgetsNearRegion, lemming, lemmingPosition))
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
        in GadgetEnumerable gadgetsNearRegion,
        Lemming lemming,
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition lemmingPosition)
    {
        var terrainManager = LevelScreen.TerrainManager;

        // The central pixel of the removed row lies at the lemming's position

        // Two most extreme pixels
        var checkLevelPosition = orientation.MoveLeft(lemmingPosition, 4);
        var pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        checkLevelPosition = orientation.MoveRight(lemmingPosition, 4);
        pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
        if (pixelIsSolid)
        {
            terrainManager.ErasePixel(orientation, this, facingDirection, checkLevelPosition);
        }

        var result = false;
        // Everything in between
        for (var i = -3; i < 4; i++)
        {
            checkLevelPosition = orientation.MoveRight(lemmingPosition, i);
            pixelIsSolid = PositionIsSolidToLemming(in gadgetsNearRegion, lemming, checkLevelPosition);
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

    string IDestructionMask.Name => LemmingActionName;

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        var oppositeArrowShift = PixelTypeHelpers.PixelTypeArrowShiftOffset +
                                 ((2 + orientation.RotNum) & 3);
        var oppositeArrowMask = (PixelType)(1 << oppositeArrowShift);
        return (pixelType & oppositeArrowMask) == PixelType.Empty;
    }
}
