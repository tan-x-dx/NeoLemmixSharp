using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections;

public interface IPerfectHasher<T>
    where T : notnull
{
    [Pure]
    int NumberOfItems { get; }

    [Pure]
    int Hash(T item);
    [Pure]
    T UnHash(int index);
}
