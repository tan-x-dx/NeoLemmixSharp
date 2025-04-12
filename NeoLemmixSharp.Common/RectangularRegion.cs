﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

/// <summary>
/// <para>Represents a rectangular region of points, specified by a <see cref="Point"/> and a <see cref="Common.Size"/>.</para>
/// <para>The constructors will ensure a well-formed <see cref="RectangularRegion"/> is created.</para>
/// <para>Note that a <see cref="RectangularRegion"/> can never be empty - the smallest region size is 1x1.</para>
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(int))]
public readonly struct RectangularRegion : IEquatable<RectangularRegion>
{
    [FieldOffset(0 * sizeof(int))] public readonly Point Position;
    [FieldOffset(0 * sizeof(int))] public readonly int X;
    [FieldOffset(1 * sizeof(int))] public readonly int Y;

    [FieldOffset(2 * sizeof(int))] public readonly Size Size;
    [FieldOffset(2 * sizeof(int))] public readonly int W;
    [FieldOffset(3 * sizeof(int))] public readonly int H;

    [DebuggerStepThrough]
    public RectangularRegion()
    {
        X = 0;
        Y = 0;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position)
    {
        Position = position;
        W = 1;
        H = 1;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Size size)
    {
        X = 0;
        Y = 0;
        W = Math.Max(size.W, 1);
        H = Math.Max(size.H, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point position, Size size)
    {
        Position = position;
        W = Math.Max(size.W, 1);
        H = Math.Max(size.H, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Rectangle rect)
    {
        X = rect.X;
        Y = rect.Y;
        W = Math.Max(rect.Width, 1);
        H = Math.Max(rect.Height, 1);
    }

    [DebuggerStepThrough]
    public RectangularRegion(Texture2D texture)
    {
        X = 0;
        Y = 0;
        W = texture.Width;
        H = texture.Height;
    }

    [DebuggerStepThrough]
    public RectangularRegion(Point p1, Point p2)
    {
        if (p1.X < p2.X)
        {
            X = p1.X;
            W = p2.X - p1.X;
        }
        else
        {
            X = p2.X;
            W = p1.X - p2.X;
        }
        W++;

        if (p1.Y < p2.Y)
        {
            Y = p1.Y;
            H = p2.Y - p1.Y;
        }
        else
        {
            Y = p2.Y;
            H = p1.Y - p2.Y;
        }
        H++;
    }

    [DebuggerStepThrough]
    public RectangularRegion(ReadOnlySpan<Point> positions)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        for (var i = 0; i < positions.Length; i++)
        {
            var p = positions[i];

            minX = Math.Min(minX, p.X);
            minY = Math.Min(minY, p.Y);
            maxX = Math.Max(maxX, p.X);
            maxY = Math.Max(maxY, p.Y);
        }

        X = minX;
        Y = minY;
        W = 1 + maxX - minX;
        H = 1 + maxY - minY;
    }

    [DebuggerStepThrough]
    private RectangularRegion(Point position, int w, int h)
    {
        Position = position;
        W = w;
        H = h;
    }

    [Pure]
    [DebuggerStepThrough]
    public RectangularRegion Translate(Point offset) => new(Position + offset, W, H);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point GetBottomRight() => new(X + W - 1, Y + H - 1);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetHorizontalInterval() => new(X, W);

    [Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interval GetVerticalInterval() => new(Y, H);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator ==(RectangularRegion left, RectangularRegion right) =>
        left.X == right.X &&
        left.Y == right.Y &&
        left.W == right.W &&
        left.H == right.H;

    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(RectangularRegion left, RectangularRegion right) =>
        left.X != right.X ||
        left.Y != right.Y ||
        left.W != right.W ||
        left.H != right.H;

    [Pure]
    [DebuggerStepThrough]
    public bool Equals(RectangularRegion other) => this == other;

    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RectangularRegion other && this == other;

    [Pure]
    public override int GetHashCode() =>
        6208021 * X +
        4149227 * Y +
        2239063 * W +
        8554379 * H +
        1748359;

    [Pure]
    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[(1 + 11 + 1 + 10 + 1) * 2];
        TryFormat(buffer, out var charsWritten);
        return buffer[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        if (!Position.TryFormat(destination, out charsWritten))
            return false;

        var result = Size.TryFormat(destination[charsWritten..], out var c);
        charsWritten += c;
        return result;
    }

    public Rectangle ToRectangle() => new(X, Y, W, H);
}