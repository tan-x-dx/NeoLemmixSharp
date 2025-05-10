using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public static class Helpers
{
    public const int Uint16NumberBufferLength = 5;
    public const int Uint32NumberBufferLength = 10;
    public const int Int32NumberBufferLength = Uint32NumberBufferLength + 1;

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

    public readonly ref struct FormatParameters(char openBracket, char separator, char closeBracket)
    {
        public readonly char OpenBracket = openBracket;
        public readonly char Separator = separator;
        public readonly char CloseBracket = closeBracket;
    }

    public static bool TryFormatSpan(
        ReadOnlySpan<int> source,
        Span<char> destination,
        out int charsWritten)
    {
        var formatParameters = new FormatParameters('(', ',', ')');
        return TryFormatSpan(source, destination, formatParameters, out charsWritten);
    }

    public static bool TryFormatSpan(
        ReadOnlySpan<int> source,
        Span<char> destination,
        FormatParameters formatParameters,
        out int charsWritten)
    {
        charsWritten = 0;

        if (source.Length == 0)
        {
            if (destination.Length < 2)
                return false;
            destination[charsWritten++] = formatParameters.OpenBracket;
            destination[charsWritten++] = formatParameters.CloseBracket;

            return true;
        }

        if (destination.Length < 3 + source.Length)
            return false;
        destination[charsWritten++] = formatParameters.OpenBracket;

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
            destination[charsWritten++] = formatParameters.Separator;
        }

        couldWriteInt = source[l].TryFormat(destination[charsWritten..], out di);
        charsWritten += di;
        if (!couldWriteInt)
            return false;

        if (charsWritten == destination.Length)
            return false;
        destination[charsWritten++] = formatParameters.CloseBracket;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum GetEnumValue<TEnum>(uint rawValue, uint numberOfEnumValues)
        where TEnum : unmanaged, Enum
    {
        var enumValue = Unsafe.As<uint, TEnum>(ref rawValue);
        if (rawValue < numberOfEnumValues)
            return enumValue;

        return ThrowUnknownEnumValueException<TEnum, TEnum>(enumValue);
    }

    [DoesNotReturn]
    public static TReturn ThrowUnknownEnumValueException<TEnum, TReturn>(TEnum enumValue)
        where TEnum : unmanaged, Enum
    {
        var typeName = typeof(TEnum).Name;
        throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, $"Unknown {typeName} enum value!");
    }
}