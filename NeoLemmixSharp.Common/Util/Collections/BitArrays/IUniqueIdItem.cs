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

public static class UniqueIdItemValidatorMethods
{
    public static void ValidateUniqueIds<T>(this ICollection<T> items)
        where T : class, IIdEquatable<T>
    {
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
}