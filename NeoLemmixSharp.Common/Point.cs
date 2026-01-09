using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct Point : IEquatable<Point>
{
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
    public bool Equals(Point other) => X == other.X &&
                                       Y == other.Y;
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Point other && Equals(other);
    [DebuggerStepThrough]
    public override int GetHashCode() =>
        3790121 * X +
        2885497 * Y;

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
    public static bool operator ==(Point left, Point right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(Point left, Point right) => !left.Equals(right);

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[1 + Helpers.Int32NumberBufferLength + 1 + Helpers.Int32NumberBufferLength + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in X, 2);
        return NumberFormattingHelpers.TryFormatIntegerSpan(source, destination, NumberFormattingHelpers.FormatParameters.Default, out charsWritten);
    }
}
