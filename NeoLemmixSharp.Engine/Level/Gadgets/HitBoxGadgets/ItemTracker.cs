using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class ItemTracker<T> : IItemCountListener
    where T : class, IIdEquatable<T>
{
    private const ulong BigMask = 0xAAAA_AAAA_AAAA_AAAAUL;

    private ulong[] _longs;

    public ItemTracker(IPerfectHasher<T> hasher)
    {
        var arrayLength = (hasher.NumberOfItems + BitArray.Mask) >> BitArray.Shift;

        _longs = new ulong[arrayLength];
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
        var id = item.Id;

        var bitIndex = (id & BitArray.Mask) << 1;
        var longIndex = id >> BitArray.Shift;

        ref var longValue = ref _longs[longIndex];
        longValue |= 1UL << bitIndex;

        var result = (int)(longValue >> bitIndex);
        return result & 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Array.Clear(_longs, 0, _longs.Length);
    }

    public void OnNumberOfItemsChanged(int numberOfItems)
    {
        var newArraySize = (numberOfItems + BitArray.Mask) >> BitArray.Shift;

        if (newArraySize <= _longs.Length)
            return;

        if (_longs.Length == 0)
        {
            _longs = new ulong[newArraySize];
            return;
        }

        var newArray = new ulong[newArraySize];
        Array.Copy(_longs, newArray, _longs.Length);
        _longs = newArray;
    }
}