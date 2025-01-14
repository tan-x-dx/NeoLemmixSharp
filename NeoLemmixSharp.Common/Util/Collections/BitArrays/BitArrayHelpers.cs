using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public static class BitArrayHelpers
{
    public const int Shift = 5;
    public const int Mask = (1 << Shift) - 1;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int CalculateBitArrayBufferLength(int length)
    {
        return (length + Mask) >> Shift;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ToNextLargestMultipleOf32(int a)
    {
        return ((a + Mask) >> Shift) << Shift;
    }

    [Pure]
    public static uint[] CreateBitArray(int length, bool setAllBits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var arrayLength = CalculateBitArrayBufferLength(length);
        if (arrayLength == 0)
            return Array.Empty<uint>();

        var result = new uint[arrayLength];
        if (!setAllBits)
            return result;

        new Span<uint>(result).Fill(uint.MaxValue);
        var lastIndexPopCount = length & Mask;
        if (lastIndexPopCount == 0)
            return result;

        result[^1] = (1U << lastIndexPopCount) - 1U;
        return result;
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
    internal static bool SetBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;
        var delta = (arrayValue ^ oldValue) >> index;
        popCount += (int)delta;
        return (delta & 1U) != 0U;
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
    internal static bool ClearBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);
        var delta = (arrayValue ^ oldValue) >> index;
        popCount -= (int)delta;
        return (delta & 1U) != 0U;
    }

    /// <summary>
    /// Sets a bit to 0. 
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to clear</param>
    public static void ClearBit(Span<uint> bits, int index)
    {
        bits[index >> Shift] &= ~(1U << index);
    }

    /// <summary>
    /// Toggles the value of a bit. Returns the new value after toggling
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to modify</param>v
    /// <param name="popCount">Will be modified accordingly if the operation changes the contents of the span</param>
    /// <returns>The bool equivalent of the binary value (0 or 1) of the bit after toggling</returns>
    internal static bool ToggleBit(Span<uint> bits, int index, ref int popCount)
    {
        ref var arrayValue = ref bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;
        var result = arrayValue > oldValue;
        var delta = result ? 1 : -1;
        popCount += delta;
        return result;
    }

    [Pure]
    internal static int GetPopCount(ReadOnlySpan<uint> bits)
    {
        // Basic implementation is faster than using TensorPrimitives - benchmarks

        var result = 0;
        for (int i = 0; i < bits.Length; i++)
        {
            result += BitOperations.PopCount(bits[i]);
        }

        return result;
    }

    /*
     * Use jump tables (switch/case) for small span lengths.
     * This is a more common occurrence than not, and is faster - benchmarks.
     * The TensorPrimitives library is best used for larger spans.
     */

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        switch (span.Length)
        {
            case 7: span[6] |= other[6]; goto case 6;
            case 6: span[5] |= other[5]; goto case 5;
            case 5: span[4] |= other[4]; goto case 4;
            case 4: span[3] |= other[3]; goto case 3;
            case 3: span[2] |= other[2]; goto case 2;
            case 2: span[1] |= other[1]; goto case 1;
            case 1: span[0] |= other[0]; return;
            case 0: return;
        }

        TensorPrimitives.BitwiseOr(span, other, span);
    }

    internal static void IntersectWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        switch (span.Length)
        {
            case 7: span[6] |= other[6]; goto case 6;
            case 6: span[5] |= other[5]; goto case 5;
            case 5: span[4] |= other[4]; goto case 4;
            case 4: span[3] |= other[3]; goto case 3;
            case 3: span[2] |= other[2]; goto case 2;
            case 2: span[1] |= other[1]; goto case 1;
            case 1: span[0] |= other[0]; return;
            case 0: return;
        }

        TensorPrimitives.BitwiseAnd(span, other, span);
    }

    internal static void ExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        switch (span.Length)
        {
            case 7: span[6] &= ~other[6]; goto case 6;
            case 6: span[5] &= ~other[5]; goto case 5;
            case 5: span[4] &= ~other[4]; goto case 4;
            case 4: span[3] &= ~other[3]; goto case 3;
            case 3: span[2] &= ~other[2]; goto case 2;
            case 2: span[1] &= ~other[1]; goto case 1;
            case 1: span[0] &= ~other[0]; return;
            case 0: return;
        }

        for (var i = 0; i < span.Length; i++)
        {
            span[i] &= ~other[i];
        }
    }

    internal static void SymmetricExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        switch (span.Length)
        {
            case 7: span[6] ^= other[6]; goto case 6;
            case 6: span[5] ^= other[5]; goto case 5;
            case 5: span[4] ^= other[4]; goto case 4;
            case 4: span[3] ^= other[3]; goto case 3;
            case 3: span[2] ^= other[2]; goto case 2;
            case 2: span[1] ^= other[1]; goto case 1;
            case 1: span[0] ^= other[0]; return;
            case 0: return;
        }

        TensorPrimitives.Xor(span, other, span);
    }

    [Pure]
    internal static bool IsSubsetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        for (var i = 0; i < span.Length; i++)
        {
            var otherBits = other[i];
            if ((span[i] | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal static bool IsProperSubsetOf(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        var allEqual = true;
        for (var i = 0; i < span.Length; i++)
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
    internal static bool Overlaps(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        for (var i = 0; i < span.Length; i++)
        {
            if ((span[i] & other[i]) != 0U)
                return true;
        }

        return false;
    }

    [Pure]
    internal static bool SetEquals(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            throw new ArgumentException("Spans have different lengths!");

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] != other[i])
                return false;
        }

        return true;
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
}