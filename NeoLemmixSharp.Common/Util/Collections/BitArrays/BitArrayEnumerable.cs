using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

[DebuggerDisplay("Count = {Count}")]
public readonly ref struct BitArrayEnumerable<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    public static BitArrayEnumerable<TPerfectHasher, T> Empty => default;

    private readonly ReadOnlySpan<uint> _bits;
    public readonly int Count;
    private readonly TPerfectHasher _hasher;

    internal BitArrayEnumerable(ReadOnlySpan<uint> bits, int count, TPerfectHasher hasher)
    {
        _bits = bits;
        Count = count;
        _hasher = hasher;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayEnumerator<TPerfectHasher, T> GetEnumerator() => new(_bits, _hasher);
}
