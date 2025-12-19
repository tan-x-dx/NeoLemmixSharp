using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            ThrowInvalidaCapacityException(requiredNumberOfItems, bufferLength);
    }

    [DoesNotReturn]
    private static void ThrowInvalidaCapacityException(int requiredNumberOfItems, int bufferLength) => throw new ArgumentException($"Number of items for Hasher exceeds max capacity of bit buffer! Requires: {requiredNumberOfItems} bits, buffer has {bufferLength << Shift} bits");

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
        uint mask = 1U;
        mask <<= index;

        bits[index >>> Shift] |= mask;
    }

    /// <summary>
    /// Sets a bit to 1
    /// </summary>
    /// <param name="p">The pointer to modify</param>
    /// <param name="index">The bit to set</param>
    internal static unsafe void SetBit(uint* p, int index)
    {
        uint mask = 1U;
        mask <<= index;

        index >>>= Shift;
        p[index] |= mask;
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
        uint mask = 1U;
        mask <<= index;
        mask = ~mask;

        bits[index >>> Shift] &= mask;
    }

    /// <summary>
    /// Sets a bit to 0. 
    /// </summary>
    /// <param name="p">The pointer to modify</param>
    /// <param name="index">The bit to clear</param>
    internal static unsafe void ClearBit(uint* p, int index)
    {
        uint mask = 1U;
        mask <<= index;
        mask = ~mask;

        index >>>= Shift;
        p[index] &= mask;
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
        // This implementation is faster than using TensorPrimitives - benchmarks

        ref uint startRef = ref MemoryMarshal.GetReference(bits);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(bits), bits.Length);
        var result = 0;
        while (Unsafe.IsAddressLessThan(ref startRef, ref endRef))
        {
            result += BitOperations.PopCount(startRef);
            startRef = ref Unsafe.Add(ref startRef, 1);
        }

        return result;
    }

    /// <summary>
    /// PopCount alternative that uses raw pointers.
    /// Used by SpacialHashGrid since that class calculates PopCount a lot.
    /// </summary>
    /// <param name="sourcePointer">The uints to collectively PopCount.</param>
    /// <param name="length">The number of uints to evaluate.</param>
    /// <returns>The PopCount over all uints specified by the pointer/length.</returns>
    [Pure]
    internal static unsafe int GetPopCount(uint* sourcePointer, uint length)
    {
        // This implementation is faster than using TensorPrimitives - benchmarks

        uint* endPointer = sourcePointer + length;
        var result = 0;
        while (sourcePointer < endPointer)
        {
            result += BitOperations.PopCount(*sourcePointer);
            sourcePointer++;
        }

        return result;
    }

    /*
     * Justification for the weird bullshit below:
     * 
     * Consider an average level. Chances are there are <= 100 lemmings present.
     * In fact, the overwhelming majority of levels in existence have 100 lemmings
     * or fewer (A cursory glance at a sample of ~10,000 levels suggests that
     * at least 90% of levels follow this rule).
     * If we're storing lemming bits in uints (which we'll be doing a lot),
     * then we will require 100 bits max. This corresponds to four 32bit ints.
     * 
     * Now, that's just for lemmings. There are other things to consider too,
     * namely: gadgets, and renderers. Chances are there will be at most a couple
     * dozen gadgets (i.e. < 100). As for renderers, this quantity is approximately
     * equal to number of lemmings + number of gadgets. So, might be a smidge over 100.
     * 
     * All in all, the input spans will most likely have lengths of four or five (or lower).
     * 
     * We can optimise for these common use cases. This is ESPECIALLY important
     * for the Union methods as they are the most commonly used. Benchmarks have
     * shown that the switch/case/goto approach yields the fastest results for
     * these small span lengths.
     * The TensorPrimitives library can pick up the slack for larger span lengths.
     */

    [DoesNotReturn]
    private static void ThrowInvalidSpanLengthsException() => throw new ArgumentException("Spans have different lengths!");

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        var spanLength = (uint)span.Length;
        var otherLength = (uint)other.Length;

        if (spanLength != otherLength)
            ThrowInvalidSpanLengthsException();

        ref uint sourceRef = ref MemoryMarshal.GetReference(span);
        ref uint otherRef = ref MemoryMarshal.GetReference(other);

        switch (spanLength)
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

            default: LargeSpanUnionWith(span, other); return;
        }

    Length8:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 7);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 7);
        sourceRef |= otherRef;
    Length7:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 6);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 6);
        sourceRef |= otherRef;
    Length6:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 5);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 5);
        sourceRef |= otherRef;
    Length5:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 4);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 4);
        sourceRef |= otherRef;
    Length4:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 3);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 3);
        sourceRef |= otherRef;
    Length3:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 2);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 2);
        sourceRef |= otherRef;
    Length2:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 1);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 1);
        sourceRef |= otherRef;
    Length1:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 0);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 0);
        sourceRef |= otherRef;
    Length0:
        return;
    }

    private static unsafe void LargeSpanUnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        TensorPrimitives.BitwiseOr(span, other, span);
    }

    /// <summary>
    /// UnionWith alternative that uses raw pointers.
    /// Used by SpacialHashGrid since that class does lots of union operations.
    /// </summary>
    /// <param name="sourcePointer">The pointer whose bits are to be modified. This pointer is written to.</param>
    /// <param name="otherPointer">The pointer whose bits are used in the masking operation. This pointer is used read only.</param>
    /// <param name="length">The number of uints that need to be modified.</param>
    internal static unsafe void UnionWith(uint* sourcePointer, uint* otherPointer, uint length)
    {
        switch (length)
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

            default: LargeSpanUnionWith(sourcePointer, otherPointer, length); return;
        }

    Length8:
        sourcePointer[7] |= otherPointer[7];
    Length7:
        sourcePointer[6] |= otherPointer[6];
    Length6:
        sourcePointer[5] |= otherPointer[5];
    Length5:
        sourcePointer[4] |= otherPointer[4];
    Length4:
        sourcePointer[3] |= otherPointer[3];
    Length3:
        sourcePointer[2] |= otherPointer[2];
    Length2:
        sourcePointer[1] |= otherPointer[1];
    Length1:
        sourcePointer[0] |= otherPointer[0];
    Length0:
        return;
    }

    private static unsafe void LargeSpanUnionWith(void* sourcePointer, void* otherPointer, uint length)
    {
        var spanLength = (int)length;
        var x = new ReadOnlySpan<uint>(sourcePointer, spanLength);
        var y = new ReadOnlySpan<uint>(otherPointer, spanLength);
        var destination = new Span<uint>(sourcePointer, spanLength);

        TensorPrimitives.BitwiseOr(x, y, destination);
    }

    internal static void IntersectWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        var spanLength = (uint)span.Length;
        var otherLength = (uint)other.Length;

        if (spanLength != otherLength)
            ThrowInvalidSpanLengthsException();

        ref uint sourceRef = ref MemoryMarshal.GetReference(span);
        ref uint otherRef = ref MemoryMarshal.GetReference(other);

        switch (spanLength)
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

            default: LargeSpanIntersectWith(span, other); return;
        }

    Length8:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 7);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 7);
        sourceRef &= otherRef;
    Length7:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 6);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 6);
        sourceRef &= otherRef;
    Length6:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 5);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 5);
        sourceRef &= otherRef;
    Length5:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 4);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 4);
        sourceRef &= otherRef;
    Length4:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 3);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 3);
        sourceRef &= otherRef;
    Length3:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 2);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 2);
        sourceRef &= otherRef;
    Length2:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 1);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 1);
        sourceRef &= otherRef;
    Length1:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 0);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 0);
        sourceRef &= otherRef;
    Length0:
        return;
    }

    private static void LargeSpanIntersectWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        TensorPrimitives.BitwiseAnd(span, other, span);
    }

    internal static void ExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        var spanLength = (uint)span.Length;
        var otherLength = (uint)other.Length;

        if (spanLength != otherLength)
            ThrowInvalidSpanLengthsException();

        // There is no good TensorPrimitives equivalent operation
        // for this one, so just do one big loop

        ref uint sourceRef = ref MemoryMarshal.GetReference(span);
        ref uint otherRef = ref MemoryMarshal.GetReference(other);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), spanLength);

        while (Unsafe.IsAddressLessThan(ref sourceRef, ref endRef))
        {
            sourceRef &= ~otherRef;
            sourceRef = ref Unsafe.Add(ref sourceRef, 1);
            otherRef = ref Unsafe.Add(ref otherRef, 1);
        }
    }

    internal static void SymmetricExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        var spanLength = (uint)span.Length;
        var otherLength = (uint)other.Length;

        if (spanLength != otherLength)
            ThrowInvalidSpanLengthsException();

        ref uint sourceRef = ref MemoryMarshal.GetReference(span);
        ref uint otherRef = ref MemoryMarshal.GetReference(other);

        switch (spanLength)
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

            default: LargeSpanSymmetricExceptWith(span, other); return;
        }

    Length8:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 7);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 7);
        sourceRef ^= otherRef;
    Length7:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 6);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 6);
        sourceRef ^= otherRef;
    Length6:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 5);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 5);
        sourceRef ^= otherRef;
    Length5:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 4);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 4);
        sourceRef ^= otherRef;
    Length4:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 3);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 3);
        sourceRef ^= otherRef;
    Length3:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 2);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 2);
        sourceRef ^= otherRef;
    Length2:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 1);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 1);
        sourceRef ^= otherRef;
    Length1:
        sourceRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), 0);
        otherRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(other), 0);
        sourceRef ^= otherRef;
    Length0:
        return;
    }

    private static void LargeSpanSymmetricExceptWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        TensorPrimitives.Xor(span, other, span);
    }

    [Pure]
    internal static bool IsSubsetOf(ReadOnlySpan<uint> firstSpan, ReadOnlySpan<uint> secondSpan)
    {
        var firstSpanLength = (uint)firstSpan.Length;
        var secondSpanLength = (uint)secondSpan.Length;

        if (firstSpanLength != secondSpanLength)
            ThrowInvalidSpanLengthsException();

        ref uint firstSpanRef = ref MemoryMarshal.GetReference(firstSpan);
        ref uint secondSpanRef = ref MemoryMarshal.GetReference(secondSpan);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(firstSpan), firstSpanLength);

        while (Unsafe.IsAddressLessThan(ref firstSpanRef, ref endRef))
        {
            uint firstValue = firstSpanRef;
            uint secondValue = secondSpanRef;

            firstValue |= secondValue;

            if (firstValue != secondValue)
                return false;
            firstSpanRef = ref Unsafe.Add(ref firstSpanRef, 1);
            secondSpanRef = ref Unsafe.Add(ref secondSpanRef, 1);
        }

        return true;
    }

    [Pure]
    internal static bool IsProperSubsetOf(ReadOnlySpan<uint> firstSpan, ReadOnlySpan<uint> secondSpan)
    {
        var firstSpanLength = (uint)firstSpan.Length;
        var secondSpanLength = (uint)secondSpan.Length;

        if (firstSpanLength != secondSpanLength)
            ThrowInvalidSpanLengthsException();

        ref uint firstSpanRef = ref MemoryMarshal.GetReference(firstSpan);
        ref uint secondSpanRef = ref MemoryMarshal.GetReference(secondSpan);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(firstSpan), firstSpanLength);

        var allEqual = true;

        while (Unsafe.IsAddressLessThan(ref firstSpanRef, ref endRef))
        {
            uint firstValue = firstSpanRef;
            uint secondValue = secondSpanRef;

            allEqual &= firstValue == secondValue;
            firstValue |= secondValue;

            if (firstValue != secondValue)
                return false;
            firstSpanRef = ref Unsafe.Add(ref firstSpanRef, 1);
            secondSpanRef = ref Unsafe.Add(ref secondSpanRef, 1);
        }

        return !allEqual;
    }

    [Pure]
    internal static bool Overlaps(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        var firstSpanLength = (uint)span.Length;
        var secondSpanLength = (uint)other.Length;

        if (firstSpanLength != secondSpanLength)
            ThrowInvalidSpanLengthsException();

        ref uint firstSpanRef = ref MemoryMarshal.GetReference(span);
        ref uint secondSpanRef = ref MemoryMarshal.GetReference(other);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), firstSpanLength);

        while (Unsafe.IsAddressLessThan(ref firstSpanRef, ref endRef))
        {
            if ((firstSpanRef & secondSpanRef) != 0U)
                return true;
            firstSpanRef = ref Unsafe.Add(ref firstSpanRef, 1);
            secondSpanRef = ref Unsafe.Add(ref secondSpanRef, 1);
        }

        return false;
    }

    [Pure]
    internal static bool SetEquals(ReadOnlySpan<uint> span, ReadOnlySpan<uint> other)
    {
        var firstSpanLength = (uint)span.Length;
        var secondSpanLength = (uint)other.Length;

        if (firstSpanLength != secondSpanLength)
            ThrowInvalidSpanLengthsException();

        ref uint firstSpanRef = ref MemoryMarshal.GetReference(span);
        ref uint secondSpanRef = ref MemoryMarshal.GetReference(other);
        ref uint endRef = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), firstSpanLength);

        while (Unsafe.IsAddressLessThan(ref firstSpanRef, ref endRef))
        {
            if (firstSpanRef != secondSpanRef)
                return false;
            firstSpanRef = ref Unsafe.Add(ref firstSpanRef, 1);
            secondSpanRef = ref Unsafe.Add(ref secondSpanRef, 1);
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
