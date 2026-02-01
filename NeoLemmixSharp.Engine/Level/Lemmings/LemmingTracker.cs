using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingTracker
{
    private const ulong BigMask = 0xaaaa_aaaa_aaaa_aaaaUL;

    private readonly LemmingManager _lemmingManager;
    private readonly ulong* _bits;

    public LemmingTracker(LemmingManager lemmingManager, ref nint bitsHandle)
    {
        _lemmingManager = lemmingManager;
        _bits = (ulong*)bitsHandle;

        var numberOfBytes = BitArrayHelpers.CalculateBitArrayBufferLength(lemmingManager.NumberOfLemmings) * sizeof(ulong);
        bitsHandle += numberOfBytes;
    }

    public void Tick()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmingManager.NumberOfLemmings);
        ulong* startPointer = _bits;
        ulong* endPointer = _bits + arrayLength;

        while (startPointer < endPointer)
        {
            var value = *startPointer;
            value <<= 1;
            value &= BigMask;
            *startPointer = value;
            startPointer++;
        }
    }

    public TrackingStatus UpdateLemmingTrackingStatus(Lemming lemming)
    {
        var id = _lemmingManager.HashLemming(lemming);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        ulong* longPointer = _bits + longIndex;
        *longPointer |= 1UL << bitIndex;

        var result = (int)(*longPointer >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    [Pure]
    public TrackingStatus QueryLemmingTrackingStatus(Lemming lemming)
    {
        var id = _lemmingManager.HashLemming(lemming);

        var bitIndex = (id & BitArrayHelpers.Mask) << 1;
        var longIndex = id >>> BitArrayHelpers.Shift;

        ulong* longPointer = _bits + longIndex;

        var result = (int)(*longPointer >>> bitIndex);
        return (TrackingStatus)(result & 3);
    }

    public void Clear()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmingManager.NumberOfLemmings);

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
