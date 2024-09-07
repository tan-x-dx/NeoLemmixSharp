using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
    private readonly IDestructionMask _destructionMask;
    private readonly LevelPosition _anchorPoint;
    private readonly LevelPosition[] _mask;

    public TerrainEraseMask(
        IDestructionMask destructionMask,
        LevelPosition anchorPoint,
        LevelPosition[] mask)
    {
        _destructionMask = destructionMask;
        _anchorPoint = anchorPoint;
        _mask = mask;
    }

    public void ApplyEraseMask(
        Orientation orientation,
        FacingDirection facingDirection,
        LevelPosition position)
    {
        var offset = position - _anchorPoint;
        var terrainManager = LevelScreen.TerrainManager;

        foreach (var pixel in _mask)
        {
            terrainManager.ErasePixel(orientation, _destructionMask, facingDirection, LevelScreen.NormalisePosition(pixel + offset));
        }
    }

    public void Foo(
        Orientation orientation,
        FacingDirection facingDirection,
        Span<PixelType> pixelSpan,
        int spanWidth,
        int spanHeight,
        LevelPosition position)
    {
        var terrainManager = LevelScreen.TerrainManager;
        terrainManager.PopulateSpanWithTerrainData(pixelSpan, spanWidth, spanHeight, position.X, position.Y);

        foreach (ref var pixelToErase in pixelSpan)
        {
            if (pixelToErase.CanBeDestroyed() &&
                _destructionMask.CanDestroyPixel(pixelToErase, orientation, facingDirection))
            {
                pixelToErase = PixelType.Empty;
            }
        }
    }
}