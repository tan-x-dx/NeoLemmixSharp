using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.LevelRegion;

public sealed class PointSetLevelRegion : ILevelRegion
{
    private const int DimensionCutoffSize = 128;
    private const int AreaCutoffSize = 128 * 128;

    private readonly RectangularLevelRegion _anchor;
    private readonly BitArray _levelPositions;
    private readonly int _offsetX;
    private readonly int _offsetY;
    private readonly int _minimumBoundingBoxWidth;
    private readonly int _minimumBoundingBoxHeight;

    public PointSetLevelRegion(RectangularLevelRegion anchor, ICollection<LevelPosition> points)
    {
        if (points.Count == 0)
            throw new ArgumentException("Cannot create PointSetLevelRegion with zero points!");

        _anchor = anchor;

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = 0;
        var maxY = 0;

        foreach (var levelPosition in points)
        {
            minX = Math.Min(minX, levelPosition.X);
            minY = Math.Min(minY, levelPosition.Y);

            maxX = Math.Max(maxX, levelPosition.X);
            maxY = Math.Max(maxY, levelPosition.Y);
        }

        _minimumBoundingBoxWidth = 1 + maxX - minX;
        _minimumBoundingBoxHeight = 1 + maxY - minY;

        if (_minimumBoundingBoxWidth > DimensionCutoffSize || _minimumBoundingBoxHeight > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_minimumBoundingBoxWidth}, H:{_minimumBoundingBoxHeight}");

        var totalNumberOfPoints = _minimumBoundingBoxWidth * _minimumBoundingBoxHeight;

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositions = new BitArray(new UintArrayWrapper(totalNumberOfPoints));

        foreach (var levelPosition in points)
        {
            var index = _minimumBoundingBoxWidth * (levelPosition.Y - minY) + (levelPosition.X - minX);
            _levelPositions.SetBit(index);
        }

        _offsetX = minX;
        _offsetY = minY;
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        levelPosition -= _anchor.TopLeft;

        var newX = levelPosition.X - _offsetX;
        var newY = levelPosition.Y - _offsetY;
        var index = _minimumBoundingBoxWidth * newY + newX;

        return newX >= 0 &&
               newY >= 0 &&
               newX < _minimumBoundingBoxWidth &&
               newY < _minimumBoundingBoxHeight &&
               _levelPositions.GetBit(index);
    }
}
