namespace NeoLemmixSharp.Common.Util;

public interface IRectangularBounds
{
    LevelPosition TopLeftPixel { get; }
    LevelPosition BottomRightPixel { get; }
}