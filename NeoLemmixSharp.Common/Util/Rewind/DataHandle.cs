using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.Rewind;

public sealed class DataHandle
{
    private nint _dataHandleRef;

    public DataHandle(RawArray rawArray)
    {
        _dataHandleRef = rawArray.Handle;
    }

    public DataHandle(nint handle)
    {
        _dataHandleRef = handle;
    }

    /// <summary>
    /// Creates an instance of the desired type, incrementing the <see cref="DataHandle"/> by the correct amount.
    /// </summary>
    /// <typeparam name="T">The type to create.</typeparam>
    /// <returns>A newly created instance of the desired type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CreateItem<T>()
        where T : unmanaged, IPointerData<T>
    {
        var dataHandleRef = _dataHandleRef;
        var result = T.Create(dataHandleRef);

        dataHandleRef += T.SizeInBytes;
        _dataHandleRef = dataHandleRef;

        return result;
    }
}
