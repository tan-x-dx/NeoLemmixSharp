namespace NeoLemmixSharp.Common.Util;

public interface IRectangularBounds
{
    Region CurrentBounds { get; }
}

public interface IPreviousRectangularBounds : IRectangularBounds
{
    Region PreviousBounds { get; }
}