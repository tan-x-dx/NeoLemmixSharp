using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Terrain;

public static class PixelTypeHelpers
{
    public static bool IsSolidToOrientation(this PixelType pixelType, Orientation orientation)
    {
        var flag = (PixelType)(1 << orientation.RotNum);
        return (pixelType & flag) == flag;
    }

    public static bool IsSteel(this PixelType pixelType)
    {
        return (pixelType & PixelType.Steel) == PixelType.Steel;
    }
}