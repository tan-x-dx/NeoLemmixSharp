using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public static class PixelTypeHelpers
{
    public const int PixelTypeSolidShiftOffset = 0;
    public const int PixelTypeArrowShiftOffset = 4;
    public const int PixelTypeSteelShift = 14;
    public const int PixelTypeVoidShift = 15;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanBeDestroyed(this PixelType pixelType)
    {
        const uint mask = (1U << PixelTypeSteelShift) |
                          (1U << PixelTypeVoidShift);

        var pixelTypeInt = (uint)pixelType;
        return (pixelTypeInt & mask) == 0U;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolidToOrientation(this PixelType pixelType, Orientation orientation)
    {
        var pixelTypeInt = (uint)pixelType;
        return ((pixelTypeInt >>> (PixelTypeSolidShiftOffset + orientation.RotNum)) & 1U) != 0U;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSteel(this PixelType pixelType)
    {
        var pixelTypeInt = (uint)pixelType;
        return ((pixelTypeInt >>> PixelTypeSteelShift) & 1U) != 0U;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this PixelType pixelType)
    {
        var pixelTypeInt = (uint)pixelType;
        return ((pixelTypeInt >>> PixelTypeVoidShift) & 1U) != 0U;
    }
}