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

    /// <summary>
    /// Allocates an array of the given type and the given size, unless the <paramref name="size"/> parameter is zero. In that case a reference to Array.Empty&lt;<typeparamref name="T"/>&gt; is returned without allocating anything.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <param name="size">The required length of the array.</param>
    /// <returns>A newly allocated array reference, or a reference to Array.Empty&lt;<typeparamref name="T"/>&gt;.</returns>
    public static T[] GetArrayForSize<T>(int size)
    {
        return size == 0
            ? Array.Empty<T>()
            : new T[size];
    }

    public static unsafe RawArray AllocateBuffer<T>(int numberOfItems)
        where T : unmanaged
    {
        var bufferLengthInBytes = numberOfItems * sizeof(T);
        var result = new RawArray(bufferLengthInBytes);
        result.AsSpan().Clear();
        return result;
    }

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

    public static TSubClass? TryFindItemOfType<TBaseClass, TSubClass>(this TBaseClass[] items)
        where TBaseClass : class
        where TSubClass : class, TBaseClass
    {
        foreach (var item in items)
        {
            if (item is TSubClass result)
                return result;
        }

        return null;
    }

    public static ReadOnlySpan<T> CombineSpans<T>(ReadOnlySpan<T> firstSpan, ReadOnlySpan<T> secondSpan)
    {
        if (firstSpan.Length == 0)
            return secondSpan;
        if (secondSpan.Length == 0)
            return firstSpan;

        var newArray = new T[firstSpan.Length + secondSpan.Length];

        firstSpan.CopyTo(newArray);
        secondSpan.CopyTo(new Span<T>(newArray, firstSpan.Length, secondSpan.Length));

        return newArray;
    }

    public static bool StringSpansMatch(
        ReadOnlySpan<char> firstSpan,
        ReadOnlySpan<char> secondSpan)
    {
        return firstSpan.Equals(secondSpan, StringComparison.OrdinalIgnoreCase);
    }

    public static ReadOnlySpan<string> GetFilePathsWithExtension(string folderPath, ReadOnlySpan<char> requiredFileExtension)
    {
        var allFiles = Directory.GetFiles(folderPath);
        var subLength = 0;

        for (int i = 0; i < allFiles.Length; i++)
        {
            var file = allFiles[i];
            var fileExtension = Path.GetExtension(file.AsSpan());

            if (StringSpansMatch(requiredFileExtension, fileExtension))
            {
                allFiles[subLength++] = file;
            }
        }

        // Clear the upper indices of unused strings
        var s = new Span<string>(allFiles);
        s = s[subLength..];
        s.Clear();

        return new ReadOnlySpan<string>(allFiles, 0, subLength);
    }

    public static ReadOnlySpan<char> GetFullFilePathWithoutExtension(ReadOnlySpan<char> fullFilePath)
    {
        var extension = Path.GetExtension(fullFilePath);

        var indexOfExtension = fullFilePath.IndexOf(extension, StringComparison.Ordinal);
        return fullFilePath[..indexOfExtension];
    }
}
