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
}