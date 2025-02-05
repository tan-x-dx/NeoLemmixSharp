using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public readonly struct Orientation : IExtendedEnumType<Orientation>
{
    public static readonly Orientation Down = new(EngineConstants.DownOrientationRotNum);
    public static readonly Orientation Left = new(EngineConstants.LeftOrientationRotNum);
    public static readonly Orientation Up = new(EngineConstants.UpOrientationRotNum);
    public static readonly Orientation Right = new(EngineConstants.RightOrientationRotNum);

    public static int NumberOfItems => EngineConstants.NumberOfOrientations;
    private static ReadOnlySpan<int> RawInts =>
    [
        EngineConstants.DownOrientationRotNum,
        EngineConstants.LeftOrientationRotNum,
        EngineConstants.UpOrientationRotNum,
        EngineConstants.RightOrientationRotNum
    ];
    public static ReadOnlySpan<Orientation> AllItems => MemoryMarshal.Cast<int, Orientation>(RawInts);

    public readonly int RotNum;

    public Orientation(int rotNum)
    {
        RotNum = rotNum & 3;
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
    public LevelPosition MoveWithoutNormalization(LevelPosition position, int dx, int dy) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.Move(position, dx, dy),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.Move(position, dx, dy),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.Move(position, dx, dy),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.Move(position, dx, dy),

        _ => position
    };

    [Pure]
    public bool MatchesHorizontally(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesHorizontally(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    public bool MatchesVertically(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.MatchesVertically(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.MatchesVertically(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.MatchesVertically(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.MatchesVertically(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    public bool FirstIsAboveSecond(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsAboveSecond(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    public bool FirstIsBelowSecond(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsBelowSecond(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    public bool FirstIsToLeftOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToLeftOfSecond(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    public bool FirstIsToRightOfSecond(LevelPosition firstPosition, LevelPosition secondPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.FirstIsToRightOfSecond(firstPosition, secondPosition),

        _ => false
    };

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsParallelTo(Orientation other) => ((RotNum ^ other.RotNum ^ 1) & 1) != 0;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPerpendicularTo(Orientation other) => ((RotNum ^ other.RotNum) & 1) != 0;

    /// <summary>
    /// If the first position were to move horizontally to be in line with the second position, what is the dx it would require?
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public int GetHorizontalDelta(LevelPosition fromPosition, LevelPosition toPosition) => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
        EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
        EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
        EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),

        _ => 0
    };

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
    public Orientation RotateClockwise() => new(RotNum + 1);
    [Pure]
    public Orientation GetOpposite() => new(RotNum + 2);
    [Pure]
    public Orientation RotateCounterClockwise() => new(RotNum + 3);
    [Pure]
    public Orientation Rotate(int clockwiseRotationOffset) => new(RotNum + clockwiseRotationOffset);

    int IIdEquatable<Orientation>.Id => RotNum;

    public bool Equals(Orientation other) => RotNum == other.RotNum;
    public override bool Equals(object? obj) => obj is Orientation other && RotNum == other.RotNum;
    public override int GetHashCode() => RotNum;
    public override string ToString() => RotNum switch
    {
        EngineConstants.DownOrientationRotNum => EngineConstants.DownOrientationName,
        EngineConstants.LeftOrientationRotNum => EngineConstants.LeftOrientationName,
        EngineConstants.UpOrientationRotNum => EngineConstants.UpOrientationName,
        EngineConstants.RightOrientationRotNum => EngineConstants.RightOrientationName,

        _ => string.Empty
    };

    public static bool operator ==(Orientation first, Orientation second) => first.RotNum == second.RotNum;
    public static bool operator !=(Orientation first, Orientation second) => first.RotNum != second.RotNum;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OrientationSet CreateBitArraySet(bool fullSet = false) => new(new OrientationHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<OrientationHasher, BitBuffer32, Orientation, TValue> CreateBitArrayDictionary<TValue>() => new(new OrientationHasher());

    public readonly struct OrientationHasher : IPerfectHasher<Orientation>, IBitBufferCreator<BitBuffer32>
    {
        public int NumberOfItems => EngineConstants.NumberOfOrientations;

        [Pure]
        public int Hash(Orientation item) => item.RotNum;
        [Pure]
        public Orientation UnHash(int index) => new(index);

        public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
    }
}
