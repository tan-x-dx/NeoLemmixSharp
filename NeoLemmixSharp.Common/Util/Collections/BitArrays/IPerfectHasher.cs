using System.Diagnostics.CodeAnalysis;
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

        var hashX = Hash(x);
        var hashY = Hash(y);

        var gt = (hashX > hashY) ? 1 : 0;
        var lt = (hashX < hashY) ? 1 : 0;
        return gt - lt;
    }
}

public interface IBitBufferCreator<TBuffer, T> : IPerfectHasher<T>
    where TBuffer : struct, IBitBuffer
    where T : notnull
{
    void CreateBitBuffer(out TBuffer buffer);
}

public interface IEnumIdentifierHelper<TBuffer, TEnum> : IBitBufferCreator<TBuffer, TEnum>
    where TEnum : unmanaged, Enum
    where TBuffer : struct, IBitBuffer
{
    static abstract TEnum GetEnumValue(uint rawValue);
}

public static class PerfectHasherHelperMethods
{
    public static void AssertUniqueIds<TPerfectHasher, T>(
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

        foreach (var item in items)
        {
            var id = hasher.Hash(item);

            minId = Math.Min(minId, id);
            maxId = Math.Max(maxId, id);

            if (!allItemIds.Add(id))
            {
                ThrowDuplicatedIdsException<T>(id);
            }
        }

        if (minId != 0 || maxId != items.Length - 1)
        {
            ThrowInvalidIdsException<T>(items.Length - 1);
        }
    }

    [DoesNotReturn]
    private static void ThrowDuplicatedIdsException<T>(int id)
    {
        var typeName = typeof(T).Name;
        throw new Exception($"Duplicated {typeName} ID: {id}");
    }

    [DoesNotReturn]
    private static void ThrowInvalidIdsException<T>(int expectedMaxId)
    {
        var typeName = typeof(T).Name;
        throw new Exception($"{typeName} ids do not span a full set of values from 0 - {expectedMaxId}");
    }
}
