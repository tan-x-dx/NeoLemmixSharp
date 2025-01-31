using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Identity;

public interface IExtendedEnumType<T> : IIdEquatable<T>
    where T : IExtendedEnumType<T>
{
    [Pure]
    static abstract int NumberOfItems { get; }
    [Pure]
    static abstract ReadOnlySpan<T> AllItems { get; }
}