using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace NeoLemmixSharp.Util;

public sealed class IntBasedBitArray : IBitArray
{
    private const int Size = 32;

    private uint _bits;

    public int Count { get; private set; }
    public int Length => Size;
    public bool AnyBitsSet => _bits != 0U;

    public IntBasedBitArray(uint initialBits = 0U)
    {
        _bits = initialBits;
        Count = initialBits == 0U ? 0 : BitOperations.PopCount(_bits);
    }

    private IntBasedBitArray(uint bits, int count)
    {
        _bits = bits;
        Count = count;
    }

    public bool GetBit(int index)
    {
        return (_bits >> index & 1U) != 0U;
    }

    public bool SetBit(int index)
    {
        uint oldValue = _bits;
        _bits |= 1U << index;
        var delta = (_bits ^ oldValue) >> index;
        Count += (int)delta;
        return delta != 0U;
    }

    public bool ClearBit(int index)
    {
        uint oldValue = _bits;
        _bits &= ~(1U << index);
        var delta = (_bits ^ oldValue) >> index;
        Count -= (int)delta;
        return delta != 0U;
    }

    public void Clear()
    {
        _bits = 0U;
        Count = 0;
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        var v = _bits;
        while (v != 0U)
        {
            var m = BitOperations.TrailingZeroCount(v);
            v ^= 1U << m;
            array[arrayIndex++] = m;
        }
    }

    object ICloneable.Clone() => Clone();
    public IntBasedBitArray Clone() => new(_bits, Count);

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<int>
    {
        private readonly IntBasedBitArray _bitField;
        private uint _v;

        public int Current { get; private set; }

        public Enumerator(IntBasedBitArray bitField)
        {
            _bitField = bitField;
            _v = _bitField._bits;
            Current = -1;
        }

        public bool MoveNext()
        {
            if (_v == 0U)
                return false;
            Current = BitOperations.TrailingZeroCount(_v);
            _v ^= 1U << Current;

            return true;
        }

        public void Reset()
        {
            _v = _bitField._bits;
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }
}
