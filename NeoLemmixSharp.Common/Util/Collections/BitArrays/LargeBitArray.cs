using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

/// <summary>
/// Can be initialized with any length (Values range from 0 - Length-1).
/// </summary>
public sealed class LargeBitArray : IBitArray
{
    private readonly uint[] _bits;

    public int Length { get; }
    public int Count { get; private set; }

    public LargeBitArray(int length, bool initialBitFlag = false)
    {
        if (length < 1)
            throw new ArgumentException("length must be greater than zero", nameof(length));

        Length = length;

        var numberOfInts = (Length + 31) >> 5;

        _bits = new uint[numberOfInts];

        if (!initialBitFlag)
            return;

        Array.Fill(_bits, uint.MaxValue);

        var shift = length & 31;
        var mask = (1U << shift) - 1U;
        _bits[^1] = mask;
        Count = length;
    }

    private LargeBitArray(int length, uint[] bits, int count)
    {
        Length = length;
        _bits = (uint[])bits.Clone();
        Count = count;
    }

    [Pure]
    public bool GetBit(int index)
    {
        return (_bits[index >> 5] & (1U << index)) != 0U;
    }

    public bool SetBit(int index)
    {
        var intIndex = index >> 5;

        var oldValue = _bits[intIndex];
        var newValue = oldValue | (1U << index);

        if (oldValue == newValue)
            return false;

        _bits[intIndex] = newValue;
        Count++;

        return true;
    }

    public bool ClearBit(int index)
    {
        var intIndex = index >> 5;

        var oldValue = _bits[intIndex];
        var newValue = oldValue & ~(1U << index);

        if (oldValue == newValue)
            return false;

        _bits[intIndex] = newValue;
        Count--;

        return true;
    }

    public bool ToggleBit(int index)
    {
        var intIndex = index >> 5;

        var oldValue = _bits[intIndex];
        var newValue = oldValue ^ (1U << index);
        _bits[intIndex] = newValue;

        bool result;
        if (newValue > oldValue)
        {
            Count++;
            result = true;
        }
        else
        {
            Count--;
            result = false;
        }

        return result;
    }

    public void Clear()
    {
        Array.Clear(_bits, 0, _bits.Length);
        Count = 0;
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
            array[arrayIndex++] = (index << 5) | m;
            remaining--;
        }
    }

    object ICloneable.Clone() => Clone();
    public LargeBitArray Clone() => new(Length, _bits, Count);

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<int>
    {
        private readonly LargeBitArray _bitArray;

        private uint _v;
        private int _remaining;
        private int _index;

        public int Current { get; private set; }

        public Enumerator(LargeBitArray bitArray)
        {
            _bitArray = bitArray;
            _v = _bitArray._bits[0];
            _remaining = _bitArray.Count;
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
                    _v = _bitArray._bits[++_index];
                }
                while (_v == 0U);
            }

            var m = BitOperations.TrailingZeroCount(_v);
            _v ^= 1U << m;

            Current = (_index << 5) | m;
            _remaining--;
            return true;
        }

        public void Reset()
        {
            _v = _bitArray._bits[0];
            _remaining = _bitArray.Count;
            _index = 0;
            Current = -1;
        }

        readonly object IEnumerator.Current => Current;
        readonly void IDisposable.Dispose() { }
    }

    internal void UnionWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        for (var i = 0; i < _bits.Length; i++)
        {
            _bits[i] |= other._bits[i];
            count += BitOperations.PopCount(_bits[i]);
        }
        Count = count;
    }

    internal void IntersectWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        for (var i = 0; i < _bits.Length; i++)
        {
            _bits[i] &= other._bits[i];
            count += BitOperations.PopCount(_bits[i]);
        }
        Count = count;
    }

    internal void ExceptWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        for (var i = 0; i < _bits.Length; i++)
        {
            _bits[i] &= ~other._bits[i];
            count += BitOperations.PopCount(_bits[i]);
        }
        Count = count;
    }

    internal void SymmetricExceptWith(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        var count = 0;
        for (var i = 0; i < _bits.Length; i++)
        {
            _bits[i] ^= other._bits[i];
            count += BitOperations.PopCount(_bits[i]);
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

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if (bits != otherBits &&
                (bits | otherBits) != bits)
                return false;
        }

        return true;
    }

    [Pure]
    internal bool IsProperSupersetOf(LargeBitArray other)
    {
        Debug.Assert(Length == other.Length);

        for (var i = 0; i < _bits.Length; i++)
        {
            var bits = _bits[i];
            var otherBits = other._bits[i];
            if (bits != otherBits &&
                (bits | otherBits) == bits)
                return false;
        }

        return true;
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