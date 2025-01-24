using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public sealed class Orientation : IExtendedEnumType<Orientation>
{
    public static readonly Orientation Down = new(EngineConstants.DownOrientationRotNum, EngineConstants.DownOrientationName, 0, 1);
    public static readonly Orientation Left = new(EngineConstants.LeftOrientationRotNum, EngineConstants.LeftOrientationName, -1, 0);
    public static readonly Orientation Up = new(EngineConstants.UpOrientationRotNum, EngineConstants.UpOrientationName, 0, -1);
    public static readonly Orientation Right = new(EngineConstants.RightOrientationRotNum, EngineConstants.RightOrientationName, 1, 0);

    private static readonly Orientation[] Orientations = GenerateOrientationCollection();

    public static int NumberOfItems => Orientations.Length;
    public static ReadOnlySpan<Orientation> AllItems => new(Orientations);

    private static Orientation[] GenerateOrientationCollection()
    {
        var orientations = new Orientation[4];

        orientations[EngineConstants.DownOrientationRotNum] = Down;
        orientations[EngineConstants.LeftOrientationRotNum] = Left;
        orientations[EngineConstants.UpOrientationRotNum] = Up;
        orientations[EngineConstants.RightOrientationRotNum] = Right;

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Orientation>(orientations));

        return orientations;
    }

    int IIdEquatable<Orientation>.Id => RotNum;
    private readonly string _orientationName;
    public readonly int RotNum;
    private readonly int _absoluteHorizontalComponent;
    private readonly int _absoluteVerticalComponent;

    private Orientation(
        int rotNum,
        string orientationName,
        int absoluteHorizontalComponent,
        int absoluteVerticalComponent)
    {
        RotNum = rotNum;
        _absoluteHorizontalComponent = absoluteHorizontalComponent;
        _absoluteVerticalComponent = absoluteVerticalComponent;
        _orientationName = orientationName;
    }

    [Pure]
    public LevelPosition MoveDown(LevelPosition position, int step)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveDown(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveDown(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveDown(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveDown(position, step),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public LevelPosition MoveLeft(LevelPosition position, int step)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveLeft(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveLeft(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveLeft(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveLeft(position, step),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public LevelPosition MoveUp(LevelPosition position, int step)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveUp(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveUp(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveUp(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveUp(position, step),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public LevelPosition MoveRight(LevelPosition position, int step)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MoveRight(position, step),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MoveRight(position, step),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MoveRight(position, step),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MoveRight(position, step),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: For the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    [Pure]
    public LevelPosition Move(LevelPosition position, LevelPosition relativeDirection)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, relativeDirection),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, relativeDirection),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, relativeDirection),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, relativeDirection),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public LevelPosition Move(LevelPosition position, int dx, int dy)
    {
        var newPosition = RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, dx, dy),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, dx, dy),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, dx, dy),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, dx, dy),

            _ => position
        };

        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    [Pure]
    public LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, dx, dy),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, dx, dy),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, dx, dy),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, dx, dy),

            _ => position
        };
    }

    [Pure]
    public bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    public bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesVertically(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesVertically(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    public bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    public bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    public bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    public bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),

            _ => false
        };
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallelTo(Orientation other) => (_absoluteVerticalComponent == 0) == (other._absoluteVerticalComponent == 0);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPerpendicularTo(Orientation other) => (_absoluteVerticalComponent == 0) == (other._absoluteHorizontalComponent == 0);

    /// <summary>
    /// If the first position were to move horizontally to be in line with the second position, what is the dx it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),

            _ => 0
        };
    }

    /// <summary>
    /// If the first position were to move vertically to be in line with the second position, what is the dy it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public int GetVerticalDelta(LevelPosition fromPosition, LevelPosition toPosition)
    {
        return RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetVerticalDelta(fromPosition, toPosition),

            _ => 0
        };
    }

    [Pure]
    public Orientation RotateClockwise() => Orientations[(RotNum + 1) & 3];
    [Pure]
    public Orientation RotateCounterClockwise() => Orientations[(RotNum + 3) & 3];
    [Pure]
    public Orientation GetOpposite() => Orientations[(RotNum + 2) & 3];
    [Pure]
    public Orientation Rotate(int clockwiseRotationOffset) => Orientations[(RotNum + clockwiseRotationOffset) & 3];

    public bool Equals(Orientation? other) => RotNum == (other?.RotNum ?? -1);
    public sealed override bool Equals(object? obj) => obj is Orientation other && RotNum == other.RotNum;
    public sealed override int GetHashCode() => RotNum;
    public sealed override string ToString() => _orientationName;

    public static bool operator ==(Orientation left, Orientation right) => left.RotNum == right.RotNum;
    public static bool operator !=(Orientation left, Orientation right) => left.RotNum != right.RotNum;
}