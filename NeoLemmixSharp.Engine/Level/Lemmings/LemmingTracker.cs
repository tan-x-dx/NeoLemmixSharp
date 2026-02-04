using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingTracker
{
    private const int LemmingTrackerShift = 4;
    private const int LemmingTrackerBitIndexMask = (1 << LemmingTrackerShift) - 1;

    public const uint LemmingTrackerTickMask = 0xaaaa_aaaaU;
    private const uint LemmingTrackerInverseTickMask = ~LemmingTrackerTickMask;

    private readonly uint* _pointer;

    public LemmingTracker(nint bitsHandle)
    {
        _pointer = (uint*)bitsHandle;
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

        var result = *p >>> bitIndex;
        return (TrackingStatus)(result & 3);
    }

    public void Clear()
    {
        var arrayLength = BitArrayHelpers.CalculateBitArrayBufferLength(LevelScreen.LemmingManager.NumberOfLemmings);

        Helpers.CreateSpan<byte>(_pointer, arrayLength * sizeof(ulong)).Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LemmingTrackerEnumerator GetEnumerator() => new(_pointer);

    public ref struct LemmingTrackerEnumerator
    {
        private readonly ReadOnlySpan<uint> _bits;

        private int _index;
        private int _current;
        private uint _v;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LemmingTrackerEnumerator(void* pointer)
        {
            var spanLength = BitArrayHelpers.CalculateBitArrayBufferLength(LevelScreen.LemmingManager.NumberOfLemmings);

            _bits = Helpers.CreateReadOnlySpan<uint>(pointer, spanLength);
            _index = 0;
            _current = 0;
            _v = _bits.At(0) & LemmingTrackerInverseTickMask;
        }

        public bool MoveNext()
        {
            var v = _v;
            var index = _index;
            if (v == 0U)
            {
                do
                {
                    ++index;
                    if ((uint)index >= (uint)_bits.Length)
                        return false;

                    v = _bits.At(index);
                    v &= LemmingTrackerInverseTickMask;
                }
                while (v == 0U);
                _index = index;
            }

            var c = BitOperations.TrailingZeroCount(v) >> 1;
            c |= index << LemmingTrackerShift;
            _current = c;
            v &= v - 1;
            _v = v;
            return true;
        }

        public readonly Lemming Current => LevelScreen.LemmingManager.UnHashLemming(_current);
    }
}

public enum TrackingStatus
{
    Absent = 0,
    Entered = 1,
    Exited = 2,
    StillPresent = 3
}
