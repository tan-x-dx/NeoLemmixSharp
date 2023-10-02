namespace NeoLemmixSharp.Common.Util.Collections;

public interface ISimpleHasher<T>
{
    int NumberOfItems { get; }

    int Hash(T item);
    T UnHash(int index);
}