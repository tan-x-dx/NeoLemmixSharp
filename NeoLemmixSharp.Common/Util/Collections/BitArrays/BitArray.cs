using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public static class BitArray
{
    public const int Shift = 5;
    public const int Mask = (1 << Shift) - 1;

    [Pure]
    public static uint[] CreateBitArray(int length, bool setAllBits)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "length must be non-negative!");

        var arraySize = (length + Mask) >> Shift;
        var result = CollectionsHelper.GetArrayForSize<uint>(arraySize);

        if (!setAllBits || arraySize == 0)
            return result;

        Array.Fill(result, uint.MaxValue);

        var lastIndexPopCount = length & Mask;

        if (lastIndexPopCount != 0)
        {
            result[^1] = (1U << lastIndexPopCount) - 1U;
        }

        return result;
    }

    public static void SetLength(ref uint[] originalBits, int newLength)
    {
        var newArraySize = (newLength + Mask) >> Shift;

        if (newArraySize <= originalBits.Length)
            return;

        if (originalBits.Length == 0)
        {
            originalBits = new uint[newArraySize];
        }

        Array.Resize(ref originalBits, newArraySize);
    }

    /// <summary>
    /// Tests if a specific bit is set
    /// </summary>
    /// <param name="bits">The span to query</param>
    /// <param name="index">The bit to query</param>
    /// <returns><see langword="true" /> if the specified bit is set</returns>
    [Pure]
    public static bool GetBit(ReadOnlySpan<uint> bits, int index)
    {
        var value = bits[index >> Shift];
        value >>= index;
        return (value & 1U) != 0U;
    }

    /// <summary>
    /// Sets a bit to 1. Returns <see langword="true" /> if a change has occurred -
    /// i.e. if the bit was previously 0
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to set</param>
    /// <param name="popCount">Will be incremented if the operation changes the contents of the span</param>
    /// <returns><see langword="true" /> if the operation changed the value of the bit, <see langword="false" /> if the bit was previously set</returns>
    public static bool SetBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;
        var delta = (arrayValue ^ oldValue) >> index;
        popCount += (int)delta;
        return delta != 0U;
    }

    /// <summary>
    /// Sets a bit to 1
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to set</param>
    public static void SetBit(Span<uint> bits, int index)
    {
        bits[index >> Shift] |= (1U << index);
    }

    /// <summary>
    /// Sets a bit to 0. Returns <see langword="true" /> if a change has occurred -
    /// i.e. if the bit was previously 1
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to clear</param>v
    /// <param name="popCount">Will be decremented if the operation changes the contents of the span</param>
    /// <returns><see langword="true" /> if the operation changed the value of the bit, <see langword="false" /> if the bit was previously clear</returns>
    public static bool ClearBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);
        var delta = (arrayValue ^ oldValue) >> index;
        popCount -= (int)delta;
        return delta != 0U;
    }

    /// <summary>
    /// Sets a bit to 0. 
    /// </summary>
    /// <param name="bits">The bit to clear</param>
    /// <param name="index">The bit to clear</param>
    public static void ClearBit(Span<uint> bits, int index)
    {
        bits[index >> Shift] &= ~(1U << index);
    }

    /// <summary>
    /// Toggles the value of a bit. Returns the new value after toggling
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to clear</param>v
    /// <param name="popCount">Will be modified accordingly if the operation changes the contents of the span</param>
    /// <returns>The bool equivalent of the binary value (0 or 1) of the bit after toggling</returns>
    public static bool ToggleBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;
        var result = arrayValue > oldValue;
        var delta = result ? 1 : -1;
        popCount += delta;
        return result;
    }

    public sealed class ReferenceTypeBitEnumerator : IEnumerator<int>
    {
        private readonly uint[] _bits;

        private uint _v;
        private int _remaining;
        private int _index;

        public int Current { get; private set; }

        public ReferenceTypeBitEnumerator(uint[] bits, int popCount)
        {
            _bits = bits;
            _index = 0;
            _v = _bits.Length == 0 ? 0U : _bits[0];
            _remaining = popCount;
            Current = 0;
        }

        public bool MoveNext()
        {
            if (_v == 0U)
            {
                if (_remaining == 0)
                    return false;

                do
                {
                    _v = _bits[++_index];
                }
                while (_v == 0U);
            }

            var m = BitOperations.TrailingZeroCount(_v);
            _v &= _v - 1;

            Current = (_index << Shift) | m;
            _remaining--;
            return true;
        }

        void IEnumerator.Reset() => throw new InvalidOperationException("Cannot reset");
        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }

    [Pure]
    internal static int GetPopCount(ReadOnlySpan<uint> bits)
    {
        var result = 0;
        foreach (var v in bits)
        {
            result += BitOperations.PopCount(v);
        }

        return result;
    }

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other, ref int popCount)
    {
        var newCount = 0;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var v = ref span[i];
            v |= other[i];
            newCount += BitOperations.PopCount(v);
        }
        popCount = newCount;
    }

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            span[i] |= other[i];
        }
    }

    internal static void IntersectWith(Span<uint> span, ReadOnlySpan<uint> other, ref int popCount)
    {
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref span[i];
            arrayValue &= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        popCount = count;
    }

    internal static void ExceptWith(Span<uint> span, ReadOnlySpan<uint> other, ref int popCount)
    {
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref span[i];
            arrayValue &= ~other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        popCount = count;
    }

    internal static void SymmetricExceptWith(Span<uint> span, ReadOnlySpan<uint> other, ref int popCount)
    {
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref span[i];
            arrayValue ^= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        popCount = count;
    }

    [Pure]
    internal static bool IsSubsetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            var otherBits = other[i];
            if ((span[i] | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal static bool IsSupersetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            var bits = span[i];
            if ((bits | other[i]) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal static bool IsProperSubsetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        var allEqual = true;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            var bits = span[i];
            var otherBits = other[i];
            allEqual &= bits == otherBits;

            if ((bits | otherBits) != otherBits)
                return false;
        }

        return !allEqual;
    }

    [Pure]
    internal static bool IsProperSupersetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        var allEqual = true;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            var bits = span[i];
            var otherBits = other[i];
            allEqual &= bits == otherBits;

            if ((bits | otherBits) != bits)
                return false;
        }

        return !allEqual;
    }

    [Pure]
    internal static bool Overlaps(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            if ((span[i] & other[i]) != 0U)
                return true;
        }

        return false;
    }

    [Pure]
    internal static bool SetEquals(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            if (span[i] != other[i])
                return false;
        }

        return true;
    }
}