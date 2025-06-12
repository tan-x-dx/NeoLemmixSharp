using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class ItemTracker<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    private const ulong BigMask = 0xAAAA_AAAA_AAAA_AAAAUL;

    private readonly TPerfectHasher _hasher;
    private readonly ulong[] _bits;

    public ItemTracker(TPerfectHasher hasher)
    {
        _hasher = hasher;
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);

        _bits = Helpers.GetArrayForSize<ulong>(arrayLength);
    }

    public void Tick()
    {
        for (var i = 0; i < _bits.Length; i++)
        {
            ref var value = ref _bits[i];
            value = (value << 1) & BigMask;
        }
    }

    public TrackingStatus TrackItem(T item)
    {
        var id = _hasher.Hash(item);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        ref var longValue = ref _bits[longIndex];
        longValue |= 1UL << bitIndex;

        var result = (int)(longValue >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    [Pure]
    public TrackingStatus QueryItemStatus(T item)
    {
        var id = _hasher.Hash(item);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        var longValue = _bits[longIndex];

        var result = (int)(longValue >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        new Span<ulong>(_bits).Clear();
    }
}

public enum TrackingStatus
{
    Absent = 0,
    JustAdded = 1,
    JustRemoved = 2,
    StillPresent = 3
}