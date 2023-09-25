using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

/// <summary>
/// Can be initialized with any length (Values range from 0 - Length-1).
/// </summary>
public sealed class LargeBitArray : IBitArray
{
    public static LargeBitArray Empty { get; } = new();

    private const int Shift = 5;
    private const int Mask = (1 << Shift) - 1;

    private readonly uint[] _bits;

    private int _indexOfFirstSetBit;

    public int Length { get; }
    public int Count { get; private set; }

    private LargeBitArray()
    {
        _bits = Array.Empty<uint>();
    }

    public LargeBitArray(int length, bool initialBitFlag = false)
    {
        if (length < 1)
            throw new ArgumentException("length must be greater than zero", nameof(length));

        Length = length;

        var numberOfInts = (Length + Mask) >> Shift;

        _bits = new uint[numberOfInts];
        _indexOfFirstSetBit = numberOfInts - 1;

        if (!initialBitFlag)
            return;

        Array.Fill(_bits, uint.MaxValue);

        var shift = length & Mask;
        var mask = (1U << shift) - 1U;
        _bits[^1] = mask;
        Count = length;
        _indexOfFirstSetBit = 0;
    }

    private LargeBitArray(int length, uint[] bits, int count, int indexOfFirstSetBit)
    {
        Length = length;
        _bits = (uint[])bits.Clone();
        Count = count;
        _indexOfFirstSetBit = indexOfFirstSetBit;
    }

    [Pure]
    public bool GetBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var value = _bits[index >> Shift];
        value >>= index;
        return (value & 1U) != 0U;
    }

    public bool SetBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        ref var arrayValue = ref _bits[intIndex];
        var oldValue = arrayValue;
        arrayValue |= 1U << index;

        var delta = (arrayValue ^ oldValue) >> index;
        Count += (int)delta;

        _indexOfFirstSetBit = Math.Min(_indexOfFirstSetBit, intIndex);

        return delta != 0U;
    }

    public bool ClearBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        ref var arrayValue = ref _bits[intIndex];
        var oldValue = arrayValue;
        arrayValue &= ~(1U << index);

        var delta = (arrayValue ^ oldValue) >> index;
        Count -= (int)delta;

        FindIndexOfFirstSetBit();

        return delta != 0U;
    }

    public bool ToggleBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        ref var arrayValue = ref _bits[intIndex];
        var oldValue = arrayValue;
        arrayValue ^= 1U << index;

        bool result;
        if (arrayValue > oldValue)
        {
            Count++;
            result = true;
            _indexOfFirstSetBit = Math.Min(_indexOfFirstSetBit, intIndex);
        }
        else
        {
            Count--;
            result = false;
            FindIndexOfFirstSetBit();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FindIndexOfFirstSetBit()
    {
        while (_indexOfFirstSetBit < _bits.Length - 1 && _bits[_indexOfFirstSetBit] == 0U)
        {
            ++_indexOfFirstSetBit;
        }
    }

    public void Clear()
    {
        Array.Clear(_bits, 0, _bits.Length);
        Count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        var remaining = Count;
        var index = 0;
        var v = _bits[_indexOfFirstSetBit];

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
    object ICloneable.Clone() => Clone();
    [Pure]
    public LargeBitArray Clone() => new(Length, _bits, Count, _indexOfFirstSetBit);

    [Pure]
    public Enumerator GetEnumerator() => new(this);
    [Pure]
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);
    [Pure]
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<uint> _bitSpan;

        private uint _v;
        private int _remaining;
        private int _index;
        private int _current;

        public readonly int Current => _current;

        public Enumerator(LargeBitArray bitArray)
        {
            _bitSpan = bitArray.AsSpan();
            _index = bitArray._indexOfFirstSetBit;
            _v = _bitSpan.Length == 0U ? 0U : _bitSpan[_index];
            _remaining = bitArray.Count;
            _current = -1;
        }

        public bool MoveNext()
        {
            if (_v == 0U)
            {
                if (_remaining == 0)
                    return false;

                do
                {
                    _v = _bitSpan[++_index];
                }
                while (_v == 0U);
            }

            var m = BitOperations.TrailingZeroCount(_v);
            _v &= _v - 1;

            _current = (_index << Shift) | m;
            _remaining--;
            return true;
        }
    }

    public sealed class ReferenceTypeEnumerator : IEnumerator<int>
    {
        private readonly LargeBitArray _bitArray;

        private uint _v;
        private int _remaining;
        private int _index;
        private int _current;

        public int Current => _current;

        public ReferenceTypeEnumerator(LargeBitArray bitArray)
        {
            _bitArray = bitArray;
            _index = bitArray._indexOfFirstSetBit;
            var bits = bitArray._bits;
            _v = bits.Length == 0U ? 0 : bits[_index];
            _remaining = bitArray.Count;
            _current = -1;
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

            _current = (_index << Shift) | m;
            _remaining--;
            return true;
        }

        public void Reset()
        {
            _index = _bitArray._indexOfFirstSetBit;
            var bits = _bitArray._bits;
            _v = bits.Length == 0 ? 0U : bits[_index];
            _remaining = _bitArray.Count;
            _current = -1;
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<uint> AsSpan() => new(_bits);

    internal void UnionWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue |= other[i];
            count += BitOperations.PopCount(arrayValue);
            if (arrayValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void IntersectWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue &= other[i];
            count += BitOperations.PopCount(arrayValue);
            if (arrayValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void ExceptWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue &= ~other[i];
            count += BitOperations.PopCount(arrayValue);
            if (arrayValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void SymmetricExceptWith(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            ref var arrayValue = ref _bits[i];
            arrayValue ^= other[i];
            count += BitOperations.PopCount(arrayValue);
            if (arrayValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    [Pure]
    internal bool IsSubsetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other[i];
            if ((bits | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsSupersetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other[i];
            if ((bits | otherBits) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsProperSubsetOf(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        var allEqual = true;
        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
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
        for (var i = 0; i < _bits.Length; i++)
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

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other[i];
            if ((bits & otherBits) != 0)
                return true;
        }

        return false;
    }

    [Pure]
    internal bool SetEquals(ReadOnlySpan<uint> other)
    {
        Debug.Assert(_bits.Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other[i];
            if (bits != otherBits)
                return false;
        }

        return true;
    }
}