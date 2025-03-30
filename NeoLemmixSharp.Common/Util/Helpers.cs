using Microsoft.Xna.Framework;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class Helpers
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle CreateRectangle(LevelPosition pos, LevelSize size) => new(pos.X, pos.Y, size.W, size.H);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition TopLeftLevelPosition(this Rectangle rectangle) => new(rectangle.X, rectangle.Y);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition BottomLeftLevelPosition(this Rectangle rectangle) => new(rectangle.X, rectangle.Y + rectangle.Height - 1);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition TopRightLevelPosition(this Rectangle rectangle) => new(rectangle.X + rectangle.Width - 1, rectangle.Y);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition BottomRightLevelPosition(this Rectangle rectangle) => new(rectangle.X + rectangle.Width - 1, rectangle.Y + rectangle.Height - 1);

    public static int CountIfNotNull<T>(this T? item)
        where T : class
    {
        return item is not null ? 1 : 0;
    }

    public static int CountIfNotNull<T>(this T? item)
        where T : struct
    {
        return item.HasValue ? 1 : 0;
    }

    internal static bool TryFormatSpan(ReadOnlySpan<int> source, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        if (source.Length == 0)
        {
            if (destination.Length < 2)
                return false;
            destination[charsWritten++] = '(';
            destination[charsWritten++] = ')';

            return true;
        }

        if (destination.Length < 3 + source.Length)
            return false;
        destination[charsWritten++] = '(';

        var l = source.Length - 1;
        bool couldWriteInt;
        int di;
        for (var j = 0; j < l; j++)
        {
            couldWriteInt = source[j].TryFormat(destination[charsWritten..], out di);
            charsWritten += di;
            if (!couldWriteInt)
                return false;

            if (charsWritten == destination.Length)
                return false;
            destination[charsWritten++] = ',';
        }

        couldWriteInt = source[l].TryFormat(destination[charsWritten..], out di);
        charsWritten += di;
        if (!couldWriteInt)
            return false;

        if (charsWritten == destination.Length)
            return false;
        destination[charsWritten++] = ')';

        return true;
    }
}