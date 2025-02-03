namespace NeoLemmixSharp.Common.Util.Collections.BitBuffers;

public interface ISpannable
{
    int Size { get; }

    Span<uint> AsSpan();
    ReadOnlySpan<uint> AsReadOnlySpan();
}
