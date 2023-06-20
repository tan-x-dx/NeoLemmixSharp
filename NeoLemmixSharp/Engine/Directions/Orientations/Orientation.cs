using NeoLemmixSharp.Engine.LevelPixels;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public abstract class Orientation : IEquatable<Orientation>
{
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlyCollection<Orientation> AllOrientations { get; } = GenerateRotationCollection();

    private static ReadOnlyCollection<Orientation> GenerateRotationCollection()
    {
        var list = new List<Orientation>
        {
            DownOrientation.Instance,
            LeftOrientation.Instance,
            UpOrientation.Instance,
            RightOrientation.Instance
        };

        return new ReadOnlyCollection<Orientation>(list);
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    public abstract int RotNum { get; }
    public abstract int AbsoluteHorizontalComponent { get; }
    public abstract int AbsoluteVerticalComponent { get; }

    public abstract LevelPosition TopLeftCornerOfLevel();
    public abstract LevelPosition TopRightCornerOfLevel();
    public abstract LevelPosition BottomLeftCornerOfLevel();
    public abstract LevelPosition BottomRightCornerOfLevel();

    public abstract LevelPosition MoveRight(in LevelPosition position, int step);
    public abstract LevelPosition MoveUp(in LevelPosition position, int step);
    public abstract LevelPosition MoveLeft(in LevelPosition position, int step);
    public abstract LevelPosition MoveDown(in LevelPosition position, int step);

    /// <summary>
    /// Note: For the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    public abstract LevelPosition Move(in LevelPosition position, in LevelPosition relativeDirection);
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    public abstract LevelPosition Move(in LevelPosition position, int dx, int dy);

    public abstract bool MatchesHorizontally(in LevelPosition firstPosition, in LevelPosition secondPosition);
    public abstract bool MatchesVertically(in LevelPosition firstPosition, in LevelPosition secondPosition);
    public abstract bool FirstIsAboveSecond(in LevelPosition firstPosition, in LevelPosition secondPosition);
    public abstract bool FirstIsBelowSecond(in LevelPosition firstPosition, in LevelPosition secondPosition);
    public abstract bool FirstIsToLeftOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition);
    public abstract bool FirstIsToRightOfSecond(in LevelPosition firstPosition, in LevelPosition secondPosition);

    public abstract ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    public abstract ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    public abstract void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite);
    public abstract void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite);

    public bool IsParallelTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteVerticalComponent == 0);
    public bool IsPerpendicularTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteHorizontalComponent == 0);

    public abstract Orientation RotateClockwise();
    public abstract Orientation RotateCounterClockwise();
    public abstract Orientation GetOpposite();

    public bool Equals(Orientation? other) => RotNum == (other?.RotNum ?? -1);
    public sealed override bool Equals(object? obj) => obj is Orientation other && RotNum == other.RotNum;
    public sealed override int GetHashCode() => RotNum;
    public abstract override string ToString();

    public static bool operator ==(Orientation left, Orientation right) => left.RotNum == right.RotNum;
    public static bool operator !=(Orientation left, Orientation right) => left.RotNum != right.RotNum;
}