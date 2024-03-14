using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class ItemTracker<T>
    where T : class, IIdEquatable<T>
{
    private const ulong BigMask = 0xAAAA_AAAA_AAAA_AAAAUL;

    private readonly ulong[] _longs;

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
}