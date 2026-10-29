using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public interface IPointerData<T>
    where T : unmanaged, IPointerData<T>
{
    static abstract T Create(nint dataHandle);
    static abstract int SizeInBytes { get; }
}

public static class PointerDataHelper
{
    /// <summary>
    /// Creates an instance of the desired type.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <param name="dataHandle">Represents the pointer the data will utilise.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CreateItem<T>(nint dataHandle)
        where T : unmanaged, IPointerData<T>
    {
        var result = T.Create(dataHandle);

        return result;
    }

    /// <summary>
    /// Creates an instance of the desired type, incrementing the <paramref name="dataHandle"/> parameter by the correct amount.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <param name="dataHandle">Represents the pointer the data will utilise. This value is incremented by the amount specified by the type.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CreateItem<T>(DataHandle dataHandle)
        where T : unmanaged, IPointerData<T>
    {
        var result = T.Create(dataHandle.Ref);

        dataHandle.Increment(T.SizeInBytes);

        return result;
    }
}
