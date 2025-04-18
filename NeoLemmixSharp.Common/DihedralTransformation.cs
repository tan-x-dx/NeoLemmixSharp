﻿using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly ref struct DihedralTransformation : IEquatable<DihedralTransformation>
{
    private const int FlipBitShift = 2;

    public readonly Orientation Orientation;
    public readonly FacingDirection FacingDirection;

    public DihedralTransformation(Orientation orientation, FacingDirection facingDirection)
    {
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    public DihedralTransformation(int encodedBits)
    {
        Orientation = new Orientation(encodedBits);
        FacingDirection = new FacingDirection(encodedBits >>> FlipBitShift);
    }

    public DihedralTransformation(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate)
    {
        var o = rotate ? 1 : 0;
        var f = flipHorizontally ? 1 : 0;
        var v = flipVertically ? 1 : 0;

        o |= v << 1;
        f ^= v;

        Orientation = new Orientation(o);
        FacingDirection = new FacingDirection(f);
    }

    [Pure]
    public Size Transform(Size size)
    {
        return Orientation.IsHorizontal()
            ? size.Transpose()
            : size;
    }

    [Pure]
    public ResizeType Transform(ResizeType resizeType)
    {
        return Orientation.IsHorizontal()
            ? resizeType.SwapComponents()
            : resizeType;
    }

    [Pure]
    public RectangularRegion Transform(RectangularRegion region)
    {
        var w = region.W - 1;
        var h = region.H - 1;

        var s = GetRotationCoefficients(out var a, out var b, ref w, ref h);
        s *= FacingDirection.Id;

        var p1 = region.GetBottomRight() - region.Position;
        var p1x = p1.X;
        var p1y = p1.Y;

        var q0x = s + (FacingDirection.DeltaX * w) + region.X;
        var q0y = h + region.Y;

        var q1x = s + (FacingDirection.DeltaX * ((a * p1x) - (b * p1y) + w)) + region.X;
        var q1y = (b * p1x) + (a * p1y) + h + region.Y;

        var q0 = new Point(q0x, q0y);
        var q1 = new Point(q1x, q1y);

        return new RectangularRegion(q0, q1);
    }

    [Pure]
    public Point Transform(
        Point position,
        Size size)
    {
        var x = position.X;
        var y = position.Y;
        var w = size.W - 1;
        var h = size.H - 1;

        var s = GetRotationCoefficients(out var a, out var b, ref w, ref h);
        s *= FacingDirection.Id;

        var x0 = s + (FacingDirection.DeltaX * ((a * x) - (b * y) + w));
        var y0 = (b * x) + (a * y) + h;
        return new Point(x0, y0);
    }

    private int GetRotationCoefficients(out int a, out int b, ref int w, ref int h)
    {
        var wTemp = w;
        var hTemp = h;
        switch (Orientation.RotNum)
        {
            case EngineConstants.DownOrientationRotNum:
                a = 1;
                b = 0;
                w = 0;
                h = 0;
                return wTemp;

            case EngineConstants.LeftOrientationRotNum:
                a = 0;
                b = 1;
                w = hTemp;
                h = 0;
                return hTemp;

            case EngineConstants.UpOrientationRotNum:
                a = -1;
                b = 0;
                // w unchanged
                // h unchanged
                return wTemp;

            case EngineConstants.RightOrientationRotNum:
                a = 0;
                b = -1;
                w = 0;
                h = wTemp;
                return hTemp;

            default:
                a = 0;
                b = 0;
                return Orientation.ThrowOrientationOutOfRangeException<int>(Orientation);
        }
    }

    [Pure]
    public static int Encode(Orientation orientation, FacingDirection facingDirection) => orientation.RotNum | (facingDirection.Id << FlipBitShift);

    [Pure]
    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[5 + 1 + 5];

        Orientation.TryFormat(buffer, out var charsWritten);
        buffer[charsWritten++] = '|';

        FacingDirection.TryFormat(buffer[charsWritten..], out var c);
        charsWritten += c;

        return buffer[..charsWritten].ToString();
    }

    [Pure]
    public bool Equals(DihedralTransformation other) => this == other;

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Equals() on {nameof(DihedralTransformation)} will always throw an exception. Use the equality operator instead.")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override bool Equals(object? obj) => throw new NotSupportedException("It's a ref struct");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"GetHashCode() on {nameof(DihedralTransformation)} will always throw an exception.")]
    public override int GetHashCode() => throw new NotSupportedException("It's a ref struct");
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    [Pure]
    public static bool operator ==(DihedralTransformation left, DihedralTransformation right) =>
        left.Orientation == right.Orientation &&
        left.FacingDirection == right.FacingDirection;

    [Pure]
    public static bool operator !=(DihedralTransformation left, DihedralTransformation right) =>
        left.Orientation != right.Orientation ||
        left.FacingDirection != right.FacingDirection;
}