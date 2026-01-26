using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainManager
{
    private readonly ArrayWrapper2D<PixelType> _pixels;

    public TerrainManager(ArrayWrapper2D<PixelType> pixels)
    {
        _pixels = pixels;
    }

    public Size LevelDimensions => _pixels.Size;
    public PixelType[] RawPixels => _pixels.Array;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PositionOutOfBounds(Point levelPosition)
    {
        return !_pixels.Size.EncompassesPoint(levelPosition);
    }

    [Pure]
    public PixelType PixelTypeAtPosition(Point levelPosition)
    {
        var pixel = _pixels.TryGetRef(levelPosition, out var isValid);
        if (isValid)
            return pixel;

        return PixelType.Void;
    }

    [Pure]
    public bool PixelIsSolidToLemming(
        Lemming lemming,
        Point levelPosition)
    {
        return PixelTypeAtPosition(levelPosition).IsSolidToOrientation(lemming.Orientation);
    }

    [Pure]
    public bool PixelIsIndestructibleToLemming(
        Lemming lemming,
        IDestructionMask destructionMask,
        Point levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return !pixel.CanBeDestroyed() ||
               !destructionMask.CanDestroyPixel(pixel, lemming.Orientation, lemming.FacingDirection);
    }

    [Pure]
    public bool PixelIsSteel(Point levelPosition)
    {
        return PixelTypeAtPosition(levelPosition).IsSteel();
    }

    public void ErasePixel(
        Orientation orientation,
        IDestructionMask destructionMask,
        FacingDirection facingDirection,
        Point pixelToErase)
    {
        ref var pixel = ref _pixels.TryGetRef(pixelToErase, out var isValid);

        if (!isValid)
            return;

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

    public void SetSolidPixel(Point pixelToSet, Color color)
    {
        ref var pixel = ref _pixels.TryGetRef(pixelToSet, out var isValid);

        if (!isValid)
            return;

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
}
