using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public sealed class PointSetLevelRegion : ILevelRegion
{
    private const int DimensionCutoffSize = 128;
    private const int AreaCutoffSize = 128 * 128;

    private readonly BitArray _levelPositions;
    private readonly LevelPosition _offset;

    private readonly int _minimumBoundingBoxWidth;
    private readonly int _minimumBoundingBoxHeight;

    public PointSetLevelRegion(ReadOnlySpan<LevelPosition> points)
    {
        if (points.Length == 0)
            throw new ArgumentException("Cannot create PointSetLevelRegion with zero points!");

        var minimumBoundingBox = new LevelPositionPair(points);

        _minimumBoundingBoxWidth = 1 + minimumBoundingBox.P2X - minimumBoundingBox.P1X;
        _minimumBoundingBoxHeight = 1 + minimumBoundingBox.P2Y - minimumBoundingBox.P1Y;

        if (_minimumBoundingBoxWidth > DimensionCutoffSize || _minimumBoundingBoxHeight > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_minimumBoundingBoxWidth}, H:{_minimumBoundingBoxHeight}");

        var totalNumberOfPoints = _minimumBoundingBoxWidth * _minimumBoundingBoxHeight;

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositions = new BitArray(totalNumberOfPoints);

        foreach (var levelPosition in points)
        {
            var x = levelPosition.X - minimumBoundingBox.P1X;
            var y = levelPosition.Y - minimumBoundingBox.P1Y;

            var index = IndexFor(x, y);
            _levelPositions.SetBit(index);
        }

        _offset = minimumBoundingBox.GetTopLeftPosition();
    }

    [Pure]
    public bool ContainsPoint(LevelPosition levelPosition)
    {
        levelPosition -= _offset;
        var index = IndexFor(levelPosition.X, levelPosition.Y);

        return levelPosition.X >= 0 &&
               levelPosition.Y >= 0 &&
               levelPosition.X < _minimumBoundingBoxWidth &&
               levelPosition.Y < _minimumBoundingBoxHeight &&
               _levelPositions.GetBit(index);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexFor(int x, int y) => _minimumBoundingBoxWidth * y + x;
}
