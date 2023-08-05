namespace NeoLemmixSharp.Common.Util;

public interface IUniqueIdItem
{
    int Id { get; }
}

public sealed class UniqueIdItemComparer<T> : IEqualityComparer<T>, IEquatable<UniqueIdItemComparer<T>>, IComparer<T>
    where T : class, IUniqueIdItem
{
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
        var firstId = first?.Id ?? -1;
        var secondId = second?.Id ?? -1;

        return firstId.CompareTo(secondId);
    }

    public bool Equals(UniqueIdItemComparer<T>? other) => other != null;
    public override bool Equals(object? obj) => obj is UniqueIdItemComparer<T>;
    public override int GetHashCode() => nameof(UniqueIdItemComparer<T>).GetHashCode();
}

public static class UniqueIdItemValidatorMethods
{
    public static void ValidateUniqueIds<T>(this T[] items)
        where T : class, IUniqueIdItem
    {
        var ids = items
            .Select(i => i.Id)
            .ToList();

        var numberOfUniqueIds = ids.Distinct().Count();

        if (numberOfUniqueIds != items.Length)
        {
            var typeName = typeof(T).Name;
            var idsString = string.Join(',', ids.OrderBy(i => i));

            throw new Exception($"Duplicated {typeName} ID: {idsString}");
        }

        var minActionId = ids.Min();
        var maxActionId = ids.Max();

        if (minActionId != 0 || maxActionId != items.Length - 1)
        {
            var typeName = typeof(T).Name;
            throw new Exception($"{typeName} ids do not span a full set of values from 0 - {items.Length - 1}");
        }
    }
}