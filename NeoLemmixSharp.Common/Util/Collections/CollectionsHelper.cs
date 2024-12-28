using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Collections;

public static class CollectionsHelper
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GetArrayForSize<T>(int size)
    {
        return size == 0
            ? Array.Empty<T>()
            : new T[size];
    }
}