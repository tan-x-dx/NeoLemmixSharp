using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public sealed class TerrainEraseMask
{
    private readonly int _maskWidth;
    private readonly int _maskHeight;

    private readonly LevelPosition _anchorPoint;
    private readonly Range[] _spanRanges;
    private readonly LevelPosition[] _maskPositions;
    private readonly IDestructionMask _destructionMask;

    public TerrainEraseMask(
        int maskWidth,
        int maskHeight,
        LevelPosition anchorPoint,
        Range[] spanRanges,
        LevelPosition[] maskPositions,
        IDestructionMask destructionMask)
    {
        _maskWidth = maskWidth;
        _maskHeight = maskHeight;
        _anchorPoint = anchorPoint;
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
        var transformation = new DihedralTransformation(orientation.RotNum, facingDirection == FacingDirection.Left);

        var offset = position - transformation.Transform(_anchorPoint, _maskWidth, _maskHeight);
        var terrainManager = LevelScreen.TerrainManager;
        var maskPositions = GetMaskPositionsForFrame(frame);

        for (var i = 0; i < maskPositions.Length; i++)
        {
            var pixel = maskPositions[i];
            pixel = transformation.Transform(pixel, _maskWidth, _maskHeight);

            terrainManager.ErasePixel(orientation, _destructionMask, facingDirection, LevelScreen.NormalisePosition(pixel + offset));
        }
    }

    private ReadOnlySpan<LevelPosition> GetMaskPositionsForFrame(int frame)
    {
        var range = _spanRanges[frame];
        var span = new ReadOnlySpan<LevelPosition>(_maskPositions);
        return span[range];
    }
}
