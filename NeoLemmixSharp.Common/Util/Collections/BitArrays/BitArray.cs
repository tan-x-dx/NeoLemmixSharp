using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class BitArray
{
    public const int Shift = 5;
    public const int Mask = (1 << Shift) - 1;

    private readonly uint[] _bits;
    private int _popCount;

    /// <summary>
    /// The footprint of this BitArray - how many uints it logically represents.
    /// </summary>
    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _bits.Length;
    }

    /// <summary>
    /// The population count (number of bits set) of the bit array.
    /// </summary>
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public int PopCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _popCount;
    }

    public BitArray(int length, bool setAllBits = false)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "length must be non-negative!");

        var arraySize = (length + Mask) >> Shift;
        _bits = new uint[arraySize];

        if (!setAllBits || arraySize == 0)
            return;

        Array.Fill(_bits, uint.MaxValue);

        var lastIndexPopCount = length & Mask;

        if (lastIndexPopCount != 0)
        {
            _bits[^1] = (1U << lastIndexPopCount) - 1U;
        }

        _popCount = length;
    }

    /// <summary>
    /// Tests if a specific bit is set
    /// </summary>
    /// <param name="index">The bit to query</param>
    /// <returns><see langword="true" /> if the specified bit is set</returns>
    [Pure]
    public bool GetBit(int index)
    {
        var value = _bits[index >> Shift];
        value >>= index;
        return (value & 1U) != 0U;
    }

    /// <summary>
    /// Sets a bit to 1. Returns <see langword="true" /> if a change has occurred -
    /// i.e. if the bit was previously 0
    /// </summary>
    /// <param name="index">The bit to set</param>
    /// <returns><see langword="true" /> if the operation changed the value of the bit, <see langword="false" /> if the bit was previously set</returns>
    public bool SetBit(int index)
    {
        ref var arrayValue = ref _bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;
        var delta = (arrayValue ^ oldValue) >> index;
        _popCount += (int)delta;
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
    /// <param name="index">The bit to clear</param>
    /// <returns><see langword="true" /> if the operation changed the value of the bit, <see langword="false" /> if the bit was previously clear</returns>
    public bool ClearBit(int index)
    {
        ref var arrayValue = ref _bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);
        var delta = (arrayValue ^ oldValue) >> index;
        _popCount -= (int)delta;
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
    /// <param name="index">The bit to toggle</param>
    /// <returns>The bool equivalent of the binary value (0 or 1) of the bit after toggling</returns>
    public bool ToggleBit(int index)
    {
        ref var arrayValue = ref _bits[index >> Shift];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;
        var result = arrayValue > oldValue;
        var delta = result ? 1 : -1;
        _popCount += delta;
        return result;
    }

    public void Clear()
    {
        Array.Clear(_bits);
        _popCount = 0;
    }

    public int[] ToArray()
    {
        if (_popCount == 0)
            return Array.Empty<int>();

        var array = new int[_popCount];
        CopyTo(array, 0);
        return array;
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        var remaining = _popCount;
        var index = 0;
        var v = _bits[0];

        while (remaining > 0)
        {
            while (v == 0U)
            {
                v = _bits[++index];
            }

            var m = BitOperations.TrailingZeroCount(v);
            v &= v - 1;
            array[arrayIndex++] = (index << Shift) | m;
            remaining--;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<uint> AsReadOnlySpan() => new(_bits);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReferenceTypeBitEnumerator GetEnumerator() => new(this);

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
            var bits = bitArray._bits;
            _v = bits.Length == 0 ? 0U : bits[0];
            _remaining = bitArray._popCount;
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
                    _v = _bitArray._bits[++_index];
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
            var bits = _bitArray._bits;
            _v = bits.Length == 0 ? 0U : bits[0];
            _remaining = _bitArray._popCount;
            Current = -1;
        }

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

    internal void UnionWith(ReadOnlySpan<uint> other)
    {
        var span = new Span<uint>(_bits);

        var newCount = 0;
        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var v = ref span[i];
            v |= other[i];
            newCount += BitOperations.PopCount(v);
        }
        _popCount = newCount;
    }

    internal static void UnionWith(Span<uint> span, ReadOnlySpan<uint> other)
    {
        Debug.Assert(span.Length == other.Length);

        for (var i = span.Length - 1; i >= 0; i--)
        {
            ref var v = ref span[i];
            v |= other[i];
        }
    }

    internal void IntersectWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue &= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        _popCount = count;
    }

    internal void ExceptWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue &= ~other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        _popCount = count;
    }

    internal void SymmetricExceptWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue ^= other[i];
            count += BitOperations.PopCount(arrayValue);
        }
        _popCount = count;
    }

    [Pure]
    internal bool IsSubsetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            var otherBits = other[i];
            if ((_bits[i] | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsSupersetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            var bits = _bits[i];
            if ((bits | other[i]) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsProperSubsetOf(ReadOnlySpan<uint> other)
    {
        var span = new ReadOnlySpan<uint>(_bits);
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
    internal bool IsProperSupersetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var allEqual = true;
        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            var bits = _bits[i];
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
        Debug.Assert(_bits.Length == other.Length);

        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            if ((_bits[i] & other[i]) != 0U)
                return true;
        }

        return false;
    }

    [Pure]
    internal bool SetEquals(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            if (_bits[i] != other[i])
                return false;
        }

        return true;
    }
}