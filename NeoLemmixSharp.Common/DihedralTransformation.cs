using System.ComponentModel;
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
        var transformationData = new TransformationData(Orientation, FacingDirection, region.Size);

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
    public static int Encode(Orientation orientation, FacingDirection facingDirection) => orientation.RotNum | (facingDirection.Id << FlipBitShift);

    public readonly ref struct TransformationData
    {
        private readonly int _a;
        private readonly int _b;
        private readonly int _w;
        private readonly int _h;
        private readonly int _s;
        private readonly int _d;

        public TransformationData(
            Orientation orientation,
            FacingDirection facingDirection,
            Size size)
        {
            _w = size.W - 1;
            _h = size.H - 1;

            _s = GetRotationCoefficients(orientation, out _a, out _b, ref _w, ref _h);
            _s *= facingDirection.Id;
            _d = facingDirection.DeltaX;
        }

        private static int GetRotationCoefficients(
            Orientation orientation,
            out int a,
            out int b,
            ref int w,
            ref int h)
        {
            var wTemp = w;
            var hTemp = h;
            switch (orientation.RotNum)
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
                    return Orientation.ThrowOrientationOutOfRangeException<int>(orientation);
            }
        }

        public Point Transform(Point p)
        {
            var x0 = _s + (_d * ((_a * p.X) - (_b * p.Y) + _w));
            var y0 = (_b * p.X) + (_a * p.Y) + _h;
            return new Point(x0, y0);
        }
    }

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