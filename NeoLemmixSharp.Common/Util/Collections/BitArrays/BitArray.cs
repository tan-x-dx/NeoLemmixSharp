using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class BitArray : ICollection<int>, IReadOnlyCollection<int>
{
    public const int Shift = 5;

    private readonly IUintWrapper _uintWrapper;

    /// <summary>
    /// The footprint of this BitArray - how many uints it logically represents
    /// </summary>
    public int Size => _uintWrapper.Size;
    /// <summary>
    /// The maximum number of integers that this BitArray can carry
    /// </summary>
    public int Length => Size << Shift;
    /// <summary>
    /// The number of integers currently stored in this BitArray
    /// </summary>
    public int Count { get; private set; }

    [Pure]
    public static BitArray CreateForLength(int specifiedLength)
    {
        IUintWrapper uintWrapper = specifiedLength > SingleUintWrapper.SmallBitArrayLength
            ? new UintArrayWrapper(specifiedLength)
            : new SingleUintWrapper();

        return new BitArray(uintWrapper);
    }

    private BitArray(IUintWrapper uintWrapper)
    {
        _uintWrapper = uintWrapper;
    }

    public BitArray(IUintWrapper uintWrapper, bool calculatePopCount)
    {
        _uintWrapper = uintWrapper;

        if (!calculatePopCount)
            return;

        Count = PopCount(uintWrapper.AsReadOnlySpan());
    }

    /// <summary>
    /// Tests if a specific bit is set
    /// </summary>
    /// <param name="index">The bit to query</param>
    /// <returns>True if the specified bit is set</returns>
    [Pure]
    public bool GetBit(int index)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        var value = span[index >> Shift];
        value >>= index;
        return (value & 1U) != 0U;
    }

    /// <summary>
    /// Sets a bit to 1. Returns true if a change has occurred -
    /// i.e. if the bit was previously 0
    /// </summary>
    /// <param name="index">The bit to set</param>
    /// <returns>True if the operation changed the value of the bit, false if the bit was previously set</returns>
    public bool SetBit(int index)
    {
        var span = _uintWrapper.AsSpan();
        var delta = SetBit(span, index);
        Count += delta;

        return delta != 0;
    }

    /// <summary>
    /// Sets a bit to 1. Returns a value of 1 if a change has occurred -
    /// i.e. if the bit was previously 0. Returns a value of 0 otherwise
    /// </summary>
    /// <param name="bits">The span to modify</param>
    /// <param name="index">The bit to set</param>
    /// <returns>1 if the operation changed the value of the bit, 0 if the bit was previously set</returns>
    public static int SetBit(Span<uint> bits, int index)
    {
        var intIndex = index >> Shift;

        ref var arrayValue = ref bits[intIndex];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;

        var delta = (arrayValue ^ oldValue) >> index;
        return (int)delta;
    }

    /// <summary>
    /// Sets a bit to 0. Returns true if a change has occurred -
    /// i.e. if the bit was previously 1
    /// </summary>
    /// <param name="index">The bit to clear</param>
    /// <returns>True if the operation changed the value of the bit, false if the bit was previously clear</returns>
    public bool ClearBit(int index)
    {
        var span = _uintWrapper.AsSpan();
        var delta = ClearBit(span, index);
        Count -= delta;

        return delta != 0;
    }

    /// <summary>
    /// Sets a bit to 0. Returns a value of 1 if a change has occurred -
    /// i.e. if the bit was previously 1. Returns a value of 0 otherwise
    /// </summary>
    /// <param name="bits">The bit to clear</param>
    /// <param name="index">The bit to clear</param>
    /// <returns>1 if the operation changed the value of the bit, 0 if the bit was previously clear</returns>
    public static int ClearBit(Span<uint> bits, int index)
    {
        var intIndex = index >> Shift;

        ref var arrayValue = ref bits[intIndex];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);

        var delta = (arrayValue ^ oldValue) >> index;
        return (int)delta;
    }

    /// <summary>
    /// Toggles the value of a bit. Returns the new value after toggling
    /// </summary>
    /// <param name="index">The bit to toggle</param>
    /// <returns>The bool equivalent of the binary value (0 or 1) of the bit after toggling</returns>
    public bool ToggleBit(int index)
    {
        var intIndex = index >> Shift;

        var span = _uintWrapper.AsSpan();
        ref var arrayValue = ref span[intIndex];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;

        var result = arrayValue > oldValue;
        var delta = result ? 1 : -1;

        Count += delta;

        return result;
    }

    public void Clear()
    {
        _uintWrapper.AsSpan().Clear();
        Count = 0;
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        var remaining = Count;
        var index = 0;
        var span = _uintWrapper.AsReadOnlySpan();
        var v = span[0];

        while (remaining > 0)
        {
            while (v == 0U)
            {
                v = span[++index];
            }

            var m = BitOperations.TrailingZeroCount(v);
            v &= v - 1;
            array[arrayIndex++] = (index << Shift) | m;
            remaining--;
        }
    }

    void ICollection<int>.Add(int i)
    {
        if (i < 0 || i >= Length)
            throw new ArgumentOutOfRangeException(nameof(i), i, $"Can only add items if they are between 0 and {Length - 1}");

        SetBit(i);
    }

    bool ICollection<int>.Contains(int i) => i >= 0 && i < Length && GetBit(i);
    bool ICollection<int>.Remove(int i) => i >= 0 && i < Length && ClearBit(i);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<uint> AsReadOnlySpan() => _uintWrapper.AsReadOnlySpan();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReferenceTypeBitEnumerator GetEnumerator() => new(this);

    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public sealed class ReferenceTypeBitEnumerator : IEnumerator<int>
    {
        private readonly BitArray _bitArray;

        private uint _v;
        private int _remaining;
        private int _index;

        public int Current { get; private set; }

        public ReferenceTypeBitEnumerator(BitArray bitArray)
        {
            _bitArray = bitArray;
            _index = 0;
            var bits = bitArray.AsReadOnlySpan();
            _v = bits.Length == 0 ? 0U : bits[0];
            _remaining = bitArray.Count;
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
                    _v = _bitArray.AsReadOnlySpan()[++_index];
                }
                while (_v == 0U);
            }

            var m = BitOperations.TrailingZeroCount(_v);
            _v &= _v - 1;

            Current = (_index << Shift) | m;
            _remaining--;
            return true;
        }

        public void Reset()
        {
            _index = 0;
            var bits = _bitArray.AsReadOnlySpan();
            _v = bits.Length == 0 ? 0U : bits[0];
            _remaining = _bitArray.Count;
            Current = -1;
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }

    [Pure]
    internal static int PopCount(ReadOnlySpan<uint> bits)
    {
        var result = 0;
        foreach (var v in bits)
        {
            result += BitOperations.PopCount(v);
        }

        return result;
    }

    internal void UnionWith(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsSpan();

        var newCount = UnionWith(span, other);
        Count = newCount;
    }

    internal static int UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = 0; i < span.Length; i++)
        {
            ref var v = ref span[i];
            v |= other[i];
            count += BitOperations.PopCount(v);
        }
        return count;
    }

    internal void IntersectWith(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsSpan();
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = 0; i < span.Length; i++)
        {
            ref var arrayValue = ref span[i];
            arrayValue &= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        Count = count;
    }

    internal void ExceptWith(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsSpan();
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = 0; i < span.Length; i++)
        {
            ref var arrayValue = ref span[i];
            arrayValue &= ~other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        Count = count;
    }

    internal void SymmetricExceptWith(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsSpan();
        Debug.Assert(span.Length == other.Length);

        var count = 0;
        for (var i = 0; i < span.Length; i++)
        {
            ref var arrayValue = ref span[i];
            arrayValue ^= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        Count = count;
    }

    [Pure]
    internal bool IsSubsetOf(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

        for (var i = 0; i < span.Length; i++)
        {
            var bits = span[i];
            var otherBits = other[i];
            if ((bits | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsSupersetOf(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

        for (var i = 0; i < span.Length; i++)
        {
            var bits = span[i];
            var otherBits = other[i];
            if ((bits | otherBits) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsProperSubsetOf(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

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
    internal bool IsProperSupersetOf(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

        var allEqual = true;
        for (var i = 0; i < span.Length; i++)
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
    internal bool Overlaps(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

        for (var i = 0; i < span.Length; i++)
        {
            var bits = span[i];
            var otherBits = other[i];
            if ((bits & otherBits) != 0)
                return true;
        }

        return false;
    }

    [Pure]
    internal bool SetEquals(ReadOnlySpan<uint> other)
    {
        var span = _uintWrapper.AsReadOnlySpan();
        Debug.Assert(span.Length == other.Length);

        for (var i = 0; i < span.Length; i++)
        {
            var bits = span[i];
            var otherBits = other[i];
            if (bits != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<int>.IsReadOnly => false;
}