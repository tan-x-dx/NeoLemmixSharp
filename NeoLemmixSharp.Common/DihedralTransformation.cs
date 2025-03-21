using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common;

[StructLayout(LayoutKind.Explicit, Size = NumberOf32BitInts * sizeof(int))]
public readonly ref struct DihedralTransformation : IEquatable<DihedralTransformation>
{
    private const int NumberOf32BitInts = 2;
    private const int FlipBitShift = 2;

    [FieldOffset(0 * sizeof(int))] public readonly Orientation Orientation;
    [FieldOffset(1 * sizeof(int))] public readonly FacingDirection FacingDirection;

    public DihedralTransformation(Orientation orientation, FacingDirection facingDirection)
    {
        Orientation = new Orientation(orientation.RotNum);
        FacingDirection = new FacingDirection(facingDirection.Id);
    }

    [Pure]
    public LevelPosition Transform(
        LevelPosition position,
        int width,
        int height)
    {
        Transform(position.X, position.Y, width, height, out var x0, out var y0);
        return new LevelPosition(x0, y0);
    }

    [Pure]
    public LevelSize Transform(LevelSize levelSize)
    {
        Transform(levelSize.W, levelSize.H, levelSize.W, levelSize.H, out var x0, out var y0);
        return new LevelSize(x0, y0);
    }

    public void Transform(
        int x,
        int y,
        int w,
        int h,
        out int x0,
        out int y0)
    {
        var s = GetRotationCoefficients(out var a, out var b, ref w, ref h);
        s *= FacingDirection.Id;

        x0 = s + FacingDirection.DeltaX * (a * x - b * y + w);
        y0 = b * x + a * y + h;
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
    public static uint EncodeToUint(Orientation o, FacingDirection f)
    {
        return (uint)((o.RotNum & 3) | ((f.Id & 1) << FlipBitShift));
    }

    [Pure]
    public uint EncodeToUint() => EncodeToUint(Orientation, FacingDirection);

    [Pure]
    [SkipLocalsInit]
    public static unsafe DihedralTransformation DecodeFromUint(uint encodedData)
    {
        uint* p = stackalloc uint[NumberOf32BitInts];
        p[0] = encodedData & 3U;
        p[1] = (encodedData >> FlipBitShift) & 1U;

        return *(DihedralTransformation*)p;
    }

    [Pure]
    [SkipLocalsInit]
    public static unsafe DihedralTransformation Simplify(
        bool flipHorizontally,
        bool flipVertically,
        bool rotate)
    {
        uint* p = stackalloc uint[NumberOf32BitInts];
        p[0] = rotate ? 1U : 0U;
        p[1] = flipHorizontally ? 1U : 0U;

        if (flipVertically)
        {
            p[0] += 2U;
            p[0] &= 3U;
            p[1] += 1U;
            p[1] &= 1U;
        }

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
    public bool Equals(DihedralTransformation other) => Orientation == other.Orientation && FacingDirection == other.FacingDirection;
    [Obsolete($"Equals() on {nameof(DihedralTransformation)} will always throw an exception. Use the equality operator instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DoesNotReturn]
    public override bool Equals(object? obj) => throw new NotSupportedException("It's a ref struct");
    [Obsolete($"GetHashCode() on {nameof(DihedralTransformation)} will always throw an exception.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DoesNotReturn]
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