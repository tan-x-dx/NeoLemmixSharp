using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class Helpers
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle CreateRectangle(Point pos, Size size) => new(pos.X, pos.Y, size.W, size.H);

    [Pure]
    public static int CountIfNotNull<T>(this T? item)
        where T : class
    {
        return item is not null ? 1 : 0;
    }

    [Pure]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum GetEnumValue<TEnum>(int rawValue, uint numberOfEnumValues)
        where TEnum : unmanaged, Enum
    {
        if ((uint)rawValue < numberOfEnumValues)
            return Unsafe.As<int, TEnum>(ref rawValue);

        return ThrowUnknownEnumValueException<TEnum, TEnum>(rawValue);
    }

    [DoesNotReturn]
    public static TReturn ThrowUnknownEnumValueException<TEnum, TReturn>(int rawValue)
        where TEnum : unmanaged, Enum
    {
        var typeName = typeof(TEnum).Name;
        throw new ArgumentOutOfRangeException(nameof(rawValue), rawValue, $"Unknown {typeName} value!");
    }
}