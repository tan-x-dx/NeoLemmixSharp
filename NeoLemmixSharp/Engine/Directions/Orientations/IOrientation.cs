using System;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public interface IOrientation : IEquatable<IOrientation>
{
    LevelPosition MoveRight(LevelPosition position, int step);
    LevelPosition MoveUp(LevelPosition position, int step);
    LevelPosition MoveLeft(LevelPosition position, int step);
    LevelPosition MoveDown(LevelPosition position, int step);
    LevelPosition Move(LevelPosition position, LevelPosition relativeDirection);
}