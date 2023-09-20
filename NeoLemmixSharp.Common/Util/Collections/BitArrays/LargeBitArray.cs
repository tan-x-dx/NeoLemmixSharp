﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;

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

        return (_bits[index >> Shift] & (1U << index)) != 0U;
    }

    public bool SetBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        var oldValue = _bits[intIndex];
        var newValue = oldValue | (1U << index);

        if (oldValue == newValue)
            return false;

        _bits[intIndex] = newValue;
        Count++;
        _indexOfFirstSetBit = Math.Min(_indexOfFirstSetBit, intIndex);

        return true;
    }

    public bool ClearBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        var oldValue = _bits[intIndex];
        var newValue = oldValue & ~(1U << index);

        if (oldValue == newValue)
            return false;

        _bits[intIndex] = newValue;
        Count--;
        FindIndexOfFirstSetBit();

        return true;
    }

    public bool ToggleBit(int index)
    {
        Debug.Assert(index >= 0 && index < Length);

        var intIndex = index >> Shift;

        var oldValue = _bits[intIndex];
        var newValue = oldValue ^ (1U << index);
        _bits[intIndex] = newValue;

        bool result;
        if (newValue > oldValue)
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
        var v = _bits[0];

        while (remaining > 0)
        {
            while (v == 0U)
            {
                v = _bits[++index];
            }

            var m = BitOperations.TrailingZeroCount(v);
            v ^= 1U << m;
            array[arrayIndex++] = (index << Shift) | m;
            remaining--;
        }
    }

    object ICloneable.Clone() => Clone();
    public LargeBitArray Clone() => new(Length, _bits, Count, _indexOfFirstSetBit);

    public Enumerator GetEnumerator() => new(this);
    public ReferenceTypeEnumerator GetReferenceTypeEnumerator() => new(this);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ReferenceTypeEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new ReferenceTypeEnumerator(this);

    public ref struct Enumerator
    {
        private readonly LargeBitArray _bitArray;

        private uint _v;
        private int _remaining;
        private int _index;
        private int _current;

        public readonly int Current => _current;

        public Enumerator(LargeBitArray bitArray)
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
            _v ^= 1U << m;

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
            _v ^= 1U << m;

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

    internal void UnionWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            var newValue = _bits[i] | other._bits[i];
            _bits[i] = newValue;
            count += BitOperations.PopCount(newValue);
            if (newValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void IntersectWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            var newValue = _bits[i] & other._bits[i];
            _bits[i] = newValue;
            count += BitOperations.PopCount(newValue);
            if (newValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void ExceptWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            var newValue = _bits[i] & ~other._bits[i];
            _bits[i] = newValue;
            count += BitOperations.PopCount(newValue);
            if (newValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    internal void SymmetricExceptWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        _indexOfFirstSetBit = _bits.Length - 1;
        for (var i = 0; i < _bits.Length; i++)
        {
            var newValue = _bits[i] ^ other._bits[i];
            _bits[i] = newValue;
            count += BitOperations.PopCount(newValue);
            if (newValue != 0U && i < _indexOfFirstSetBit)
            {
                _indexOfFirstSetBit = i;
            }
        }
        Count = count;
    }

    [Pure]
    internal bool IsSubsetOf(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if ((bits | otherBits) != otherBits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsSupersetOf(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if ((bits | otherBits) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsProperSubsetOf(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var allEqual = true;
        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            allEqual &= bits == otherBits;

            if ((bits | otherBits) != otherBits)
                return false;
        }

        return !allEqual;
    }

    [Pure]
    internal bool IsProperSupersetOf(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var allEqual = true;
        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            allEqual &= bits == otherBits;

            if ((bits | otherBits) != bits)
                return false;
        }

        return !allEqual;
    }

    [Pure]
    internal bool Overlaps(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if ((bits & otherBits) != 0)
                return true;
        }

        return false;
    }

    [Pure]
    internal bool SetEquals(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if (bits != otherBits)
                return false;
        }

        return true;
    }
}