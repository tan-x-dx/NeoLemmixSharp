namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

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

    public int GetHashCode(T obj)
    {
        return HashCode.Combine(obj.Id);
    }

    public int Compare(T? x, T? y) => IdEquatableItemHelperMethods.Compare(x, y);

    public bool Equals(ExtendedEnumTypeComparer<T>? other) => other is not null;
    public override bool Equals(object? obj) => obj is ExtendedEnumTypeComparer<T>;
    public override int GetHashCode() => typeof(T).GetHashCode();

    public int NumberOfItems => T.NumberOfItems;

    public int Hash(T item) => item.Id;
    public T UnHash(int index) => T.AllItems[index];

    public static LargeSimpleSet<T> LargeSetForType(bool fullSet = false)
    {
        return new LargeSimpleSet<T>(Instance, fullSet);
    }

    public static SmallSimpleSet<T> SmallSetForType(bool fullSet = false)
    {
        if (Instance.NumberOfItems > SmallBitArray.Size)
            throw new InvalidOperationException("Cannot create small set for this type - too many items!");

        return new SmallSimpleSet<T>(Instance, fullSet);
    }
}