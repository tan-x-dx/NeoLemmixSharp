using System.Collections;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

/// <summary>
/// Has a fixed length of 32 (Values range 0 - 31).
/// </summary>
public sealed class SmallBitArray : IBitArray
{
    public const int Size = 8 * sizeof(uint);

    private uint _bits;

    public int Count { get; private set; }
    public int Length => Size;

    public SmallBitArray(uint initialBits = 0U)
    {
        _bits = initialBits;
        Count = initialBits == 0U ? 0 : BitOperations.PopCount(_bits);
    }

    private SmallBitArray(uint bits, int count)
    {
        _bits = bits;
        Count = count;
    }

    [Pure]
    public bool GetBit(int index)
    {
        return (_bits & 1U << index) != 0U;
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
        }

        object IEnumerator.Current => Current;
        void IDisposable.Dispose() { }
    }
}
