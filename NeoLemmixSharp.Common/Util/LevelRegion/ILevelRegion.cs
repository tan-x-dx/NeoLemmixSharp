namespace NeoLemmixSharp.Common.Util.LevelRegion;

public interface ILevelRegion
{
    bool ContainsPoint(LevelPosition levelPosition);

    bool IsEmpty { get; }
}

public interface IRectangularLevelRegion : ILevelRegion
{
    int X { get; }
    int Y { get; }
    int W { get; }
    int H { get; }

    int X1 { get; }
    int Y1 { get; }
}