using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitBuffers;

public interface ISpannable
{
    [Pure]
    int Size { get; }

    [Pure]
    Span<uint> AsSpan();
    [Pure]
    ReadOnlySpan<uint> AsReadOnlySpan();
}
