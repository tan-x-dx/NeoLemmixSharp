using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

/// <summary>
/// Has a fixed length of 32 (Values range 0 - 31).
/// </summary>
public sealed class SmallBitArray : IBitArray
{
    public const int Size = 32;

    private uint _bits;

    public int Count { get; private set; }
    public int Length => Size;

    public SmallBitArray(uint initialBits = 0U)
    {
        _bits = initialBits;
        Count = initialBits == 0 ? 0 : BitOperations.PopCount(initialBits);
    }

    private SmallBitArray(uint bits, int count)
    {
        _bits = bits;
        Count = count;
    }

    [Pure]
    public bool GetBit(int index)
    {
        return (_bits & (1U << index)) != 0U;
    }

    public bool SetBit(int index)
    {
        var oldValue = _bits;
        _bits |= 1U << index;
        var delta = (_bits ^ oldValue) >> index;
        Count += (int)delta;
        return delta != 0U;
    }

    public bool ClearBit(int index)
    {
        var oldValue = _bits;
        _bits &= ~(1U << index);
        var delta = (_bits ^ oldValue) >> index;
        Count -= (int)delta;
        return delta != 0U;
    }

    public bool ToggleBit(int index)
    {
        var oldValue = _bits;
        _bits ^= 1U << index;
        bool result;
        if (_bits > oldValue)
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
    public SmallBitArray Clone() => new(_bits, Count);

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<int>
    {
        private readonly SmallBitArray _bitField;
        private uint _v;

        public int Current { get; private set; }

        public Enumerator(SmallBitArray bitField)
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
            Current = -1;
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }

    internal void UnionWith(SmallBitArray other)
    {
        _bits |= other._bits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void IntersectWith(SmallBitArray other)
    {
        _bits &= other._bits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void ExceptWith(SmallBitArray other)
    {
        _bits &= ~other._bits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void SymmetricExceptWith(SmallBitArray other)
    {
        _bits ^= other._bits;
        Count = BitOperations.PopCount(_bits);
    }

    [Pure]
    internal bool IsSubsetOf(SmallBitArray other)
    {
        return (_bits | other._bits) == other._bits;
    }

    [Pure]
    internal bool IsSupersetOf(SmallBitArray other)
    {
        return (_bits | other._bits) == _bits;
    }

    [Pure]
    internal bool IsProperSubsetOf(SmallBitArray other)
    {
        return _bits != other._bits &&
               (_bits | other._bits) == other._bits;
    }

    [Pure]
    internal bool IsProperSupersetOf(SmallBitArray other)
    {
        return _bits != other._bits &&
               (_bits | other._bits) == _bits;
    }

    [Pure]
    internal bool Overlaps(SmallBitArray other)
    {
        return (_bits & other._bits) != 0;
    }

    [Pure]
    internal bool SetEquals(SmallBitArray other)
    {
        return _bits == other._bits;
    }
}
