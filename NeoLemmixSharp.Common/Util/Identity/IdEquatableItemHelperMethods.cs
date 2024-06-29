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
        var allItemIds = new HashSet<int>(items.Length, new IntEqualityComparer());

        foreach (var item in items)
        {
            var id = item.Id;

            minId = Math.Min(minId, id);
            maxId = Math.Max(maxId, id);

            if (!allItemIds.Add(id))
            {
                var typeName = typeof(T).Name;

                throw new Exception($"Duplicated {typeName} ID: {id}");
            }
        }

        if (minId != 0 || maxId != items.Length - 1)
        {
            var typeName = typeof(T).Name;
            throw new Exception($"{typeName} ids do not span a full set of values from 0 - {items.Length - 1}");
        }
    }

    public static int Compare<T>(T? x, T? y)
        where T : class, IIdEquatable<T>
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var xId = x.Id;
        var yId = y.Id;

        if (xId < yId) return -1;
        return xId > yId ? 1 : 0;
    }

    public static int GetHashCode<T>(T obj)
        where T : class, IIdEquatable<T>
    {
        return 2965019 * obj.Id +
               5477821;
    }

    private sealed class IntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;
        public int GetHashCode(int n) => n;
    }
}