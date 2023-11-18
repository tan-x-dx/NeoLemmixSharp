using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public abstract class Orientation : IExtendedEnumType<Orientation>
{
    private static readonly Orientation[] Orientations = GenerateOrientationCollection();

    public static int NumberOfItems => Orientations.Length;
    public static ReadOnlySpan<Orientation> AllItems => new(Orientations);

    private static Orientation[] GenerateOrientationCollection()
    {
        var orientations = new Orientation[4];

        orientations[LevelConstants.DownOrientationRotNum] = DownOrientation.Instance;
        orientations[LevelConstants.LeftOrientationRotNum] = LeftOrientation.Instance;
        orientations[LevelConstants.UpOrientationRotNum] = UpOrientation.Instance;
        orientations[LevelConstants.RightOrientationRotNum] = RightOrientation.Instance;

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Orientation>(orientations));

        return orientations;
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
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public abstract LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallelTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteVerticalComponent == 0);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPerpendicularTo(Orientation other) => (AbsoluteVerticalComponent == 0) == (other.AbsoluteHorizontalComponent == 0);

    /// <summary>
    /// If the first position were to move horizontally to be in line with the second position, what is the dx it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public abstract int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition);
    /// <summary>
    /// If the first position were to move vertically to be in line with the second position, what is the dy it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public abstract int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition);

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