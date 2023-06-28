using System.Collections.Generic;

namespace NeoLemmixSharp.Util.LevelRegion;

public sealed class PointSetLevelRegion : ILevelRegion
{
    private readonly HashSet<LevelPosition> _points;

    public PointSetLevelRegion(IEnumerable<LevelPosition> points)
    {
        _points = new HashSet<LevelPosition>(points, LevelPositionEqualityComparer.Instance);
    }

    public bool ContainsPoint(LevelPosition levelPosition) => _points.Contains(levelPosition);
}