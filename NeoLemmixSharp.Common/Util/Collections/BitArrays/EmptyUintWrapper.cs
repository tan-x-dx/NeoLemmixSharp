using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class EmptyUintWrapper : IUintWrapper
{
    public static readonly EmptyUintWrapper Instance = new();

    public int Size => 0;

    private EmptyUintWrapper()
    {
    }

    [Pure]
    public Span<uint> AsSpan() => Span<uint>.Empty;

    [Pure]
    public ReadOnlySpan<uint> AsReadOnlySpan() => ReadOnlySpan<uint>.Empty;
}