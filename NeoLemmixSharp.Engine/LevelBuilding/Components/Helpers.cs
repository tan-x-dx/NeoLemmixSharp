using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public static class Helpers
{
    public const int PositionOffset = 512;

    private const int FlipBitShift = 2;

    public static (Orientation, FacingDirection) DecipherOrientations(byte b)
    {
        var rotNum = b & 3;
        var orientation = Orientation.AllItems[rotNum];
        
        var flip = (b >> FlipBitShift) & 1;
        var facingDirection = FacingDirection.AllItems[flip];

        return (orientation, facingDirection);
    }

    public static void ReaderAssert(bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelReadingException($"Error occurred when reading level file. Details: [{details}]");
    }
}