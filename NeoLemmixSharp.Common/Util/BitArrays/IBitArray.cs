using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.BitArrays;

public interface IBitArray : ICollection<int>, IReadOnlyCollection<int>, ICloneable
{
    int Length { get; }
    new int Count { get; }

    [Pure]
    bool GetBit(int index);
    bool SetBit(int index);
    bool ClearBit(int index);

    void ICollection<int>.Add(int i)
    {
        if (i < 0 || i >= Length)
            throw new ArgumentOutOfRangeException(nameof(i), i, $"Can only add items if they are between 0 and {Length - 1}");

        SetBit(i);
    }

    bool ICollection<int>.Contains(int i) => i >= 0 && i < Length && GetBit(i);
    bool ICollection<int>.Remove(int i) => i >= 0 && i < Length && ClearBit(i);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<int>.IsReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IBitArray GetBestFitForSize(int size) => size > IntBasedBitArray.Size
        ? new ArrayBasedBitArray(size)
        : new IntBasedBitArray();
}