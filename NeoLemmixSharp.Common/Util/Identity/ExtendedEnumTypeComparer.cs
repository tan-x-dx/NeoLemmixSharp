using NeoLemmixSharp.Common.Util.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Identity;

public sealed class ExtendedEnumTypeComparer<T> :
    IEqualityComparer<T>,
    IEquatable<ExtendedEnumTypeComparer<T>>,
    IComparer<T>,
    IPerfectHasher<T>
    where T : class, IExtendedEnumType<T>
{
    private static readonly ExtendedEnumTypeComparer<T> Instance = new();

    private ExtendedEnumTypeComparer()
    {
    }

    [Pure]
    public bool Equals(T? x, T? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        return x.Id == y.Id;
    }

    [Pure]
    public int GetHashCode(T obj) => 2965019 * obj.Id +
                                     5477821;

    [Pure]
    public int Compare(T? x, T? y) => IdEquatableItemHelperMethods.Compare(x, y);

    [Pure]
    public bool Equals(ExtendedEnumTypeComparer<T>? other) => other is not null;
    [Pure]
    public override bool Equals(object? obj) => obj is ExtendedEnumTypeComparer<T>;
    [Pure]
    public override int GetHashCode() => typeof(T).GetHashCode();

    [Pure]
    public static bool operator ==(ExtendedEnumTypeComparer<T> left, ExtendedEnumTypeComparer<T> right) => true;
    [Pure]
    public static bool operator !=(ExtendedEnumTypeComparer<T> left, ExtendedEnumTypeComparer<T> right) => false;

    [Pure]
    public int NumberOfItems => T.NumberOfItems;

    [Pure]
    public int Hash(T item) => item.Id;
    [Pure]
    public T UnHash(int index) => T.AllItems[index];

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleDictionary<ExtendedEnumTypeComparer<T>, T, TValue> CreateSimpleDictionary<TValue>() => new(Instance);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleSet<ExtendedEnumTypeComparer<T>, T> CreateSimpleSet(bool fullSet = false) => new(Instance, fullSet);
}