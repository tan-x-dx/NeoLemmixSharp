using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct Size : IEquatable<Size>
{
    [FieldOffset(0 * sizeof(int))] public readonly int W;
    [FieldOffset(1 * sizeof(int))] public readonly int H;

    [DebuggerStepThrough]
    public Size(int w, int h)
    {
        W = Math.Max(w, 0);
        H = Math.Max(h, 0);
    }

    [DebuggerStepThrough]
    private Size(int w, int h, byte _)
    {
        W = w;
        H = h;
    }

    [Pure]
    [DebuggerStepThrough]
    public Size Transpose() => new(H, W, 0);

    [Pure]
    [DebuggerStepThrough]
    public Size Scale(int widthScaleFactor, int heightScaleFactor)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(widthScaleFactor);
        ArgumentOutOfRangeException.ThrowIfNegative(heightScaleFactor);

        return new Size(W * widthScaleFactor, H * heightScaleFactor, 0);
    }

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Area() => W * H;

    [Pure]
    [DebuggerStepThrough]
    public bool EncompassesPoint(Point p)
    {
        return (uint)p.X < (uint)W &&
               (uint)p.Y < (uint)H;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AssertEncompassesPoint(Point p)
    {
        if (EncompassesPoint(p))
            return;

        ThrowEncompassesPointException(p);
    }

    [DoesNotReturn]
    private static void ThrowEncompassesPointException(Point p) => throw new ArgumentOutOfRangeException(nameof(p), p, "Invalid position");

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndexOfPoint(Point p) => W * p.Y + p.X;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(Size other) => W == other.W &&
                                      H == other.H;
    [Pure]
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Size other && Equals(other);
    [Pure]
    [DebuggerStepThrough]
    public override int GetHashCode() =>
        8322929 * W +
        5282777 * H;

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(Size left, Size right) => !left.Equals(right);

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[1 + NumberFormattingHelpers.Uint32NumberBufferLength + 1 + NumberFormattingHelpers.Uint32NumberBufferLength + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in W, 2);
        return NumberFormattingHelpers.TryFormatIntegerSpan(source, destination, NumberFormattingHelpers.FormatParameters.Default, out charsWritten);
    }
}
