using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

/// <summary>
/// <para>Represents a rectangular region of points within a level.</para>
/// <para>A well-formed <see cref="LevelRegion"/> has the P1 points less than or equal to the P2 points.
/// The constructors will ensure a well-formed <see cref="LevelRegion"/> is created.</para>
/// <para>Note that a <see cref="LevelRegion"/> can never be empty - the smallest region is 1x1.</para>
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
public readonly ref struct LevelRegion
{
    [FieldOffset(0)] public readonly LevelPosition P1;
    [FieldOffset(8)] public readonly LevelPosition P2;

    [FieldOffset(0)] public readonly int P1X;
    [FieldOffset(4)] public readonly int P1Y;

    [FieldOffset(8)] public readonly int P2X;
    [FieldOffset(12)] public readonly int P2Y;

    [DebuggerStepThrough]
    public LevelRegion(int x1, int y1, int x2, int y2)
    {
        if (x1 < x2)
        {
            P1X = x1;
            P2X = x2;
        }
        else
        {
            P1X = x2;
            P2X = x1;
        }

        if (y1 < y2)
        {
            P1Y = y1;
            P2Y = y2;
        }
        else
        {
            P1Y = y2;
            P2Y = y1;
        }
    }

    [DebuggerStepThrough]
    public LevelRegion(LevelPosition p1, LevelPosition p2)
    {
        if (p1.X < p2.X)
        {
            P1X = p1.X;
            P2X = p2.X;
        }
        else
        {
            P1X = p2.X;
            P2X = p1.X;
        }

        if (p1.Y < p2.Y)
        {
            P1Y = p1.Y;
            P2Y = p2.Y;
        }
        else
        {
            P1Y = p2.Y;
            P2Y = p1.Y;
        }
    }

    [DebuggerStepThrough]
    public LevelRegion(ReadOnlySpan<LevelPosition> positions)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        for (var i = 0; i < positions.Length; i++)
        {
            var p = positions[i];

            minX = Math.Min(minX, p.X);
            minY = Math.Min(minY, p.Y);
            maxX = Math.Max(maxX, p.X);
            maxY = Math.Max(maxY, p.Y);
        }

        P1X = minX;
        P1Y = minY;
        P2X = maxX;
        P2Y = maxY;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public LevelSize GetSize() => new(1 + P2X - P1X, 1 + P2Y - P1Y);

    [DebuggerStepThrough]
    public bool Overlaps(LevelRegion other)
    {
        return other.P1X <= P2X &&
               P1X <= other.P2X &&
               other.P1Y <= P2Y &&
               P1Y <= other.P2Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public bool Contains(LevelPosition anchorPosition)
    {
        return P1X <= anchorPosition.X && anchorPosition.X <= P2X &&
               P1Y <= anchorPosition.Y && anchorPosition.Y <= P2Y;
    }
}