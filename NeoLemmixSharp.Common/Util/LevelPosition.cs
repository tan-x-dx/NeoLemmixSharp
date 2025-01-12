﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly struct LevelPosition : IEquatable<LevelPosition>
{
    public readonly int X;
    public readonly int Y;

    [DebuggerStepThrough]
    public LevelPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    [DebuggerStepThrough]
    public static bool operator ==(LevelPosition left, LevelPosition right) =>
        left.X == right.X &&
        left.Y == right.Y;

    [DebuggerStepThrough]
    public static bool operator !=(LevelPosition left, LevelPosition right) =>
        left.X != right.X ||
        left.Y != right.Y;

    [DebuggerStepThrough]
    public static LevelPosition operator +(LevelPosition left, LevelPosition right) =>
        new(left.X + right.X, left.Y + right.Y);

    [DebuggerStepThrough]
    public static LevelPosition operator -(LevelPosition left, LevelPosition right) =>
        new(left.X - right.X, left.Y - right.Y);

    [DebuggerStepThrough]
    public static LevelPosition operator +(LevelPosition position, LevelSize size) =>
        new(position.X + size.W, position.Y + size.H);

    [DebuggerStepThrough]
    public bool Equals(LevelPosition other) => X == other.X && Y == other.Y;
    public override bool Equals([NotNullWhen(true)] object? obj) => (obj is LevelPosition other) && X == other.X && Y == other.Y;
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
        charsWritten = 0;

        if (destination.Length < 5) // Need 5 chars min
            return false;
        destination[charsWritten++] = '(';

        var couldWriteInt = X.TryFormat(destination[charsWritten..], out var di);
        charsWritten += di;
        if (!couldWriteInt)
            return false;

        if (charsWritten == destination.Length)
            return false;
        destination[charsWritten++] = ',';

        couldWriteInt = Y.TryFormat(destination[charsWritten..], out di);
        charsWritten += di;
        if (!couldWriteInt)
            return false;

        if (charsWritten == destination.Length)
            return false;
        destination[charsWritten++] = ')';

        return true;
    }
}