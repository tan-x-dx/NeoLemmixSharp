using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public static class PixelTypeHelpers
{
    public const int PixelTypeSolidShiftOffset = 0;
    public const int PixelTypeArrowShiftOffset = 4;
    public const int PixelTypeBlockerShiftOffset = 8;
    public const int PixelTypeSteelShift = 14;
    public const int PixelTypeVoidShift = 15;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanBeDestroyed(this PixelType pixelType)
    {
        const PixelType mask = PixelType.Void | PixelType.Steel;

        return (pixelType & mask) == PixelType.Empty;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolidToOrientation(this PixelType pixelType, Orientation orientation)
    {
        var flag = (PixelType)(1 << orientation.RotNum);
        return (pixelType & flag) == flag;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSteel(this PixelType pixelType)
    {
        return (pixelType & PixelType.Steel) == PixelType.Steel;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this PixelType pixelType)
    {
        return (pixelType & PixelType.Void) == PixelType.Void;
    }

    [Pure]
    public static Orientation GetOrientationFromBlockerMask(PixelType pixelType)
    {
        return pixelType switch
        {
            PixelType.BlockerDown => Orientation.Down,
            PixelType.BlockerLeft => Orientation.Left,
            PixelType.BlockerUp => Orientation.Up,
            PixelType.BlockerRight => Orientation.Right,

            _ => ExpectedBlockerMaskPixelType(pixelType)
        };
    }

    [DoesNotReturn]
    private static Orientation ExpectedBlockerMaskPixelType(PixelType pixelType)
    {
        throw new ArgumentOutOfRangeException(nameof(pixelType), pixelType, "Expected single bit set to match blocker masks");
    }
}