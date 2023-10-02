using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IUintWrapper
{
    /// <summary>
    /// The footprint of this uint wrapper - how many uints it logically represents
    /// </summary>
    int Size { get; }

    [Pure]
    Span<uint> AsSpan();
    [Pure]
    ReadOnlySpan<uint> AsReadOnlySpan();
}