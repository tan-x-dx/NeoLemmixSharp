using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Orientations;

public static class OrientationMethods
{
    /*
     * Note for the potentially confusing usages of Sine/Cosine below:
     * 
     * In this class we utilise the concepts of rotation matrices from linear algebra,
     * but restricted to integer multiples of pi/2. In these cases, the Sine/Cosine
     * functions evaluate to 0 or +-1, thus we can do some simple int based calculations
     * to get the results we desire. As a result, in these methods there are no floating
     * point calculations, nor any explicit matrix/vector types. Instead, these equivalent
     * operations are hardcoded into the individual methods.
     * 
     * The main source of confusion will probably come from the fact that the rotation
     * matrix multiplication is the wrong way round.
     * 
     * This is a consequence of how orientations are implemented inside this engine.
     * First, the "zero degree" angle corresponds to the Down orientation, instead of
     * what we'd call the Right orientation.
     * Secondly, the ids of the orientations increment in a clockwise manner, as
     * opposed to the standard counter-clockwise manner that we expect.
     * 
     * The calculations are correct, and are implemented in such a way as to simplify
     * stuff.
     */

    /// <summary>
    /// Computes the Sine of an angle, interpreting the input as an integer multiple of pi/2 radians.
    /// This method maps <see langword="int" />s to <see langword="int" />s, and avoids any floating point calculations.
    /// This function effectively maps (theta mod 4) -> [0, 1, 0, -1]
    /// </summary>
    /// <param name="theta">The angle as a multiple of pi/2 radians.</param>
    /// <returns>The Sine of that angle, as an <see langword="int" />.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IntSin(int theta)
    {
        ReadOnlySpan<int> Values = [0, 1, 0, -1];
        return Values[theta & 3];
    }

    /// <summary>
    /// Computes the Cosine of an angle, interpreting the input as an integer multiple of pi/2 radians.
    /// This method maps <see langword="int" />s to <see langword="int" />s, and avoids any floating point calculations.
    /// This function effectively maps (theta mod 4) -> [1, 0, -1, 0]
    /// </summary>
    /// <param name="theta">The angle as a multiple of pi/2 radians.</param>
    /// <returns>The Cosine of that angle, as an <see langword="int" />.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IntCos(int theta) => IntSin(theta + 1);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Point MoveRelativeToOrientation(
        Orientation orientation,
        int relativeOrientationRotNum,
        Point position,
        int step)
    {
        var s = IntSin(orientation.RotNum + relativeOrientationRotNum);
        var c = IntCos(orientation.RotNum + relativeOrientationRotNum);

        var absoluteDx = s * step;
        var absoluteDy = c * step;

        var newPosition = new Point(position.X - absoluteDx, position.Y + absoluteDy);
        return LevelScreen.NormalisePosition(newPosition);
    }

    [Pure]
    public static Point MoveDown(this Orientation orientation, Point position, int step) => MoveRelativeToOrientation(orientation, EngineConstants.DownOrientationRotNum, position, step);
    [Pure]
    public static Point MoveLeft(this Orientation orientation, Point position, int step) => MoveRelativeToOrientation(orientation, EngineConstants.LeftOrientationRotNum, position, step);
    [Pure]
    public static Point MoveUp(this Orientation orientation, Point position, int step) => MoveRelativeToOrientation(orientation, EngineConstants.UpOrientationRotNum, position, step);
    [Pure]
    public static Point MoveRight(this Orientation orientation, Point position, int step) => MoveRelativeToOrientation(orientation, EngineConstants.RightOrientationRotNum, position, step);

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="orientation">The relative orientation to use.</param>
    /// <param name="position">The position to translate.</param>
    /// <param name="dx">The horizontal translation component, relative to the orientation parameter.</param>
    /// <param name="dy">The vertical translation component, relative to the orientation parameter.</param>
    /// <returns>The translated position, relative to the orientation.</returns>
    [Pure]
    public static Point Move(
        this Orientation orientation,
        Point position,
        int dx,
        int dy)
    {
        var newPosition = orientation.MoveWithoutNormalization(position, dx, dy);
        return LevelScreen.NormalisePosition(newPosition);
    }

    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="orientation">The relative orientation to use.</param>
    /// <param name="position">The position to translate.</param>
    /// <param name="dx">The horizontal translation component, relative to the orientation parameter.</param>
    /// <param name="dy">The vertical translation component, relative to the orientation parameter.</param>
    /// <returns>The translated position, relative to the orientation.</returns>
    [Pure]
    public static Point MoveWithoutNormalization(
        this Orientation orientation,
        Point position,
        int dx,
        int dy)
    {
        var s = IntSin(orientation.RotNum);
        var c = IntCos(orientation.RotNum);

        var absoluteDx = (c * dx) + (s * dy);
        var absoluteDy = (s * dx) - (c * dy);

        return new Point(position.X + absoluteDx, position.Y + absoluteDy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EvaluateVerticalPositions(
        Orientation orientation,
        Point firstPosition,
        Point secondPosition,
        out int a0,
        out int a1)
    {
        var s = IntSin(orientation.RotNum);
        var c = IntCos(orientation.RotNum);

        a0 = (c * firstPosition.Y) - (s * firstPosition.X);
        a1 = (c * secondPosition.Y) - (s * secondPosition.X);
    }

    [Pure]
    public static bool MatchesVertically(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateVerticalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 == a1;
    }

    [Pure]
    public static bool FirstIsAboveSecond(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateVerticalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 < a1;
    }

    [Pure]
    public static bool FirstIsBelowSecond(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateVerticalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 > a1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EvaluateHorizontalPositions(
        Orientation orientation,
        Point firstPosition,
        Point secondPosition,
        out int a0,
        out int a1)
    {
        var s = IntSin(orientation.RotNum);
        var c = IntCos(orientation.RotNum);

        a0 = (c * firstPosition.X) + (s * firstPosition.Y);
        a1 = (c * secondPosition.X) + (s * secondPosition.Y);
    }

    [Pure]
    public static bool MatchesHorizontally(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateHorizontalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 == a1;
    }

    [Pure]
    public static bool FirstIsToLeftOfSecond(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateHorizontalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 < a1;
    }

    [Pure]
    public static bool FirstIsToRightOfSecond(
        this Orientation orientation,
        Point firstPosition,
        Point secondPosition)
    {
        EvaluateHorizontalPositions(orientation, firstPosition, secondPosition, out var a0, out var a1);

        return a0 > a1;
    }

    /// <summary>
    /// If the first position were to move horizontally to be in line with the second position, what is the dx it would require?
    /// </summary>
    /// <param name="orientation"></param>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public static int GetHorizontalDelta(
        this Orientation orientation,
        Point fromPosition,
        Point toPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetHorizontalDelta(fromPosition, toPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<int>(orientation)
        };

    /// <summary>
    /// If the first position were to move vertically to be in line with the second position, what is the dy it would require?
    /// </summary>
    /// <param name="orientation"></param>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    [Pure]
    public static int GetVerticalDelta(
        this Orientation orientation,
        Point fromPosition,
        Point toPosition) => orientation.RotNum switch
        {
            EngineConstants.DownOrientationRotNum => DownOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.LeftOrientationRotNum => LeftOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.UpOrientationRotNum => UpOrientationMethods.GetVerticalDelta(fromPosition, toPosition),
            EngineConstants.RightOrientationRotNum => RightOrientationMethods.GetVerticalDelta(fromPosition, toPosition),

            _ => Orientation.ThrowOrientationOutOfRangeException<int>(orientation)
        };
}
