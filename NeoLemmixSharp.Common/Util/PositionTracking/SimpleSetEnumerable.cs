﻿using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public readonly ref struct SimpleSetEnumerable<T>
{
    public static SimpleSetEnumerable<T> Empty => default;

    private readonly IPerfectHasher<T> _hasher;
    private readonly ReadOnlySpan<uint> _bits;
    private readonly int _count;

    public bool IsEmpty => _count == 0;

    public SimpleSetEnumerable(IPerfectHasher<T> hasher, ReadOnlySpan<uint> bits, int count)
    {
        _hasher = hasher;
        _bits = bits;
        _count = count;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBasedEnumerator<T> GetEnumerator() => new(_hasher, _bits, _count);
}