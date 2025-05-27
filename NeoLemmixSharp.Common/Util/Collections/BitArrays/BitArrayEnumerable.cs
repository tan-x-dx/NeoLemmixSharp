using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public readonly ref struct BitArrayEnumerable<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    public static BitArrayEnumerable<TPerfectHasher, T> Empty => default;

    private readonly ReadOnlySpan<uint> _bits;
    public readonly int Count;
    private readonly TPerfectHasher _hasher;

    internal BitArrayEnumerable(TPerfectHasher hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        Count = count;
    }

    public BitArrayEnumerable(TPerfectHasher hasher, ReadOnlySpan<uint> bits)
    {
        AssertSpanLengthIsAppropriateForHasher(hasher, bits);

        _hasher = hasher;
        _bits = bits;
        Count = BitArrayHelpers.GetPopCount(bits);
    }

    private static void AssertSpanLengthIsAppropriateForHasher(TPerfectHasher hasher, ReadOnlySpan<uint> bits)
    {
        var numberOfItems = hasher.NumberOfItems;
        var expectedSpanLength = BitArrayHelpers.CalculateBitArrayBufferLength(numberOfItems);

        if (bits.Length == expectedSpanLength)
            return;

        throw new ArgumentException($"Invalid length for span. The hasher ({typeof(TPerfectHasher).Name}) expects a span length of {expectedSpanLength}. Actually was {bits.Length}.");
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator<TPerfectHasher, T> GetEnumerator() => new(_hasher, _bits, Count);
}