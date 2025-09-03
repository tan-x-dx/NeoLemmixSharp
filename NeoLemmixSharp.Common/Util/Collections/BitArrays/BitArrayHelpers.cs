using System.Diagnostics.CodeAnalysis;
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
    public static int CalculateBitArrayBufferLength(int length) => (length + Mask) >>> Shift;

    [Pure]
    public static uint[] CreateBitArray(int length, bool setAllBits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var arrayLength = CalculateBitArrayBufferLength(length);
        if (arrayLength == 0)
            return Array.Empty<uint>();

        var result = new uint[arrayLength];
        if (setAllBits)
            PopulateBitArray(result, length);

        return result;
    }

    internal static void PopulateBitArray(Span<uint> bitArray, int requiredPopCount)
    {
        ThrowIfInvalidCapacity(requiredPopCount, bitArray.Length);

        var requiredSpanLength = CalculateBitArrayBufferLength(requiredPopCount);

        var subSpan = bitArray[requiredSpanLength..];
        subSpan.Clear();
        subSpan = bitArray[..requiredSpanLength];
        subSpan.Fill(uint.MaxValue);

        var lastIndexPopCount = requiredPopCount & Mask;
        if (lastIndexPopCount != 0)
            subSpan[^1] = (1U << lastIndexPopCount) - 1U;
    }

    internal static void ThrowIfInvalidCapacity(int requiredNumberOfItems, int bufferLength)
    {
        if (requiredNumberOfItems > (bufferLength << Shift))
            throw new ArgumentException($"Number of items for Hasher exceeds max capacity of bit buffer! Requires: {requiredNumberOfItems} bits, buffer has {bufferLength << Shift} bits");
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
        var value = bits[index >>> Shift];
        value >>>= index;
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
        ref var arrayValue = ref bits[index >>> Shift];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;
        var delta = (arrayValue ^ oldValue) >>> index;
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
        bits[index >>> Shift] |= (1U << index);
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
        ref var arrayValue = ref bits[index >>> Shift];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);
        var delta = (arrayValue ^ oldValue) >>> index;
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
        bits[index >>> Shift] &= ~(1U << index);
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
        ref var arrayValue = ref bits[index >>> Shift];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;
        var result = arrayValue > oldValue;
        var delta = result ? 1 : -1;
        popCount += delta;
        return result;
    }

    public static void AssertSourceDataIsValidForDestination(ReadOnlySpan<uint> source, int destinationLength, int numberOfItems)
    {
        if (source.Length > destinationLength)
            throw new ArgumentException("Source buffer too big!");

        if (source.Length < destinationLength)
            return;

        var upperIntNumberOfItems = numberOfItems & Mask;

        if (upperIntNumberOfItems == 0)
            return;

        var lastInt = source[^1];
        var i = (1U << upperIntNumberOfItems) - 1U;
        if ((lastInt & ~i) != 0U)
            throw new ArgumentException("Upper bits set outside of valid range");
    }

    [Pure]
    public static int GetPopCount(ReadOnlySpan<uint> bits)
    {
        // Basic implementation is faster than using TensorPrimitives - benchmarks

        var result = 0;
        switch (bits.Length)
        {
            case 7: result += BitOperations.PopCount(bits[6]); goto case 6;
            case 6: result += BitOperations.PopCount(bits[5]); goto case 5;
            case 5: result += BitOperations.PopCount(bits[4]); goto case 4;
            case 4: result += BitOperations.PopCount(bits[3]); goto case 3;
            case 3: result += BitOperations.PopCount(bits[2]); goto case 2;
            case 2: result += BitOperations.PopCount(bits[1]); goto case 1;
            case 1: result += BitOperations.PopCount(bits[0]); return result;
            case 0: return 0;
        }

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

    [DoesNotReturn]
    private static void ThrowInvalidSpanLengthsException() => throw new ArgumentException("Spans have different lengths!");

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

        switch (span.Length)
        {
            case 7: span[6] &= other[6]; goto case 6;
            case 6: span[5] &= other[5]; goto case 5;
            case 5: span[4] &= other[4]; goto case 4;
            case 4: span[3] &= other[3]; goto case 3;
            case 3: span[2] &= other[2]; goto case 2;
            case 2: span[1] &= other[1]; goto case 1;
            case 1: span[0] &= other[0]; return;
            case 0: return;
        }

        TensorPrimitives.BitwiseAnd(span, other, span);
    }

    internal static void ExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        if (span.Length != other.Length)
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

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
            ThrowInvalidSpanLengthsException();

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] != other[i])
                return false;
        }

        return true;
    }

    internal struct SimpleBitEnumerator
    {
        private readonly uint[] _bits;

        private uint _v;
        private int _remaining;
        private int _index;

        public int Current { get; private set; }

        public SimpleBitEnumerator(uint[] bits, int popCount)
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
    }
}
