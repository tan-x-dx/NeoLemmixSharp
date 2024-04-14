namespace NeoLemmixSharp.Common.Util.Collections;

public static class CollectionsHelper
{
    public static T[] GetArrayForSize<T>(int size)
    {
        if (size == 0)
            return Array.Empty<T>();

        return new T[size];
    }
}