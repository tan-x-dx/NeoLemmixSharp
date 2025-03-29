using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
    private readonly LevelPosition _anchorPoint;
    private readonly LevelSize _maskSize;
    private readonly Range[] _spanRanges;
    private readonly LevelPosition[] _maskPositions;
    private readonly IDestructionMask _destructionMask;

    public TerrainEraseMask(
        LevelPosition anchorPoint,
        LevelSize maskSize,
        Range[] spanRanges,
        LevelPosition[] maskPositions,
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
        LevelPosition position,
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

    private ReadOnlySpan<LevelPosition> GetMaskPositionsForFrame(int frame)
    {
        var span = new ReadOnlySpan<LevelPosition>(_maskPositions);
        var range = _spanRanges[frame];
        return span[range];
    }
}
