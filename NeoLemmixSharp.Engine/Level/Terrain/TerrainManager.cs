using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainManager
{
    private readonly ArrayWrapper2D<PixelType> _pixels;

    public TerrainManager(ArrayWrapper2D<PixelType> pixels)
    {
        _pixels = pixels;
    }

    [Pure]
    public PixelType PixelTypeAtPosition(LevelPosition levelPosition)
    {
        if (_pixels.EncompasesPoint(levelPosition))
            return _pixels[levelPosition];

        return PixelType.Void;
    }

    [Pure]
    public bool PixelIsSolidToLemming(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return PixelTypeAtPosition(levelPosition).IsSolidToOrientation(lemming.Orientation);
    }

    [Pure]
    public bool PixelIsIndestructibleToLemming(
        Lemming lemming,
        IDestructionMask destructionMask,
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return !pixel.CanBeDestroyed() ||
               !destructionMask.CanDestroyPixel(pixel, lemming.Orientation, lemming.FacingDirection);
    }

    [Pure]
    public bool PixelIsSteel(LevelPosition levelPosition)
    {
        return PixelTypeAtPosition(levelPosition).IsSteel();
    }

    public void ErasePixel(
        Orientation orientation,
        IDestructionMask destructionMask,
        FacingDirection facingDirection,
        LevelPosition pixelToErase)
    {
        if (!_pixels.EncompasesPoint(pixelToErase))
            return;

        ref var pixel = ref _pixels[pixelToErase];

        if (!pixel.CanBeDestroyed() ||
            !destructionMask.CanDestroyPixel(pixel, orientation, facingDirection))
            return;

        var previousValue = pixel;
        pixel &= PixelType.TerrainDataInverseMask;
        if (pixel == previousValue)
            return;

        LevelScreen.TerrainPainter.RecordPixelChange(
            pixelToErase,
            Color.Transparent,
            previousValue & PixelType.TerrainDataMask,
            0);
    }

    public void SetSolidPixel(LevelPosition pixelToSet, Color color)
    {
        if (!_pixels.EncompasesPoint(pixelToSet))
            return;

        ref var pixel = ref _pixels[pixelToSet];

        if (pixel != PixelType.Empty)
            return;

        var previousValue = pixel;
        pixel |= PixelType.SolidToAllOrientations;
        if (pixel == previousValue)
            return;

        LevelScreen.TerrainPainter.RecordPixelChange(
            pixelToSet,
            color,
            previousValue & PixelType.TerrainDataMask,
            PixelType.SolidToAllOrientations);
    }

    public void SetBlockerMaskPixel(LevelPosition pixelToSet, PixelType pixelTypeMask, bool set)
    {
        if (!_pixels.EncompasesPoint(pixelToSet))
            return;

        ref var pixel = ref _pixels[pixelToSet];

        if (set)
        {
            pixel |= pixelTypeMask;
        }
        else
        {
            pixel &= pixelTypeMask;
        }
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