using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IBitArray : ICollection<int>, IReadOnlyCollection<int>, ICloneable
{
    int Length { get; }
    new int Count { get; }

    /// <summary>
    /// Tests if a specific bit is set
    /// </summary>
    /// <param name="index">The bit to query</param>
    /// <returns>True if the specified bit is set</returns>
    [Pure]
    bool GetBit(int index);
    /// <summary>
    /// Sets a bit to 1. Returns true if a change has occurred -
    /// i.e. if the bit was previously 0
    /// </summary>
    /// <param name="index">The bit to set</param>
    /// <returns>True if the operation changed the value of the bit, false if the bit was previously set</returns>
    bool SetBit(int index);
    /// <summary>
    /// Sets a bit to 0. Returns true if a change has occurred -
    /// i.e. if the bit was previously 1
    /// </summary>
    /// <param name="index">The bit to clear</param>
    /// <returns>True if the operation changed the value of the bit, false if the bit was previously clear</returns>
    bool ClearBit(int index);
    /// <summary>
    /// Toggles the value of a bit. Returns the new value after toggling
    /// </summary>
    /// <param name="index">The bit to toggle</param>
    /// <returns>The bool equivalent of the binary value (0 or 1) of the bit after toggling</returns>
    bool ToggleBit(int index);

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
}