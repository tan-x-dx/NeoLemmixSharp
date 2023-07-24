using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Terrain;

public static class PixelTypeHelpers
{
    public const int PixelTypeArrowOffset = 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanBeDestroyed(this PixelType pixelType, Orientation orientation)
    {
        return pixelType.IsSolidToOrientation(orientation) &&
               !pixelType.IsSteel() &&
               !pixelType.IsVoid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolidToOrientation(this PixelType pixelType, Orientation orientation)
    {
        var flag = (PixelType)(1 << orientation.RotNum);
        return (pixelType & flag) == flag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSteel(this PixelType pixelType)
    {
        return (pixelType & PixelType.Steel) == PixelType.Steel;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this PixelType pixelType)
    {
        return (pixelType & PixelType.Void) == PixelType.Void;
    }
}