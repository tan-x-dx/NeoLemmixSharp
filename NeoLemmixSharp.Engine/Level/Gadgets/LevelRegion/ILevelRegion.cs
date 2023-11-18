using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

public interface ILevelRegion
{
    bool ContainsPoint(LevelPosition levelPosition);
}

public interface IRectangularLevelRegion : ILevelRegion
{
    int X { get; }
    int Y { get; }
    int W { get; }
    int H { get; }

    int X1 { get; }
    int Y1 { get; }

    LevelPosition TopLeft { get; }
    LevelPosition BottomRight { get; }
    Rectangle ToRectangle();
}