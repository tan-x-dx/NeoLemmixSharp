using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public interface IOrientation : IEquatable<IOrientation>
{
    public static ReadOnlyCollection<IOrientation> AllOrientations { get; } = GenerateRotationCollection();

    private static ReadOnlyCollection<IOrientation> GenerateRotationCollection()
    {
        var list = new List<IOrientation>
        {
            DownOrientation.Instance,
            LeftOrientation.Instance,
            UpOrientation.Instance,
            RightOrientation.Instance
        };

        return new ReadOnlyCollection<IOrientation>(list);
    }

    int RotNum { get; }

    LevelPosition MoveRight(LevelPosition position, int step);
    LevelPosition MoveUp(LevelPosition position, int step);
    LevelPosition MoveLeft(LevelPosition position, int step);
    LevelPosition MoveDown(LevelPosition position, int step);
    LevelPosition Move(LevelPosition position, LevelPosition relativeDirection);
}