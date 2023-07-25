namespace NeoLemmixSharp.Common.Util;

public interface IUniqueIdItem
{
    int Id { get; }
}

public sealed class UniqueIdItemEqualityComparer<T> : IEqualityComparer<T>, IEquatable<UniqueIdItemEqualityComparer<T>>
    where T : class, IUniqueIdItem
{
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(T obj)
    {
        return HashCode.Combine(obj.Id);
    }

    public bool Equals(UniqueIdItemEqualityComparer<T>? other) => other != null;
    public override bool Equals(object? obj) => obj is UniqueIdItemEqualityComparer<T>;
    public override int GetHashCode() => nameof(UniqueIdItemEqualityComparer<T>).GetHashCode();
}

public static class ListValidatorMethods
{
    public static void ValidateUniqueIds<T>(ICollection<T> items)
        where T : class, IUniqueIdItem
    {
        var ids = items
            .Select(la => la.Id)
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
}