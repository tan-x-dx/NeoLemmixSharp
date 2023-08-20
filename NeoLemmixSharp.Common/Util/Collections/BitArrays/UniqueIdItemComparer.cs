namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class UniqueIdItemComparer<T> :
    IEqualityComparer<T>,
    IEquatable<UniqueIdItemComparer<T>>,
    IComparer<T>,
    ISimpleHasher<T>
    where T : class, IUniqueIdItem<T>
{
    public static UniqueIdItemComparer<T> Instance { get; } = new();

    private UniqueIdItemComparer()
    {
    }

    public bool Equals(T? x, T? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(T obj)
    {
        return HashCode.Combine(obj.Id);
    }

    public int Compare(T? first, T? second)
    {
        if (first is null && second is null) return 0;
        if (first is null) return -1;
        if (second is null) return 1;

        return first.Id.CompareTo(second.Id);
    }

    public bool Equals(UniqueIdItemComparer<T>? other) => other is not null;
    public bool Equals(ISimpleHasher<T>? other) => other is UniqueIdItemComparer<T>;
    public override bool Equals(object? obj) => obj is UniqueIdItemComparer<T>;
    public override int GetHashCode() => typeof(UniqueIdItemComparer<T>).GetHashCode();

    public int NumberOfItems => T.NumberOfItems;

    public int Hash(T item) => item.Id;
    public T Unhash(int hash) => T.AllItems[hash];
}