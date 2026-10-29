namespace NeoLemmixSharp.Common.Util;

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

    internal nint Ref => _dataHandleRef;

    internal void Increment(int sizeInBytes)
    {
        _dataHandleRef += sizeInBytes;
    }
}
