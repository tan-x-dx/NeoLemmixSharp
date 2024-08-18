using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class ItemTracker<T> : IItemCountListener
    where T : notnull
{
    private const ulong BigMask = 0xAAAA_AAAA_AAAA_AAAAUL;

    private readonly IPerfectHasher<T> _hasher;
    private ulong[] _longs;

    public ItemTracker(IPerfectHasher<T> hasher)
    {
        _hasher = hasher;
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);

        _longs = CollectionsHelper.GetArrayForSize<ulong>(arrayLength);
    }

    public void Tick()
    {
        var span = new Span<ulong>(_longs);

        foreach (ref var value in span)
        {
            value = (value << 1) & BigMask;
        }
    }

    public int TrackItem(T item)
    {
        var id = _hasher.Hash(item);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >> BitArrayHelpers.Shift;

        ref var longValue = ref _longs[longIndex];
        longValue |= 1UL << bitIndex;

        var result = (int)(longValue >> bitIndex);
        return result & 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        new Span<ulong>(_longs).Clear();
    }

    public void OnNumberOfItemsChanged()
    {
        var numberOfItems = _hasher.NumberOfItems;
        var newArrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(numberOfItems);

        if (newArrayLength <= _longs.Length)
            return;

        Array.Resize(ref _longs, newArrayLength);
    }
}