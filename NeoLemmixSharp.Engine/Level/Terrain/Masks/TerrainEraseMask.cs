using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
    private readonly Point _anchorPoint;
    private readonly Size _maskSize;
    private readonly Range[] _spanRanges;
    private readonly Point[] _maskPositions;
    private readonly IDestructionMask _destructionMask;

    public TerrainEraseMask(
        Point anchorPoint,
        Size maskSize,
        Range[] spanRanges,
        Point[] maskPositions,
        IDestructionMask destructionMask)
    {
        _anchorPoint = anchorPoint;
        _maskSize = maskSize;
        _spanRanges = spanRanges;
        _maskPositions = maskPositions;
        _destructionMask = destructionMask;
    }

    public void ApplyEraseMask(
        Orientation orientation,
        FacingDirection facingDirection,
        Point position,
        int frame)
    {
        var transformation = new DihedralTransformation(orientation, facingDirection);

        var offset = position - transformation.Transform(_anchorPoint, _maskSize);
        var terrainManager = LevelScreen.TerrainManager;
        var maskPositions = GetMaskPositionsForFrame(frame);

        for (var i = 0; i < maskPositions.Length; i++)
        {
            var pixel = maskPositions[i];
            pixel = transformation.Transform(pixel, _maskSize);

            terrainManager.ErasePixel(orientation, _destructionMask, facingDirection, LevelScreen.NormalisePosition(pixel + offset));
        }
    }

    private ReadOnlySpan<Point> GetMaskPositionsForFrame(int frame)
    {
        var span = new ReadOnlySpan<Point>(_maskPositions);
        var range = _spanRanges[frame];
        return span[range];
    }
}
