namespace NeoLemmixSharp.Common.Util.Collections;

public interface IPerfectHasher<T>
    where T : notnull
{
    int NumberOfItems { get; }

    int Hash(T item);
    T UnHash(int index);
}