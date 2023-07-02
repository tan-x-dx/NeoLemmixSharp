using System.Collections.ObjectModel;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Terrain;

namespace NeoLemmixSharp.Engine.Engine.Orientations;

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

    public abstract LevelPosition MoveRight(LevelPosition position, int step);
    public abstract LevelPosition MoveUp(LevelPosition position, int step);
    public abstract LevelPosition MoveLeft(LevelPosition position, int step);
    public abstract LevelPosition MoveDown(LevelPosition position, int step);

    /// <summary>
    /// Note: For the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    public abstract LevelPosition Move(LevelPosition position, LevelPosition relativeDirection);
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    public abstract LevelPosition Move(LevelPosition position, int dx, int dy);

    public abstract bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition);
    public abstract bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition);
    public abstract bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    public abstract bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    public abstract bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    public abstract bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition);

    public bool IsParallelTo(Orientation other) => AbsoluteVerticalComponent == 0 == (other.AbsoluteVerticalComponent == 0);
    public bool IsPerpendicularTo(Orientation other) => AbsoluteVerticalComponent == 0 == (other.AbsoluteHorizontalComponent == 0);

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