﻿using System.Collections;
using System.Diagnostics;
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
        Debug.Assert(index >= 0 && index < Size);

        return (_bits & (1U << index)) != 0U;
    }

    public bool SetBit(int index)
    {
        Debug.Assert(index >= 0 && index < Size);

        var oldValue = _bits;
        _bits |= 1U << index;
        var delta = (_bits ^ oldValue) >> index;
        Count += (int)delta;
        return delta != 0U;
    }

    public bool ClearBit(int index)
    {
        Debug.Assert(index >= 0 && index < Size);

        var oldValue = _bits;
        _bits &= ~(1U << index);
        var delta = (_bits ^ oldValue) >> index;
        Count -= (int)delta;
        return delta != 0U;
    }

    public bool ToggleBit(int index)
    {
        Debug.Assert(index >= 0 && index < Size);

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

    [Pure]
    internal uint GetRawBits() => _bits;
    [Pure]
    public uint GetBitMask(uint mask) => mask & _bits;

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

        public readonly bool IsEmpty => _v == 0;

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

        readonly object IEnumerator.Current => Current;
        readonly void IDisposable.Dispose() { }
    }

    internal void UnionWith(uint otherBits)
    {
        _bits |= otherBits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void IntersectWith(uint otherBits)
    {
        _bits &= otherBits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void ExceptWith(uint otherBits)
    {
        _bits &= ~otherBits;
        Count = BitOperations.PopCount(_bits);
    }

    internal void SymmetricExceptWith(uint otherBits)
    {
        _bits ^= otherBits;
        Count = BitOperations.PopCount(_bits);
    }

    [Pure]
    internal bool IsSubsetOf(uint otherBits)
    {
        return (_bits | otherBits) == otherBits;
    }

    [Pure]
    internal bool IsSupersetOf(uint otherBits)
    {
        return (_bits | otherBits) == _bits;
    }

    [Pure]
    internal bool IsProperSubsetOf(uint otherBits)
    {
        return _bits != otherBits &&
               (_bits | otherBits) == otherBits;
    }

    [Pure]
    internal bool IsProperSupersetOf(uint otherBits)
    {
        return _bits != otherBits &&
               (_bits | otherBits) == _bits;
    }

    [Pure]
    internal bool Overlaps(uint otherBits)
    {
        return (_bits & otherBits) != 0;
    }

    [Pure]
    internal bool SetEquals(uint otherBits)
    {
        return _bits == otherBits;
    }
}
