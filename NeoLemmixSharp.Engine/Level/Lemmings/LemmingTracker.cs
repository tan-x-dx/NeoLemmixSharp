using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Numerics.Tensors;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingTracker
{
    private const int LemmingTrackerShift = 4;
    private const int LemmingTrackerBitIndexMask = (1 << LemmingTrackerShift) - 1;

    private const uint LemmingTrackerTickMask = 0xaaaa_aaaaU;

    private readonly uint* _pointer;

    public LemmingTracker(ref nint bitsHandle, int numberOfLemmings)
    {
        _pointer = (uint*)bitsHandle;

        var numberOfBytes = BitArrayHelpers.CalculateBitArrayBufferLength(numberOfLemmings) * sizeof(ulong);
        bitsHandle += numberOfBytes;
    }

    public uint Tick()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(LevelScreen.LemmingManager.NumberOfLemmings);
        uint* p = _pointer;

        switch (arrayLength)
        {
            case 8: goto Length8;
            case 7: goto Length7;
            case 6: goto Length6;
            case 5: goto Length5;
            case 4: goto Length4;
            case 3: goto Length3;
            case 2: goto Length2;
            case 1: goto Length1;
            case 0: goto Length0;

            default: ProcessLargeSpan(p, arrayLength); return 0;
        }

    Length8:
        p[7] <<= 1;
        p[7] &= LemmingTrackerTickMask;
    Length7:
        p[6] <<= 1;
        p[6] &= LemmingTrackerTickMask;
    Length6:
        p[5] <<= 1;
        p[5] &= LemmingTrackerTickMask;
    Length5:
        p[4] <<= 1;
        p[4] &= LemmingTrackerTickMask;
    Length4:
        p[3] <<= 1;
        p[3] &= LemmingTrackerTickMask;
    Length3:
        p[2] <<= 1;
        p[2] &= LemmingTrackerTickMask;
    Length2:
        p[1] <<= 1;
        p[1] &= LemmingTrackerTickMask;
    Length1:
        p[0] <<= 1;
        p[0] &= LemmingTrackerTickMask;
    Length0:
        return 0;
    }

    private static void ProcessLargeSpan(uint* p, int length)
    {
        var x = Helpers.CreateReadOnlySpan<uint>(p, length);
        var destination = Helpers.CreateSpan<uint>(p, length);

        TensorPrimitives.ShiftLeft(x, 1, destination);
        TensorPrimitives.BitwiseAnd(x, LemmingTrackerTickMask, destination);
    }

    public TrackingStatus UpdateLemmingTrackingStatus(Lemming lemming)
    {
        var id = LevelScreen.LemmingManager.HashLemming(lemming);

        var uintIndex = id >>> LemmingTrackerShift;
        uint* p = _pointer + uintIndex;

        var bitIndex = (id & LemmingTrackerBitIndexMask) << 1;
        *p |= 1U << bitIndex;

        var result = *p >>> bitIndex;
        return (TrackingStatus)(result & 3);
    }

    [Pure]
    public TrackingStatus QueryLemmingTrackingStatus(Lemming lemming)
    {
        var id = LevelScreen.LemmingManager.HashLemming(lemming);

        var uintIndex = id >>> LemmingTrackerShift;
        uint* p = _pointer + uintIndex;

        var bitIndex = (id & LemmingTrackerBitIndexMask) << 1;
        *p |= 1U << bitIndex;

        var result = *p >>> bitIndex;
        return (TrackingStatus)(result & 3);
    }

    public void Clear()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(LevelScreen.LemmingManager.NumberOfLemmings);

        Helpers.CreateSpan<byte>(_pointer, arrayLength * sizeof(ulong)).Clear();
    }
}

public enum TrackingStatus
{
    Absent = 0,
    Entered = 1,
    Exited = 2,
    StillPresent = 3
}
