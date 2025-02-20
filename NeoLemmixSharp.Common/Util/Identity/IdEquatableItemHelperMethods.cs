using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Identity;

public static class IdEquatableItemHelperMethods
{
    public static void ValidateUniqueIds<T>(ReadOnlySpan<T> items)
        where T : class, IIdEquatable<T>
    {
        if (items.Length == 0)
            return;

        var minId = int.MaxValue;
        var maxId = int.MinValue;
        var allItemIds = new HashSet<int>(items.Length);
        string? typeName;

        foreach (var item in items)
        {
            var id = item.Id;

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

    [Pure]
    public static int Compare<T>(T? x, T? y)
        where T : class, IIdEquatable<T>
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return x.Id.CompareTo(y.Id);
    }
}