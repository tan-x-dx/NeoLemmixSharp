namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface ISimpleHasher<T>
{
    int NumberOfItems { get; }

    int Hash(T item);
    T UnHash(int index);

    internal static ISimpleHasher<T> Empty => new EmptyHasher();

    private sealed class EmptyHasher : ISimpleHasher<T>
    {
        public int NumberOfItems => 0;
        public int Hash(T item) => 0;
        public T UnHash(int index) => throw new NotSupportedException("Empty hasher cannot be used in this manner!");
    }
}