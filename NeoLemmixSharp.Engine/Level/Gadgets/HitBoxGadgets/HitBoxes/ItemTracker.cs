using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public unsafe sealed class ItemTracker<TPerfectHasher, T>
    where TPerfectHasher : IPerfectHasher<T>
    where T : notnull
{
    private const ulong BigMask = 0xaaaa_aaaa_aaaa_aaaaUL;

    private readonly TPerfectHasher _hasher;
    private readonly ulong* _bits;

    public ItemTracker(TPerfectHasher hasher, nint bitsHandle)
    {
        _hasher = hasher;
        _bits = (ulong*)bitsHandle;
    }

    public void Tick()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);
        ulong* startPointer = _bits;
        ulong* endPointer = _bits + arrayLength;

        while (startPointer != endPointer)
        {
            var value = *startPointer;
            value <<= 1;
            value &= BigMask;
            *startPointer = value;
            startPointer++;
        }
    }

    public TrackingStatus TrackItem(T item)
    {
        var id = _hasher.Hash(item);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        ulong* longPointer = _bits + longIndex;
        *longPointer |= 1UL << bitIndex;

        var result = (int)(*longPointer >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    [Pure]
    public TrackingStatus QueryItemStatus(T item)
    {
        var id = _hasher.Hash(item);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        ulong* longPointer = _bits + longIndex;

        var result = (int)(*longPointer >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    public void Clear()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_hasher.NumberOfItems);

        Helpers.CreateSpan<ulong>(_bits, arrayLength).Clear();
    }
}

public enum TrackingStatus
{
    Absent = 0,
    Entered = 1,
    Exited = 2,
    StillPresent = 3
}
