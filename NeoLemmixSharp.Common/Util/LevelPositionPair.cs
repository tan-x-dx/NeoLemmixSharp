using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly ref struct LevelPositionPair
{
    public readonly int P1X;
    public readonly int P1Y;

    public readonly int P2X;
    public readonly int P2Y;

    public LevelPositionPair(LevelPosition p1, LevelPosition p2)
    {
        P1X = Math.Min(p1.X, p2.X);
        P1Y = Math.Min(p1.Y, p2.Y);

        P2X = Math.Max(p1.X, p2.X);
        P2Y = Math.Max(p1.Y, p2.Y);
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