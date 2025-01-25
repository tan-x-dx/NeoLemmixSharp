namespace NeoLemmixSharp.Common.Util;

public interface IRectangularBounds
{
    LevelRegion CurrentBounds { get; }
}

public interface IPreviousRectangularBounds : IRectangularBounds
{
    LevelRegion PreviousBounds { get; }
}