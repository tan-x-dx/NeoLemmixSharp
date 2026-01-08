using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe RawArray AllocateBuffer<T>(int numberOfItems)
        where T : unmanaged
    {
        var bufferLengthInBytes = numberOfItems * sizeof(T);
        var result = new RawArray(bufferLengthInBytes);
        result.AsSpan().Clear();
        return result;
    }

    /// <summary>
    /// Creates a span from a pointer and a length.
    /// 
    /// Generally speaking, when creating a span, the compiler emits checks to ensure the length is valid.
    /// This method bypasses these checks. Only use this method if you can guarantee the length is valid!
    /// </summary>
    /// <typeparam name="T">The type of the span.</typeparam>
    /// <param name="p">The pointer to use.</param>
    /// <param name="length">The (assumed valid) desired length of the span.</param>
    /// <returns>A span over the desired data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> CreateSpan<T>(void* p, int length) where T : unmanaged
    {
#if DEBUG
        ArgumentOutOfRangeException.ThrowIfNegative(length);
#endif

        return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(p), length);
    }

    /// <summary>
    /// Creates a read-only span from a pointer and a length.
    /// 
    /// Generally speaking, when creating a read-only span, the compiler emits checks to ensure the length is valid.
    /// This method bypasses these checks. Only use this method if you can guarantee the length is valid!
    /// </summary>
    /// <typeparam name="T">The type of the read-only span.</typeparam>
    /// <param name="p">The pointer to use.</param>
    /// <param name="length">The (assumed valid) desired length of the read-only span.</param>
    /// <returns>A span over the desired data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(void* p, int length) where T : unmanaged
    {
#if DEBUG
        ArgumentOutOfRangeException.ThrowIfNegative(length);
#endif

        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(p), length);
    }

    /// <summary>
    /// Returns a mutable reference to the specified span index.
    /// 
    /// Generally speaking, when indexing into a span, the compiler emits checks to ensure the index is valid.
    /// This method bypasses these checks. Only use this method if you can guarantee the index is valid!
    /// </summary>
    /// <typeparam name="T">The type of the span.</typeparam>
    /// <param name="span">The span to index into.</param>
    /// <param name="index">The (assumed valid) index.</param>
    /// <returns>A mutable reference to the data at that index</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T At<T>(this Span<T> span, int index)
    {
#if DEBUG
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#endif

        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), index);
    }

    /// <summary>
    /// Returns a read-only reference to the specified read-only span index.
    /// 
    /// Generally speaking, when indexing into a read-only span, the compiler emits checks to ensure the index is valid.
    /// This method bypasses these checks. Only use this method if you can guarantee the index is valid!
    /// </summary>
    /// <typeparam name="T">The type of the read-only span.</typeparam>
    /// <param name="span">The read-only span to index into.</param>
    /// <param name="index">The (assumed valid) index.</param>
    /// <returns>A read-only reference to the data at that index</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T At<T>(this ReadOnlySpan<T> span, int index)
    {
#if DEBUG
        ArgumentOutOfRangeException.ThrowIfNegative(index);
#endif

        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), index);
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

    /// <summary>
    /// Calculates a % b, but does not return negative numbers for negative inputs.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LogicalMod(int a, int b)
    {
        var result = a % b;
        if (result < 0)
            result += b;
        return result;
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

    public static IEnumerable<T> MaybeConcat<T>(IEnumerable<T>? first, IEnumerable<T>? second)
    {
        if (first is null)
            return second ?? [];

        if (second is null)
            return first;

        return first.Concat(second);
    }

    public static bool StringSpansMatch(
        ReadOnlySpan<char> firstSpan,
        ReadOnlySpan<char> secondSpan)
    {
        return firstSpan.Equals(secondSpan, StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] GetFilePathsWithExtension(string folderPath, string requiredFileExtension)
    {
        var extensionSearch = "*" + requiredFileExtension;

        return Directory.GetFiles(folderPath, extensionSearch, SearchOption.TopDirectoryOnly);
    }

    public static ReadOnlySpan<char> GetFullFilePathWithoutExtension(ReadOnlySpan<char> fullFilePath)
    {
        var extension = Path.GetExtension(fullFilePath);

        var indexOfExtension = fullFilePath.IndexOf(extension, StringComparison.Ordinal);
        return fullFilePath[..indexOfExtension];
    }

    [DoesNotReturn]
    public static void ThrowKeyAlreadyAddedException<TKey>(TKey key) => throw new ArgumentException("Key already added!", nameof(key));
    [DoesNotReturn]
    public static void ThrowDestinationSpanTooShortException() => throw new ArgumentException("Destination span too short!");
    [DoesNotReturn]
    public static void ThrowKeyNotFoundException() => throw new KeyNotFoundException("Key not found!");
}
