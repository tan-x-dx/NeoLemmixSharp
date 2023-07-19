using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.LevelRegion;

public sealed class PointSetLevelRegion : ILevelRegion
{
    private const int DimensionCutoffSize = 128;
    private const int AreaCutoffSize = 128 * 128;

    private readonly IBitArray _levelPositions;
    private readonly int _offsetX;
    private readonly int _offsetY;
    private readonly int _width;
    private readonly int _height;

    public PointSetLevelRegion(ICollection<LevelPosition> points)
    {
        if (points.Count == 0)
            throw new ArgumentException("Cannot create PointSetLevelRegion with zero points!");

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

        _width = 1 + maxX - minX;
        _height = 1 + maxY - minY;

        if (_width > DimensionCutoffSize || _height > DimensionCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! W:{_width}, H:{_height}");

        var totalNumberOfPoints = _width * _height;

        if (totalNumberOfPoints > AreaCutoffSize)
            throw new ArgumentException($"The region enclosed by this set of points is far too large! Area:{totalNumberOfPoints}");

        _levelPositions = IBitArray.GetBestFitForSize(totalNumberOfPoints);

        foreach (var levelPosition in points)
        {
            var index = _width * (levelPosition.Y - minY) + (levelPosition.X - minX);
            _levelPositions.SetBit(index);
        }

        _offsetX = minX;
        _offsetY = minY;
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var newX = levelPosition.X - _offsetX;
        var newY = levelPosition.Y - _offsetY;
        var index = _width * newY + newX;

        return newX >= 0 &&
               newY >= 0 &&
               newX < _width &&
               newY < _height &&
               _levelPositions.GetBit(index);
    }

    public bool IsEmpty => false;
}
