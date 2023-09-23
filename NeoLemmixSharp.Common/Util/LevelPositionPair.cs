using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly ref struct LevelPositionPair
{
    private readonly int _p1X;
    private readonly int _p1Y;

    private readonly int _p2X;
    private readonly int _p2Y;

    public LevelPositionPair(int p1X, int p1Y, int p2X, int p2Y)
    {
        Debug.Assert(p1X < p2X);
        Debug.Assert(p1Y < p2Y);

        _p1X = p1X;
        _p1Y = p1Y;
        _p2X = p2X;
        _p2Y = p2Y;
    }

    public LevelPositionPair(LevelPosition p1, LevelPosition p2)
    {
        _p1X = Math.Min(p1.X, p2.X);
        _p1Y = Math.Min(p1.Y, p2.Y);

        _p2X = Math.Max(p1.X, p2.X);
        _p2Y = Math.Max(p1.Y, p2.Y);
    }

    public LevelPositionPair(LevelPosition p1, LevelPosition p2, LevelPosition p3, LevelPosition p4)
    {
        var x0 = Math.Min(p1.X, p2.X);
        var y0 = Math.Min(p1.Y, p2.Y);

        var x1 = Math.Min(p3.X, p4.X);
        var y1 = Math.Min(p3.Y, p4.Y);

        _p1X = Math.Min(x0, x1);
        _p1Y = Math.Min(y0, y1);

        x0 = Math.Max(p1.X, p2.X);
        y0 = Math.Max(p1.Y, p2.Y);

        x1 = Math.Max(p3.X, p4.X);
        y1 = Math.Max(p3.Y, p4.Y);

        _p2X = Math.Max(x0, x1);
        _p2Y = Math.Max(y0, y1);
    }

    public LevelPositionPair(Span<LevelPosition> positions)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var position in positions)
        {
            minX = Math.Min(minX, position.X);
            minY = Math.Min(minY, position.Y);
            maxX = Math.Max(maxX, position.X);
            maxY = Math.Max(maxY, position.Y);
        }

        _p1X = minX;
        _p1Y = minY;
        _p2X = maxX;
        _p2Y = maxY;
    }

    public LevelPosition GetTopLeftPosition() => new(_p1X, _p1Y);
    public LevelPosition GetBottomRightPosition() => new(_p2X, _p2Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(LevelPositionPair other)
    {
        return other._p1X <= _p2X &&
               _p1X <= other._p2X &&
               other._p1Y <= _p2Y &&
               _p1Y <= other._p2Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(LevelPosition anchorPosition)
    {
        return _p1X <= anchorPosition.X && anchorPosition.X <= _p2X &&
               _p1Y <= anchorPosition.Y && anchorPosition.Y <= _p2Y;
    }
}