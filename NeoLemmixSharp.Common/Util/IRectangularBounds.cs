namespace NeoLemmixSharp.Common.Util;

public interface IRectangularBounds
{
    RectangularRegion CurrentBounds { get; }
}

public interface IPreviousRectangularBounds : IRectangularBounds
{
    RectangularRegion PreviousBounds { get; }
}