using NeoLemmixSharp.Common.Util;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

public readonly struct Interval : IEquatable<Interval>
{
    public readonly int Start;
    public readonly int Length;

    public Interval(int start, int length)
    {
        Start = start;
        Length = Math.Max(length, 0);
    }

    public bool Equals(Interval other) => Start == other.Start &&
                                          Length == other.Length;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Interval other && Equals(other);
    public override int GetHashCode() =>
        5120813 * Start +
        1646497 * Length;

    public static bool operator ==(Interval left, Interval right) => left.Equals(right);
    public static bool operator !=(Interval left, Interval right) => !left.Equals(right);

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[1 + Helpers.Int32NumberBufferLength + 1 + Helpers.Uint32NumberBufferLength + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in Start, 2);
        return Helpers.TryFormatSpan(source, destination, out charsWritten);
    }
}
