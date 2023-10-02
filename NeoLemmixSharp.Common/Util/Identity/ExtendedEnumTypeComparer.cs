using NeoLemmixSharp.Common.Util.Collections;

namespace NeoLemmixSharp.Common.Util.Identity;

public sealed class ExtendedEnumTypeComparer<T> :
    IEqualityComparer<T>,
    IEquatable<ExtendedEnumTypeComparer<T>>,
    IComparer<T>,
    ISimpleHasher<T>
    where T : class, IExtendedEnumType<T>
{
    public static ExtendedEnumTypeComparer<T> Instance { get; } = new();

    private ExtendedEnumTypeComparer()
    {
    }

    public bool Equals(T? x, T? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(T obj) => HashCode.Combine(obj);

    public int Compare(T? x, T? y) => IdEquatableItemHelperMethods.Compare(x, y);

    public bool Equals(ExtendedEnumTypeComparer<T>? other) => other is not null;
    public override bool Equals(object? obj) => obj is ExtendedEnumTypeComparer<T>;
    public override int GetHashCode() => typeof(T).GetHashCode();

    public int NumberOfItems => T.NumberOfItems;

    public int Hash(T item) => item.Id;
    public T UnHash(int index) => T.AllItems[index];

    public static SimpleSet<T> CreateSimpleSet()
    {
        return new SimpleSet<T>(Instance);
    }
}