namespace NeoLemmixSharp.Common.Util.Collections;

public interface IPerfectHasher<T>
{
    int NumberOfItems { get; }

    int Hash(T item);
    T UnHash(int index);
}