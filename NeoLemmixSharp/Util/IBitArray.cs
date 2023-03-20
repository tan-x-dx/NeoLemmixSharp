using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoLemmixSharp.Util;

public interface IBitArray : ICollection<int>, IReadOnlyCollection<int>, ICloneable
{
    public int Length { get; }
    public bool AnyBitsSet { get; }
    
    public bool GetBit(int index);
    public bool SetBit(int index);
    public bool ClearBit(int index);

    void ICollection<int>.Add(int i) => SetBit(i);
    bool ICollection<int>.Contains(int i) => GetBit(i);
    bool ICollection<int>.Remove(int i) => ClearBit(i);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<int>.IsReadOnly => false;
}