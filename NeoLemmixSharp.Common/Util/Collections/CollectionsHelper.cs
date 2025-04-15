namespace NeoLemmixSharp.Common.Util.Collections;

public static class CollectionsHelper
{
    public static T[] GetArrayForSize<T>(int size)
    {
        return size == 0
            ? Array.Empty<T>()
            : new T[size];
    }
}