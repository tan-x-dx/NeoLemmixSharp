namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public static class SimpleSetHelpers
{
    public static LargeSimpleSet<T> LargeSetForUniqueItemType<T>(bool fullSet = false)
        where T : class, IUniqueIdItem<T>
    {
        var hasher = UniqueIdItemComparer<T>.Instance;
        
        return new LargeSimpleSet<T>(hasher, fullSet);
    }

    public static SmallSimpleSet<T> SmallSetForUniqueItemType<T>(bool fullSet = false)
        where T : class, IUniqueIdItem<T>
    {
        var hasher = UniqueIdItemComparer<T>.Instance;

        if (hasher.NumberOfItems > SmallBitArray.Size)
            throw new InvalidOperationException("Cannot create small set for this type - too many items!");

        return new SmallSimpleSet<T>(hasher, fullSet);
    }
}