using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoLemmixSharp.Util;

public interface IBitArray : ICollection<int>, IReadOnlyCollection<int>, ICloneable
{
    int Length { get; }
    bool AnyBitsSet { get; }

    bool GetBit(int index);
    bool SetBit(int index);
    bool ClearBit(int index);

    void ICollection<int>.Add(int i) => SetBit(i);
    bool ICollection<int>.Contains(int i) => GetBit(i);
    bool ICollection<int>.Remove(int i) => ClearBit(i);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<int>.IsReadOnly => false;
}