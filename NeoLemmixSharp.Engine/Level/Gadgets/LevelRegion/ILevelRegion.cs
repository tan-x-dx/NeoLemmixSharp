﻿using Microsoft.Xna.Framework;
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

public static class LevelRegionHelpers
{
    public static LevelPosition GetRelativePosition(LevelPosition basePosition, LevelPosition testPosition)
    {
        var dx = LevelScreen.HorizontalBoundaryBehaviour.GetDelta(basePosition.X, testPosition.X);
        var dy = LevelScreen.VerticalBoundaryBehaviour.GetDelta(basePosition.Y, testPosition.Y);

        return new LevelPosition(dx, dy);
    }

    public static bool ContainsPoint(IRectangularLevelRegion levelRegion, LevelPosition levelPosition)
    {
        var thisTestX = levelRegion.X;
        var thisTestX1 = levelRegion.X1;
        var levelPositionTestX = levelPosition.X;

        var thisTestY = levelRegion.Y;
        var thisTestY1 = levelRegion.Y1;
        var levelPositionTestY = levelPosition.Y;

        LevelScreen.HorizontalBoundaryBehaviour.NormaliseCoords(ref thisTestX, ref thisTestX1, ref levelPositionTestX);
        LevelScreen.VerticalBoundaryBehaviour.NormaliseCoords(ref thisTestY, ref thisTestY1, ref levelPositionTestY);

        return thisTestX <= levelPositionTestX &&
               thisTestY <= levelPositionTestY &&
               levelPositionTestX < thisTestX1 &&
               levelPositionTestY < thisTestY1;
    }
}