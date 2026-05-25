using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly struct Interval : IEquatable<Interval>, ISpanFormattable
{
    public readonly int Start;
    public readonly int Length;

    public Interval(int start, int length)
    {
        Start = start;
        Length = Math.Max(length, 0);
    }

    internal Interval(int start, int length, byte _)
    {
        Start = start;
        Length = length;
    }

    public int End => Start + Length;

    public bool Intersects(Interval other)
    {
        return Start < other.End &&
               other.Start < End;
    }

    public bool Equals(Interval other) => Start == other.Start &&
                                          Length == other.Length;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Interval other && Equals(other);
    public override int GetHashCode() =>
        5120813 * Start +
        1646497 * Length;

    public static bool operator ==(Interval left, Interval right) => left.Equals(right);
    public static bool operator !=(Interval left, Interval right) => !left.Equals(right);

    [Pure]
    [DebuggerStepThrough]
    public override string ToString()
    {
        return ToString(default, null);
    }

    [Pure]
    [DebuggerStepThrough]
    [SkipLocalsInit]
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[1 + NumberFormattingHelpers.Int32NumberBufferLength + 1 + NumberFormattingHelpers.Uint32NumberBufferLength + 1];
        TryFormat(buffer, out var charsWritten, format, formatProvider);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in Start, 2);
        return NumberFormattingHelpers.TryFormatIntegerSpan(source, destination, NumberFormattingHelpers.FormatParameters.Default, out charsWritten);
    }
}
