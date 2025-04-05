using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct LevelPosition : IEquatable<LevelPosition>
{
    [FieldOffset(0 * sizeof(int))] public readonly int X;
    [FieldOffset(1 * sizeof(int))] public readonly int Y;

    [DebuggerStepThrough]
    public LevelPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(LevelPosition left, LevelPosition right) =>
        left.X == right.X &&
        left.Y == right.Y;

    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(LevelPosition left, LevelPosition right) =>
        left.X != right.X ||
        left.Y != right.Y;

    [Pure]
    [DebuggerStepThrough]
    public static LevelPosition operator +(LevelPosition left, LevelPosition right) =>
        new(left.X + right.X, left.Y + right.Y);

    [Pure]
    [DebuggerStepThrough]
    public static LevelPosition operator -(LevelPosition left, LevelPosition right) =>
        new(left.X - right.X, left.Y - right.Y);

    [Pure]
    [DebuggerStepThrough]
    public static LevelPosition operator +(LevelPosition position, LevelSize offset) =>
        new(position.X + offset.W, position.Y + offset.H);

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(LevelPosition other) => X == other.X && Y == other.Y;
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is LevelPosition other && X == other.X && Y == other.Y;
    [DebuggerStepThrough]
    public override int GetHashCode() => 3790121 * X +
                                         2885497 * Y +
                                         1088251;

    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[1 + 11 + 1 + 11 + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in X, 2);
        return Helpers.TryFormatSpan(source, destination, out charsWritten);
    }
}