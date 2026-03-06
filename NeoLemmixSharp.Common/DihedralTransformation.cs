using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common;

public readonly ref struct DihedralTransformation : IEquatable<DihedralTransformation>
{
    private const int FlipBitShift = 2;

    public readonly Orientation Orientation;
    public readonly FacingDirection FacingDirection;

    [DebuggerStepThrough]
    public DihedralTransformation(Orientation orientation, FacingDirection facingDirection)
    {
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    [DebuggerStepThrough]
    public DihedralTransformation(int encodedBits)
    {
        Orientation = new Orientation(encodedBits);
        FacingDirection = new FacingDirection(encodedBits >>> FlipBitShift);
    }

    [DebuggerStepThrough]
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
    public RectangularRegion Transform(RectangularRegion region) => Transform(region, region.Size);

    [Pure]
    public RectangularRegion Transform(RectangularRegion region, Size size)
    {
        var transformationData = new TransformationData(Orientation, FacingDirection, size);

        var q0 = transformationData.Transform(new Point());
        var q1 = transformationData.Transform(region.GetBottomRight() - region.Position);

        q0 += region.Position;
        q1 += region.Position;

        return new RectangularRegion(q0, q1);
    }

    [Pure]
    public Point Transform(
        Point position,
        Size size)
    {
        var transformationData = new TransformationData(Orientation, FacingDirection, size);
        return transformationData.Transform(position);
    }

    [Pure]
    public static int Encode(Orientation orientation, FacingDirection facingDirection) => (orientation.RotNum & 3) | ((facingDirection.Id & 1) << FlipBitShift);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TransformationData GetTransformationData(Size size)
    {
        return new TransformationData(Orientation, FacingDirection, size);
    }

    public readonly ref struct TransformationData
    {
        private readonly int _cos;
        private readonly int _sin;
        private readonly int _w;
        private readonly int _h;
        private readonly int _facingDirectionOffset;
        private readonly int _facingDirectionDelta;

        public TransformationData(
            Orientation orientation,
            FacingDirection facingDirection,
            Size size)
        {
            var wTemp = size.W - 1;
            var hTemp = size.H - 1;

            var r = orientation.RotNum;
            var s = OrientationConstants.IntSin(r);
            var c = OrientationConstants.IntCos(r);
            _cos = c;
            _sin = s;
            s &= 1;
            c &= 1;

            _facingDirectionOffset = (c * wTemp) + (s * hTemp);
            _facingDirectionOffset *= facingDirection.Id;
            _facingDirectionDelta = facingDirection.DeltaX;

            r--;
            r &= 3;

            var l0 = IsZero(r);
            r--;
            r &= 3;

            var l1 = IsZero(r);
            _w = (l1 * wTemp) + (l0 * hTemp);

            r--;
            r &= 3;

            l0 = IsZero(r);
            _h = (l0 * wTemp) + (l1 * hTemp);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IsZero(int n)
        {
            return n == 0 ? 1 : 0;
        }

        public Point Transform(Point p)
        {
            var x0 = _facingDirectionOffset + (_facingDirectionDelta * ((_cos * p.X) - (_sin * p.Y) + _w));
            var y0 = (_sin * p.X) + (_cos * p.Y) + _h;
            return new Point(x0, y0);
        }
    }

    [Pure]
    [DebuggerStepThrough]
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
    public bool Equals(DihedralTransformation other) => Orientation.Equals(other.Orientation) &&
                                                        FacingDirection.Equals(other.FacingDirection);

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
    [DebuggerStepThrough]
    public static bool operator ==(DihedralTransformation left, DihedralTransformation right) => left.Equals(right);

    [Pure]
    [DebuggerStepThrough]
    public static bool operator !=(DihedralTransformation left, DihedralTransformation right) => !left.Equals(right);
}
