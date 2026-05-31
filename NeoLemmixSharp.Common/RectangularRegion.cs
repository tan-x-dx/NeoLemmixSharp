using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

/// <summary>
/// <para>Represents a rectangular region of points, specified by the top left and bottom right points.</para>
/// <para>The constructors will ensure a well-formed <see cref="RectangularRegion"/> is created.</para>
/// <para>Note that a <see cref="RectangularRegion"/> can never be empty - the smallest region size is 1x1.</para>
/// </summary>
public readonly struct RectangularRegion : IEquatable<RectangularRegion>, ISpanFormattable
{
    public readonly Point TopLeft;
    public readonly Point BottomRight;

    public int X => TopLeft.X;
    public int Y => TopLeft.Y;
    public int W => 1 + BottomRight.X - TopLeft.X;
    public int H => 1 + BottomRight.Y - TopLeft.Y;

    public Size Size => new(W, H, 0);

    [DebuggerStepThrough]
    public RectangularRegion()
    {
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position)
    {
        TopLeft = position;
        BottomRight = position;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Size size)
    {
        TopLeft = Point.Zero;
        var bottomRightX = Math.Max(size.W - 1, 0);
        var bottomRightY = Math.Max(size.H - 1, 0);
        BottomRight = new Point(bottomRightX, bottomRightY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position, Size size)
    {
        TopLeft = position;
        var bottomRightX = Math.Max(size.W - 1, 0);
        var bottomRightY = Math.Max(size.H - 1, 0);
        BottomRight = position + new Point(bottomRightX, bottomRightY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Rectangle rect)
    {
        var position = new Point(rect.X, rect.Y);
        TopLeft = position;
        var bottomRightX = Math.Max(rect.Width - 1, 0);
        var bottomRightY = Math.Max(rect.Height - 1, 0);
        BottomRight = position + new Point(bottomRightX, bottomRightY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Texture2D texture)
    {
        TopLeft = Point.Zero;
        var bottomRightX = Math.Max(texture.Width - 1, 0);
        var bottomRightY = Math.Max(texture.Height - 1, 0);
        BottomRight = new Point(bottomRightX, bottomRightY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Interval horizontalRegion, Interval verticalRegion)
    {
        var position = new Point(horizontalRegion.Start, verticalRegion.Start);
        TopLeft = position;
        var bottomRightX = Math.Max(horizontalRegion.Length - 1, 0);
        var bottomRightY = Math.Max(verticalRegion.Length - 1, 0);
        BottomRight = position + new Point(bottomRightX, bottomRightY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point p1, Point p2)
    {
        var minX = Math.Min(p1.X, p2.X);
        var minY = Math.Min(p1.Y, p2.Y);
        var maxX = Math.Max(p1.X, p2.X);
        var maxY = Math.Max(p1.Y, p2.Y);

        TopLeft = new Point(minX, minY);
        BottomRight = new Point(maxX, maxY);
    }

    [DebuggerStepThrough]
    public RectangularRegion(ReadOnlySpan<Point> positions)
    {
        if (positions.Length == 0)
        {
            TopLeft = default;
            BottomRight = default;
            return;
        }

        var i = positions.Length - 1;
        var p = positions.At(i);

        var minX = p.X;
        var minY = p.Y;
        var maxX = minX;
        var maxY = minY;

        i--;

        while (i >= 0)
        {
            var t = p.X;
            if (minX > t)
                minX = t;
            if (maxX < t)
                maxX = t;
            t = p.Y;
            if (minY > t)
                minY = t;
            if (maxY < t)
                maxY = t;

            i--;
        }

        TopLeft = new Point(minX, minY);
        BottomRight = new Point(maxX, maxY);
    }

    [DebuggerStepThrough]
    private RectangularRegion(Point topLeft, Point bottomRight, byte _)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    [Pure]
    public static RectangularRegion Combine(RectangularRegion first, RectangularRegion second)
    {
        var minX = Math.Min(first.X, second.X);
        var minY = Math.Min(first.Y, second.Y);

        var maxX = Math.Max(first.BottomRight.X, second.BottomRight.X);
        var maxY = Math.Max(first.BottomRight.Y, second.BottomRight.Y);

        return new RectangularRegion(new Point(minX, minY), new Point(maxX, maxY), 0);
    }

    [Pure]
    [DebuggerStepThrough]
    public RectangularRegion Translate(Point offset) => new(TopLeft + offset, BottomRight + offset, 0);

    public bool Contains(Point point)
    {
        return X <= point.X &&
               point.X <= BottomRight.X &&
               Y <= point.Y &&
               point.Y <= BottomRight.Y;
    }

    public bool Overlaps(RectangularRegion other)
    {
        return X <= other.BottomRight.X &&
               other.X <= BottomRight.X &&
               Y <= other.BottomRight.Y &&
               other.Y <= BottomRight.Y;
    }

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetHorizontalInterval() => new(X, W, 0);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetVerticalInterval() => new(Y, H, 0);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(RectangularRegion left, RectangularRegion right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(RectangularRegion left, RectangularRegion right) => !left.Equals(right);

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(RectangularRegion other) => TopLeft == other.TopLeft &&
                                                   BottomRight == other.BottomRight;

    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RectangularRegion other && Equals(other);

    [Pure]
    public override int GetHashCode() =>
        353 * TopLeft.GetHashCode() +
        719 * BottomRight.GetHashCode();

    [Pure]
    [DebuggerStepThrough]
    public override string ToString() => ToString(default, null);

    [Pure]
    [DebuggerStepThrough]
    [SkipLocalsInit]
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[(1 + NumberFormattingHelpers.Int32NumberBufferLength + 1 + NumberFormattingHelpers.Int32NumberBufferLength + 1) * 2];
        TryFormat(buffer, out var charsWritten, format, formatProvider);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (!TopLeft.TryFormat(destination, out charsWritten, format, provider))
            return false;

        var result = BottomRight.TryFormat(destination[charsWritten..], out var c, format, provider);
        charsWritten += c;
        return result;
    }

    public Rectangle ToRectangle() => new(X, Y, W, H);
}
