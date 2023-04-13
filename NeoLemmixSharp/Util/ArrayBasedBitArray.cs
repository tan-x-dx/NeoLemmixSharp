using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace NeoLemmixSharp.Util;

public sealed class ArrayBasedBitArray : IBitArray
{
    private readonly uint[] _uints;

    public int Length { get; }
    public int Count { get; private set; }
    public bool AnyBitsSet => Count > 0;

    public ArrayBasedBitArray(int length, bool initialBitFlag = false)
    {
        if (length < 1)
            throw new ArgumentException("length must be greater than zero", nameof(length));

        Length = length;

        var numberOfInts = Length + 31 >> 5;

        _uints = new uint[numberOfInts];

        if (!initialBitFlag)
            return;

        Array.Fill(_uints, uint.MaxValue);

        var shift = length & 31;
        var mask = (1U << shift) - 1U;
        _uints[^1] = mask;

        Count = ((_uints.Length - 1) << 5) + shift;
    }

    private ArrayBasedBitArray(int length, uint[] bits, int count)
    {
        Length = length;
        _uints = (uint[])bits.Clone();
        Count = count;
    }

    public bool GetBit(int index)
    {
        return (_uints[index >> 5] & (1U << index)) != 0U;
    }

    public bool SetBit(int index)
    {
        int intIndex = index >> 5;

        uint oldValue = _uints[intIndex];
        uint newValue = oldValue | (1U << index);

        if (oldValue == newValue)
            return false;

        Count++;
        _uints[intIndex] = newValue;

        return true;
    }

    public bool ClearBit(int index)
    {
        int intIndex = index >> 5;

        uint oldValue = _uints[intIndex];
        uint newValue = oldValue & ~(1U << index);

        if (oldValue == newValue)
            return false;

        Count--;
        _uints[intIndex] = newValue;

        return true;
    }

    public void Clear()
    {
        Array.Clear(_uints, 0, _uints.Length);
        Count = 0;
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        var remaining = Count;
        var index = 0;
        var v = _uints[0];

        while (remaining > 0)
        {
            while (v == 0U)
            {
                v = _uints[++index];
            }

            var m = BitOperations.TrailingZeroCount(v);
            v ^= 1U << m;
            array[arrayIndex++] = (index << 5) + m;
            remaining--;
        }
    }

    object ICloneable.Clone() => Clone();
    public ArrayBasedBitArray Clone() => new(Length, _uints, Count);

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<int>
    {
        private readonly ArrayBasedBitArray _arrayBasedBitArray;

        private uint _v;
        private int _remaining;
        private int _index;

        public int Current { get; private set; }

        public Enumerator(ArrayBasedBitArray arrayBasedBitArray)
        {
            _arrayBasedBitArray = arrayBasedBitArray;
            _v = _arrayBasedBitArray._uints[0];
            _remaining = _arrayBasedBitArray.Count;
            _index = 0;
            Current = -1;
        }

        public bool MoveNext()
        {
            if (_v == 0U)
            {
                if (_remaining == 0)
                    return false;

                do
                {
                    _v = _arrayBasedBitArray._uints[++_index];
                }
                while (_v == 0U);
            }

            var m = BitOperations.TrailingZeroCount(_v);
            _v ^= 1U << m;

            Current = (_index << 5) + m;
            _remaining--;
            return true;
        }

        public void Reset()
        {
            _v = _arrayBasedBitArray._uints[0];
            _remaining = _arrayBasedBitArray.Count;
            _index = 0;
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }
}