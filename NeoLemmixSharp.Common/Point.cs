using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct Point : IEquatable<Point>,
    ISpanFormattable,
    IAdditionOperators<Point, Point, Point>,
    IAdditiveIdentity<Point, Point>,
    IEqualityOperators<Point, Point, bool>,
    ISubtractionOperators<Point, Point, Point>,
    IUnaryNegationOperators<Point, Point>,
    IUnaryPlusOperators<Point, Point>
{
    static Point IAdditiveIdentity<Point, Point>.AdditiveIdentity => Zero;
    public static Point Zero => new();

    [FieldOffset(0 * sizeof(int))] public readonly int X;
    [FieldOffset(1 * sizeof(int))] public readonly int Y;

    [DebuggerStepThrough]
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(Point other) => this == other;

    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Point other && Equals(other);
    [DebuggerStepThrough]
    public override int GetHashCode() =>
        3790121 * X +
        2885497 * Y;

    [Pure]
    [DebuggerStepThrough]
    public static Point operator +(Point p) => p;

    [Pure]
    [DebuggerStepThrough]
    public static Point operator -(Point p) => new(-p.X, -p.Y);

    [Pure]
    [DebuggerStepThrough]
    public static Point operator +(Point left, Point right) =>
        new(left.X + right.X, left.Y + right.Y);

    [Pure]
    [DebuggerStepThrough]
    public static Point operator -(Point left, Point right) =>
        new(left.X - right.X, left.Y - right.Y);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(Point left, Point right)
    {
        var leftLong = Unsafe.BitCast<Point, long>(left);
        var rightLong = Unsafe.BitCast<Point, long>(right);

        return leftLong == rightLong;
    }
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(Point left, Point right) => !(left == right);

    [Pure]
    [DebuggerStepThrough]
    [SkipLocalsInit]
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[1 + NumberFormattingHelpers.Int32NumberBufferLength + 1 + NumberFormattingHelpers.Int32NumberBufferLength + 1];
        TryFormat(buffer, out var charsWritten, format, formatProvider);
        return buffer[..charsWritten].ToString();
    }

    [Pure]
    [DebuggerStepThrough]
    public override string ToString()
    {
        return ToString(default, null);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in X, 2);
        return NumberFormattingHelpers.TryFormatIntegerSpan(source, destination, NumberFormattingHelpers.FormatParameters.Default, out charsWritten);
    }
}
