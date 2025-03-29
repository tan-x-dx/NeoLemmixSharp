using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct LevelSize : IEquatable<LevelSize>
{
    [FieldOffset(0 * sizeof(int))] public readonly int W;
    [FieldOffset(1 * sizeof(int))] public readonly int H;

    [DebuggerStepThrough]
    public LevelSize(int w, int h)
    {
        W = w;
        if (W < 0) W = 0;
        H = h;
        if (H < 0) H = 0;
    }

    [DebuggerStepThrough]
    public LevelSize(Texture2D texture)
    {
        W = texture.Width;
        H = texture.Height;
    }

    [Pure]
    public LevelSize Transpose() => new(H, W);

    [Pure]
    public int Area() => W * H;

    [Pure]
    public bool EncompassesPoint(LevelPosition p)
    {
        return (uint)p.X < (uint)W &&
               (uint)p.Y < (uint)H;
    }

    public void AssertEncompassesPoint(LevelPosition p)
    {
        if (EncompassesPoint(p))
            return;

        throw new ArgumentOutOfRangeException(nameof(p), p, "Invalid position");
    }

    [Pure]
    public int GetIndexOfPoint(LevelPosition p) => W * p.Y + p.X;

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
        Span<char> buffer = stackalloc char[1 + 10 + 1 + 10 + 1];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        var source = MemoryMarshal.CreateReadOnlySpan(in W, 2);
        return Helpers.TryFormatSpan(source, destination, out charsWritten);
    }
}