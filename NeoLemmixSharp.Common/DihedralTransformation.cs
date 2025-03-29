using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly ref struct DihedralTransformation : IEquatable<DihedralTransformation>
{
    private const int FlipBitShift = 2;

    [FieldOffset(0 * sizeof(int))] public readonly Orientation Orientation;
    [FieldOffset(1 * sizeof(int))] public readonly FacingDirection FacingDirection;

    public DihedralTransformation(Orientation orientation, FacingDirection facingDirection)
    {
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    [Pure]
    public LevelSize Transform(LevelSize levelSize)
    {
        return Orientation.IsPerpendicularTo(Orientation.Down)
            ? levelSize.Transpose()
            : levelSize;
    }

    [Pure]
    public ResizeType Transform(ResizeType resizeType)
    {
        return Orientation.IsPerpendicularTo(Orientation.Down)
            ? resizeType.SwapComponents()
            : resizeType;
    }

    [Pure]
    public LevelPosition Transform(
        LevelPosition position,
        LevelSize size)
    {
        var x = position.X;
        var y = position.Y;
        var w = size.W - 1;
        var h = size.H - 1;

        var s = GetRotationCoefficients(out var a, out var b, ref w, ref h);
        s *= FacingDirection.Id;

        var x0 = s + FacingDirection.DeltaX * (a * x - b * y + w);
        var y0 = b * x + a * y + h;
        return new LevelPosition(x0, y0);
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
    public static unsafe DihedralTransformation Decode(int encodedData)
    {
        int* p = stackalloc int[2];
        p[0] = encodedData & 3;
        p[1] = (encodedData >> FlipBitShift) & 1;

        return *(DihedralTransformation*)p;
    }

    [Pure]
    [SkipLocalsInit]
    public static unsafe DihedralTransformation Decode(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate)
    {
        var o = rotate ? 1 : 0;
        var f = flipHorizontally ? 1 : 0;
        var v = flipVertically ? 1 : 0;

        o |= v << 1;
        f ^= v;

        int* p = stackalloc int[2] { o, f };
        return *(DihedralTransformation*)p;
    }

    [Pure]
    [SkipLocalsInit]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[5 + 1 + 5];
        var charsWritten = 0;

        // The following usages of ToString() will return string consts
        // Therefore this will not incur any extra allocations
        var sourceSpan = Orientation.ToString().AsSpan();
        sourceSpan.CopyTo(buffer);
        charsWritten += sourceSpan.Length;

        buffer[charsWritten++] = '|';

        sourceSpan = FacingDirection.ToString().AsSpan();
        sourceSpan.CopyTo(buffer[charsWritten..]);
        charsWritten += sourceSpan.Length;

        return buffer[..charsWritten].ToString();
    }

    [Pure]
    public bool Equals(DihedralTransformation other) =>
        Orientation == other.Orientation &&
        FacingDirection == other.FacingDirection;

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"Equals() on {nameof(DihedralTransformation)} will always throw an exception. Use the equality operator instead.")]
    public override bool Equals(object? obj) => throw new NotSupportedException("It's a ref struct");

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete($"GetHashCode() on {nameof(DihedralTransformation)} will always throw an exception.")]
    public override int GetHashCode() => throw new NotSupportedException("It's a ref struct");

    [Pure]
    public static bool operator ==(DihedralTransformation left, DihedralTransformation right) =>
        left.Orientation == right.Orientation &&
        left.FacingDirection == right.FacingDirection;

    [Pure]
    public static bool operator !=(DihedralTransformation left, DihedralTransformation right) =>
        left.Orientation != right.Orientation ||
        left.FacingDirection != right.FacingDirection;
}