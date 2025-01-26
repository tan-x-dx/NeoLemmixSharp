using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util;

public readonly struct LevelSize : IEquatable<LevelSize>
{
    public readonly int W;
    public readonly int H;

    [DebuggerStepThrough]
    public LevelSize(int w, int h)
    {
        W = w;
        if (W < 0) W = 0;
        H = h;
        if (H < 0) H = 0;
    }

    [DebuggerStepThrough]
    public static bool operator ==(LevelSize left, LevelSize right) =>
        left.W == right.W &&
        left.H == right.H;

    [DebuggerStepThrough]
    public static bool operator !=(LevelSize left, LevelSize right) =>
        left.W != right.W ||
        left.H != right.H;

    [DebuggerStepThrough]
    public bool Equals(LevelSize other) => W == other.W && H == other.H;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is LevelSize other && W == other.W && H == other.H;
    public override int GetHashCode() => 8322929 * W +
                                         5282777 * H +
                                         4685531;

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[1 + 11 + 1 + 11 + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in W, 2);
        return Helpers.TryFormatSpan(source, destination, out charsWritten);
    }
}