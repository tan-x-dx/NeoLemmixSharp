using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class ItemTracker<T>
    where T : class, IIdEquatable<T>
{
    private const ulong BigMask = 0xAAAA_AAAA_AAAA_AAAA;

    private const int Shift = 5;
    private const int Mask = (1 << Shift) - 1;

    private readonly ulong[] _longs;

    public ItemTracker(ISimpleHasher<T> hasher)
    {
        var length = hasher.NumberOfItems;

        var numberOfLongs = (length + Mask) >> Shift;

        _longs = new ulong[numberOfLongs];
    }

    public void Tick()
    {
        var span = new Span<ulong>(_longs);

        for (var i = 0; i < span.Length; i++)
        {
            ref var value = ref span[i];
            value = (value << 1) & BigMask;
        }
    }

    public int EvaluateItem(T item)
    {
        var id = item.Id;

        var bitIndex = (id & Mask) << 1;
        var longIndex = id >> Shift;

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