﻿using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainManager
{
    private readonly PixelType[] _pixels;
    private readonly GadgetManager _gadgetManager;

    private readonly TerrainPainter _terrainPainter;

    public IHorizontalBoundaryBehaviour HorizontalBoundaryBehaviour { get; }
    public IVerticalBoundaryBehaviour VerticalBoundaryBehaviour { get; }

    public int LevelWidth => HorizontalBoundaryBehaviour.LevelWidth;
    public int LevelHeight => VerticalBoundaryBehaviour.LevelHeight;

    public TerrainManager(
        PixelType[] pixels,
        GadgetManager gadgetManager,
        TerrainPainter terrainPainter,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _pixels = pixels;
        _gadgetManager = gadgetManager;

        _terrainPainter = terrainPainter;

        HorizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        VerticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            HorizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            VerticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= LevelWidth ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= LevelHeight;
    }

    [Pure]
    public PixelType PixelTypeAtPosition(LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return PixelType.Void;

        var index = LevelWidth * levelPosition.Y + levelPosition.X;
        return _pixels[index];
    }

    [Pure]
    public bool PixelIsSolidToLemming(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return pixel.IsSolidToOrientation(lemming.Orientation) ||
               _gadgetManager.HasGadgetWithBehaviourAtPosition(levelPosition, MetalGrateGadgetBehaviour.Instance);
    }

    [Pure]
    public bool PixelIsIndestructibleToLemming(
        Lemming lemming,
        IDestructionMask destructionMask,
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return !pixel.CanBeDestroyed() ||
               !destructionMask.CanDestroyPixel(pixel, lemming.Orientation, lemming.FacingDirection) ||
               _gadgetManager.HasGadgetWithBehaviourAtPosition(levelPosition, MetalGrateGadgetBehaviour.Instance);
    }

    [Pure]
    public bool PixelIsSteel(
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return pixel.IsSteel() ||
               _gadgetManager.HasGadgetWithBehaviourAtPosition(levelPosition, MetalGrateGadgetBehaviour.Instance);
    }

    public void ErasePixel(
        Orientation orientation,
        IDestructionMask destructionMask,
        FacingDirection facingDirection,
        LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = LevelWidth * pixelToErase.Y + pixelToErase.X;
        ref var pixel = ref _pixels[index];

        if (!pixel.CanBeDestroyed() ||
            !destructionMask.CanDestroyPixel(pixel, orientation, facingDirection))
            return;

        var previousValue = pixel;
        pixel = PixelType.Empty;
        if (pixel == previousValue)
            return;

        _terrainPainter.RecordPixelChange(
            pixelToErase,
            Color.Transparent,
            previousValue,
            pixel);
        LevelScreen.PixelChangeCount++;
    }

    public void SetSolidPixel(LevelPosition pixelToSet, Color color)
    {
        if (PositionOutOfBounds(pixelToSet) ||
            _gadgetManager.HasGadgetWithBehaviourAtPosition(pixelToSet, MetalGrateGadgetBehaviour.Instance))
            return;

        var index = LevelWidth * pixelToSet.Y + pixelToSet.X;
        ref var pixel = ref _pixels[index];

        if (pixel != PixelType.Empty)
            return;

        var previousValue = pixel;
        pixel |= PixelType.SolidToAllOrientations;
        if (pixel == previousValue)
            return;

        //_terrainRenderer.SetPixelColor(pixelToSet.X, pixelToSet.Y, color);
        _terrainPainter.RecordPixelChange(pixelToSet, color, previousValue, pixel);
        LevelScreen.PixelChangeCount++;
    }

    public void SetBlockerMaskPixel(LevelPosition pixelToSet, PixelType pixelTypeMask, bool set)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = LevelWidth * pixelToSet.Y + pixelToSet.X;
        ref var pixel = ref _pixels[index];

        if (set)
        {
            pixel |= pixelTypeMask;
        }
        else
        {
            pixel &= pixelTypeMask;
        }
    }

    [Pure]
    public PixelType GetBlockerData(LevelPosition pixel)
    {
        if (PositionOutOfBounds(pixel))
            return PixelType.Empty;

        var index = LevelWidth * pixel.Y + pixel.X;
        var result = _pixels[index];

        return result & PixelType.BlockerMask;
    }

    public void PopulateSpanWithTerrainData(
        Span<PixelType> pixelSpan,
        int spanWidth,
        int spanHeight,
        int xOffset,
        int yOffset)
    {
        for (var y0 = spanHeight - 1; y0 >= 0; y0--)
        {
            var y1 = y0 + yOffset;
            var y2 = y0 * spanWidth;
            for (var x0 = spanWidth - 1; x0 >= 0; x0--)
            {
                pixelSpan[y2 + x0] = PixelTypeAtPosition(new LevelPosition(x0 + xOffset, y1));
            }
        }
    }
}