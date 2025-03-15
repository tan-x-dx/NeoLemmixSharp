using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IPerfectHasher<T> : IComparer<T>
    where T : notnull
{
    [Pure]
    int NumberOfItems { get; }

    [Pure]
    int Hash(T item);
    [Pure]
    T UnHash(int index);

    int IComparer<T>.Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return Hash(x) - Hash(y);
    }
}

public static class PerfectHasherHelperMethods
{
    public static void ValidateUniqueIds<TPerfectHasher, T>(
        this TPerfectHasher hasher,
        ReadOnlySpan<T> items)
        where TPerfectHasher : IPerfectHasher<T>
        where T : notnull
    {
        if (items.Length == 0)
            return;

        var minId = int.MaxValue;
        var maxId = int.MinValue;
        var allItemIds = new HashSet<int>(items.Length);
        string? typeName;

        foreach (var item in items)
        {
            var id = hasher.Hash(item);

            minId = Math.Min(minId, id);
            maxId = Math.Max(maxId, id);

            if (!allItemIds.Add(id))
            {
                typeName = typeof(T).Name;
                throw new Exception($"Duplicated {typeName} ID: {id}");
            }
        }

        if (minId != 0 || maxId != items.Length - 1)
        {
            typeName = typeof(T).Name;
            throw new Exception($"{typeName} ids do not span a full set of values from 0 - {items.Length - 1}");
        }
    }
}