using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

/// <summary>
/// <para>Represents a rectangular region of points, specified by a <see cref="Point"/> and a <see cref="Common.Size"/>.</para>
/// <para>The constructors will ensure a well-formed <see cref="RectangularRegion"/> is created.</para>
/// <para>Note that a <see cref="RectangularRegion"/> can never be empty - the smallest region size is 1x1.</para>
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(int))]
public readonly struct RectangularRegion : IEquatable<RectangularRegion>
{
    [FieldOffset(0 * sizeof(int))] public readonly Point Position;
    [FieldOffset(0 * sizeof(int))] public readonly int X;
    [FieldOffset(1 * sizeof(int))] public readonly int Y;

    [FieldOffset(2 * sizeof(int))] public readonly Size Size;
    [FieldOffset(2 * sizeof(int))] public readonly int W;
    [FieldOffset(3 * sizeof(int))] public readonly int H;

    [DebuggerStepThrough]
    public RectangularRegion()
    {
        X = 0;
        Y = 0;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position)
    {
        X = position.X;
        Y = position.Y;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Size size)
    {
        X = 0;
        Y = 0;
        W = Math.Max(size.W, 1);
        H = Math.Max(size.H, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position, Size size)
    {
        X = position.X;
        Y = position.Y;
        W = Math.Max(size.W, 1);
        H = Math.Max(size.H, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Rectangle rect)
    {
        X = rect.X;
        Y = rect.Y;
        W = Math.Max(rect.Width, 1);
        H = Math.Max(rect.Height, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Interval horizontalRegion, Interval verticalRegion)
    {
        X = horizontalRegion.Start;
        Y = verticalRegion.Start;
        W = Math.Max(horizontalRegion.Length, 1);
        H = Math.Max(verticalRegion.Length, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Texture2D texture)
    {
        X = 0;
        Y = 0;
        W = texture.Width;
        H = texture.Height;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point p1, Point p2)
    {
        X = Math.Min(p1.X, p2.X);
        Y = Math.Min(p1.Y, p2.Y);
        var w0 = p1.X - p2.X;
        var h0 = p1.Y - p2.Y;
        W = w0 < 0 ? -w0 : w0;
        H = h0 < 0 ? -h0 : h0;
        W++;
        H++;
    }

    [DebuggerStepThrough]
    public RectangularRegion(ReadOnlySpan<Point> positions)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var p in positions)
        {
            if (p.X < minX)
                minX = p.X;
            if (p.X > maxX)
                maxX = p.X;
            if (p.Y < minY)
                minY = p.Y;
            if (p.Y > maxY)
                maxY = p.Y;
        }

        X = minX;
        Y = minY;
        W = 1 + maxX - minX;
        H = 1 + maxY - minY;
    }

    [DebuggerStepThrough]
    private RectangularRegion(Point position, int w, int h)
    {
        X = position.X;
        Y = position.Y;
        W = w;
        H = h;
    }

    public unsafe RectangularRegion(void* pointer)
    {
        int* intPointer = (int*)pointer;

        X = intPointer[0];
        Y = intPointer[1];
        W = Math.Max(intPointer[2], 1);
        H = Math.Max(intPointer[3], 1);
    }

    [Pure]
    public static RectangularRegion Combine(RectangularRegion first, RectangularRegion second)
    {
        var minX = Math.Min(first.X, second.X);
        var minY = Math.Max(first.Y, second.Y);

        var firstBottomRight = first.GetBottomRight();
        var secondBottomRight = second.GetBottomRight();

        var maxX = Math.Max(firstBottomRight.X, secondBottomRight.X);
        var maxY = Math.Max(firstBottomRight.Y, secondBottomRight.Y);

        var w = 1 + maxX - minX;
        var h = 1 + maxY - minY;

        return new RectangularRegion(new Point(minX, minY), w, h);
    }

    [Pure]
    [DebuggerStepThrough]
    public RectangularRegion Translate(Point offset) => new(Position + offset, W, H);

    public bool Contains(Point point) => Size.EncompassesPoint(point - Position);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point GetBottomRight() => new(X + W - 1, Y + H - 1);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetHorizontalInterval() => new(X, W);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetVerticalInterval() => new(Y, H);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(RectangularRegion left, RectangularRegion right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(RectangularRegion left, RectangularRegion right) => !left.Equals(right);

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(RectangularRegion other) => X == other.X &&
                                                   Y == other.Y &&
                                                   W == other.W &&
                                                   H == other.H;

    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RectangularRegion other && Equals(other);

    [Pure]
    public override int GetHashCode() =>
        6208021 * X +
        4149227 * Y +
        2239063 * W +
        8554379 * H;

    [Pure]
    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[(1 + NumberFormattingHelpers.Int32NumberBufferLength + 1 + NumberFormattingHelpers.Uint32NumberBufferLength + 1) * 2];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        if (!Position.TryFormat(destination, out charsWritten))
            return false;

        var result = Size.TryFormat(destination[charsWritten..], out var c);
        charsWritten += c;
        return result;
    }

    public Rectangle ToRectangle() => new(X, Y, W, H);
}
