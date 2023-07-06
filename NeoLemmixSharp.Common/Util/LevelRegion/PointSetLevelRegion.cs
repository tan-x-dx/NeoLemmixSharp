using NeoLemmixSharp.Common.Util.BitArrays;

namespace NeoLemmixSharp.Common.Util.LevelRegion;

public static class PointSetLevelRegion
{
    private const int CutoffSize = 256;

    public static ILevelRegion GetLevelRegionForPoints(ICollection<LevelPosition> points)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var levelPosition in points)
        {
            minX = Math.Min(minX, levelPosition.X);
            minY = Math.Min(minY, levelPosition.Y);

            maxX = Math.Max(maxX, levelPosition.X);
            maxY = Math.Max(maxY, levelPosition.Y);
        }

        var width = 1 + maxX - minX;
        var height = 1 + maxY - minY;

        if (width > CutoffSize || height > CutoffSize)
            return new HashSetLevelRegion(points);

        var totalNumberOfPoints = width * height;

        if (totalNumberOfPoints > CutoffSize)
            return new HashSetLevelRegion(points);

        IBitArray levelPositions = totalNumberOfPoints > 32
            ? new ArrayBasedBitArray(totalNumberOfPoints)
            : new IntBasedBitArray();

        foreach (var levelPosition in points)
        {
            var index = width * (levelPosition.Y - minX) + (levelPosition.X - minY);
            levelPositions.SetBit(index);
        }

        return new SmallPointSetLevelRegion(levelPositions, minX, minY, width);
    }

    private sealed class HashSetLevelRegion : ILevelRegion
    {
        private readonly HashSet<LevelPosition> _points;

        public HashSetLevelRegion(IEnumerable<LevelPosition> points)
        {
            _points = new HashSet<LevelPosition>(points, LevelPositionEqualityComparer.Instance);
        }

        public bool ContainsPoint(LevelPosition levelPosition) => _points.Contains(levelPosition);
        public bool IsEmpty => _points.Count == 0;
    }

    private sealed class SmallPointSetLevelRegion : ILevelRegion
    {
        private readonly IBitArray _levelPositions;
        private readonly int _offsetX;
        private readonly int _offsetY;
        private readonly int _width;

        public SmallPointSetLevelRegion(
            IBitArray levelPositions,
            int offsetX,
            int offsetY,
            int width)
        {
            _levelPositions = levelPositions;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _width = width;
        }

        public bool ContainsPoint(LevelPosition levelPosition)
        {
            var index = _width * (levelPosition.Y - _offsetY) + (levelPosition.X - _offsetX);
            return _levelPositions.GetBit(index);
        }

        public bool IsEmpty => !_levelPositions.AnyBitsSet;
    }
}
