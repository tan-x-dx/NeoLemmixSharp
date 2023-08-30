using System.Diagnostics.Contracts;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public abstract class Orientation : IUniqueIdItem<Orientation>
{
    private static readonly Orientation[] Orientations = GenerateOrientationCollection();
    protected static TerrainManager Terrain { get; private set; }

    public static int NumberOfItems => Orientations.Length;
    public static ReadOnlySpan<Orientation> AllItems => new(Orientations);

    private static Orientation[] GenerateOrientationCollection()
    {
        var orientations = new Orientation[4];

        orientations[GameConstants.DownOrientationRotNum] = DownOrientation.Instance;
        orientations[GameConstants.LeftOrientationRotNum] = LeftOrientation.Instance;
        orientations[GameConstants.UpOrientationRotNum] = UpOrientation.Instance;
        orientations[GameConstants.RightOrientationRotNum] = RightOrientation.Instance;

        orientations.ValidateUniqueIds();

        return orientations;
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    int IIdEquatable<Orientation>.Id => RotNum;
    public abstract int RotNum { get; }
    public abstract int AbsoluteHorizontalComponent { get; }
    public abstract int AbsoluteVerticalComponent { get; }

    [Pure]
    public abstract LevelPosition TopLeftCornerOfLevel();
    [Pure]
    public abstract LevelPosition TopRightCornerOfLevel();
    [Pure]
    public abstract LevelPosition BottomLeftCornerOfLevel();
    [Pure]
    public abstract LevelPosition BottomRightCornerOfLevel();

    [Pure]
    public abstract LevelPosition MoveRight(LevelPosition position, int step);
    [Pure]
    public abstract LevelPosition MoveUp(LevelPosition position, int step);
    [Pure]
    public abstract LevelPosition MoveLeft(LevelPosition position, int step);
    [Pure]
    public abstract LevelPosition MoveDown(LevelPosition position, int step);

    /// <summary>
    /// Note: For the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    [Pure]
    public abstract LevelPosition Move(LevelPosition position, LevelPosition relativeDirection);
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public abstract LevelPosition Move(LevelPosition position, int dx, int dy);

    [Pure]
    public abstract bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition);
    [Pure]
    public abstract bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition);
    [Pure]
    public abstract bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    [Pure]
    public abstract bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    [Pure]
    public abstract bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition);
    [Pure]
    public abstract bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition);

    [Pure]
    public bool IsParallelTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteVerticalComponent == 0);
    [Pure]
    public bool IsPerpendicularTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteHorizontalComponent == 0);

    [Pure]
    public abstract Orientation RotateClockwise();
    [Pure]
    public abstract Orientation RotateCounterClockwise();
    [Pure]
    public abstract Orientation GetOpposite();

    public bool Equals(Orientation? other) => RotNum == (other?.RotNum ?? -1);
    public sealed override bool Equals(object? obj) => obj is Orientation other && RotNum == other.RotNum;
    public sealed override int GetHashCode() => RotNum;
    public abstract override string ToString();

    public static bool operator ==(Orientation left, Orientation right) => left.RotNum == right.RotNum;
    public static bool operator !=(Orientation left, Orientation right) => left.RotNum != right.RotNum;
}