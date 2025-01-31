﻿using NeoLemmixSharp.Common.Util.Collections;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Identity;

public readonly struct ExtendedEnumTypeComparer<T> :
    IEqualityComparer<T>,
    IEquatable<ExtendedEnumTypeComparer<T>>,
    IComparer<T>,
    IPerfectHasher<T>
    where T : class, IExtendedEnumType<T>
{
    [Pure]
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id == y.Id;
    }

    [Pure]
    public int GetHashCode(T obj) => 2965019 * obj.Id +
                                     5477821;

    [Pure]
    public int Compare(T? x, T? y) => IdEquatableItemHelperMethods.Compare(x, y);

    [Pure]
    public bool Equals(ExtendedEnumTypeComparer<T> other) => true;
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
    public static SimpleSet<ExtendedEnumTypeComparer<T>, T> CreateSimpleSet(bool fullSet = false) => new(new ExtendedEnumTypeComparer<T>(), fullSet);
    [Pure]
    public static SimpleDictionary<ExtendedEnumTypeComparer<T>, T, TValue> CreateSimpleDictionary<TValue>() => new(new ExtendedEnumTypeComparer<T>());
}