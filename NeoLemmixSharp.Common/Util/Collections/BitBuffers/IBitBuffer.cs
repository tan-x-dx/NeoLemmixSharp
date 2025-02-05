using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitBuffers;

public interface IBitBuffer
{
    [Pure]
    int Size { get; }

    [Pure]
    Span<uint> AsSpan();
    [Pure]
    ReadOnlySpan<uint> AsReadOnlySpan();
}

public interface IBitBufferCreator<TBuffer>
    where TBuffer : struct, IBitBuffer
{
    void CreateBitBuffer(out TBuffer buffer);
}
