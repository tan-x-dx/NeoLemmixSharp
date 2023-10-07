using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly ref struct LevelPositionPair
{
    public readonly int P1X;
    public readonly int P1Y;

    public readonly int P2X;
    public readonly int P2Y;

    public LevelPositionPair(int p1X, int p1Y, int p2X, int p2Y)
    {
        Debug.Assert(p1X < p2X);
        Debug.Assert(p1Y < p2Y);

        P1X = p1X;
        P1Y = p1Y;

        P2X = p2X;
        P2Y = p2Y;
    }

    public LevelPositionPair(LevelPosition p1, LevelPosition p2)
    {
        P1X = Math.Min(p1.X, p2.X);
        P1Y = Math.Min(p1.Y, p2.Y);

        P2X = Math.Max(p1.X, p2.X);
        P2Y = Math.Max(p1.Y, p2.Y);
    }

    public LevelPositionPair(LevelPosition p1, LevelPosition p2, LevelPosition p3, LevelPosition p4)
    {
        var x0 = Math.Min(p1.X, p2.X);
        var y0 = Math.Min(p1.Y, p2.Y);

        var x1 = Math.Min(p3.X, p4.X);
        var y1 = Math.Min(p3.Y, p4.Y);

        P1X = Math.Min(x0, x1);
        P1Y = Math.Min(y0, y1);

        x0 = Math.Max(p1.X, p2.X);
        y0 = Math.Max(p1.Y, p2.Y);

        x1 = Math.Max(p3.X, p4.X);
        y1 = Math.Max(p3.Y, p4.Y);

        P2X = Math.Max(x0, x1);
        P2Y = Math.Max(y0, y1);
    }

    public LevelPositionPair(ReadOnlySpan<LevelPosition> positions)
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

        P1X = minX;
        P1Y = minY;
        P2X = maxX;
        P2Y = maxY;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition GetTopLeftPosition() => new(P1X, P1Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition GetBottomRightPosition() => new(P2X, P2Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(LevelPositionPair other)
    {
        return other.P1X <= P2X &&
               P1X <= other.P2X &&
               other.P1Y <= P2Y &&
               P1Y <= other.P2Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(LevelPosition anchorPosition)
    {
        return P1X <= anchorPosition.X && anchorPosition.X <= P2X &&
               P1Y <= anchorPosition.Y && anchorPosition.Y <= P2Y;
    }
}