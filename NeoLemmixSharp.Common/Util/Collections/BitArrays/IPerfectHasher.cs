using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IPerfectHasher<T>
    where T : notnull
{
    [Pure]
    int NumberOfItems { get; }

    [Pure]
    int Hash(T item);
    [Pure]
    T UnHash(int index);
}

public interface IBitBufferCreator<TBuffer, T> : IPerfectHasher<T>
    where TBuffer : struct, IBitBuffer
    where T : notnull
{
    void CreateBitBuffer(out TBuffer buffer);
}
