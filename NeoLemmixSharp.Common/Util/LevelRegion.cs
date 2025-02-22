using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

/// <summary>
/// <para>Represents a rectangular region of points within a level, specified by a <see cref="LevelPosition"/> and a <see cref="LevelSize"/>.</para>
/// <para>The constructors will ensure a well-formed <see cref="LevelRegion"/> is created.</para>
/// <para>Note that a <see cref="LevelRegion"/> can never be empty - the smallest region size is 1x1.</para>
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(int))]
public readonly struct LevelRegion : IEquatable<LevelRegion>
{
    [FieldOffset(0 * sizeof(int))] public readonly LevelPosition P;
    [FieldOffset(0 * sizeof(int))] public readonly int X;
    [FieldOffset(1 * sizeof(int))] public readonly int Y;

    [FieldOffset(2 * sizeof(int))] public readonly LevelSize S;
    [FieldOffset(2 * sizeof(int))] public readonly int W;
    [FieldOffset(3 * sizeof(int))] public readonly int H;

    [DebuggerStepThrough]
    public LevelRegion()
    {
        X = 0;
        Y = 0;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public LevelRegion(LevelPosition position)
    {
        P = position;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public LevelRegion(LevelPosition position, LevelSize size)
    {
        P = position;
        W = size.W;
        if (W < 1) W = 1;
        H = size.H;
        if (H < 1) H = 1;
    }

    [DebuggerStepThrough]
    public LevelRegion(Rectangle rect)
    {
        X = rect.X;
        Y = rect.Y;
        W = rect.Width;
        if (W < 1) W = 1;
        H = rect.Height;
        if (H < 1) H = 1;
    }

    [DebuggerStepThrough]
    public LevelRegion(LevelPosition p1, LevelPosition p2)
    {
        if (p1.X < p2.X)
        {
            X = p1.X;
            W = p2.X - p1.X;
        }
        else
        {
            X = p2.X;
            W = p1.X - p2.X;
        }
        W++;

        if (p1.Y < p2.Y)
        {
            Y = p1.Y;
            H = p2.Y - p1.Y;
        }
        else
        {
            Y = p2.Y;
            H = p1.Y - p2.Y;
        }
        H++;
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

        X = minX;
        Y = minY;
        W = 1 + maxX - minX;
        H = 1 + maxY - minY;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition GetBottomRight() => new(X + W - 1, Y + H - 1);

    [DebuggerStepThrough]
    public bool Overlaps(LevelRegion other)
    {
        return other.X < X + W &&
               X < other.X + other.W &&
               other.Y < Y + H &&
               Y < other.Y + other.H;
    }

    [DebuggerStepThrough]
    public static bool operator ==(LevelRegion left, LevelRegion right) =>
        left.X == right.X &&
        left.Y == right.Y &&
        left.W == right.W &&
        left.H == right.H;

    [DebuggerStepThrough]
    public static bool operator !=(LevelRegion left, LevelRegion right) =>
        left.X != right.X ||
        left.Y != right.Y ||
        left.W != right.W ||
        left.H != right.H;

    [DebuggerStepThrough]
    public bool Equals(LevelRegion other) =>
        X == other.X &&
        Y == other.Y &&
        W == other.W &&
        H == other.H;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is LevelRegion other &&
        X == other.X &&
        Y == other.Y &&
        W == other.W &&
        H == other.H;

    public override int GetHashCode() =>
        6208021 * X +
        4149227 * Y +
        2239063 * W +
        8554379 * H +
        1748359;

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[(1 + 11 + 1 + 10 + 1) * 2];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        if (!P.TryFormat(destination, out charsWritten))
            return false;

        var result = S.TryFormat(destination, out var c);
        charsWritten += c;
        return result;
    }
}