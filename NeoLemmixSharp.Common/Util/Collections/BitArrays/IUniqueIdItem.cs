namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public interface IIdEquatable<T> : IEquatable<T>
    where T : class, IIdEquatable<T>
{
    int Id { get; }

    static abstract bool operator ==(T left, T right);
    static abstract bool operator !=(T left, T right);
}

public interface IUniqueIdItem<T> : IIdEquatable<T>
    where T : class, IUniqueIdItem<T>
{
    static abstract int NumberOfItems { get; }
    static abstract ReadOnlySpan<T> AllItems { get; }
}

public static class IdEquatableItemHelperMethods
{
    public static void ValidateUniqueIds<T>(this ICollection<T> items)
        where T : class, IIdEquatable<T>
    {
        if (items.Count == 0)
            return;

        var ids = items
            .Select(i => i.Id)
            .ToList();

        var numberOfUniqueIds = ids.Distinct().Count();

        if (numberOfUniqueIds != items.Count)
        {
            var typeName = typeof(T).Name;
            var idsString = string.Join(',', ids.OrderBy(i => i));

            throw new Exception($"Duplicated {typeName} ID: {idsString}");
        }

        var minActionId = ids.Min();
        var maxActionId = ids.Max();

        if (minActionId != 0 || maxActionId != items.Count - 1)
        {
            var typeName = typeof(T).Name;
            throw new Exception($"{typeName} ids do not span a full set of values from 0 - {items.Count - 1}");
        }
    }

    public static int Compare<T>(T? x, T? y)
        where T : class, IIdEquatable<T>
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return x.Id - y.Id;
    }
}