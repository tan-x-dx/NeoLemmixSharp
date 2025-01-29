using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Identity;

public interface IIdEquatable<T> : IEquatable<T>
    where T : IIdEquatable<T>
{
    [Pure]
    int Id { get; }

    [Pure]
    static abstract bool operator ==(T left, T right);
    [Pure]
    static abstract bool operator !=(T left, T right);
}