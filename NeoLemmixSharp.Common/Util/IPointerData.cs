using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public interface IPointerData<T>
    where T : unmanaged, IPointerData<T>
{
    static abstract T Create(nint dataRef);
    static abstract int SizeInBytes { get; }
}

public static class PointerDataHelper
{
    /// <summary>
    /// Creates an instance of the desired type, incrementing the <paramref name="dataRef"/> parameter by the correct amount.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <param name="dataRef">Represents the pointer the data will utilise. This value is incremented by the amount specified by the type.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CreateItem<T>(ref nint dataRef)
        where T : unmanaged, IPointerData<T>
    {
        var result = T.Create(dataRef);

        dataRef += T.SizeInBytes;

        return result;
    }
}
