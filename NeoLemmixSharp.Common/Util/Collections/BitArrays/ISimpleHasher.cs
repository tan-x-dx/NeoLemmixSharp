﻿namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface ISimpleHasher<T>
{
    int NumberOfItems { get; }

    int Hash(T item);
    T UnHash(int index);
}