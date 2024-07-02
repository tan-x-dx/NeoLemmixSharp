using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public static class Helpers
{
    public const int PositionOffset = 512;

    private const int FlipBitShift = 2;

    public static byte GetOrientationByte(Orientation orientation, FacingDirection facingDirection)
    {
        return GetOrientationByte(orientation.RotNum, facingDirection == FacingDirection.LeftInstance);
    }

    public static byte GetOrientationByte(int rotNum, bool flip)
    {
        var orientationBits = (rotNum & 3) |
                              (flip ? 1 << FlipBitShift : 0);

        return (byte)orientationBits;
    }

    public static void ReaderAssert(bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelReadingException($"Error occurred when reading level file. Details: [{details}]");
    }
}