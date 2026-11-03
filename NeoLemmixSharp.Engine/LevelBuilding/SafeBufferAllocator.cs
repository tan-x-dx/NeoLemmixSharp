using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Rewind;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly struct SafeBufferAllocator
{
    private readonly List<RawArray> _rawArrays = [];

    public SafeBufferAllocator()
    {
    }

    public RawArray AllocateRawArray(int numberOfBytesToAllocate)
    {
        var newRawArray = new RawArray(numberOfBytesToAllocate);
        newRawArray.AsSpan().Clear();
        _rawArrays.Add(newRawArray);
        return newRawArray;
    }

    public RawArray AllocateRawArray<T>(int numberOfItems)
        where T : unmanaged, IPointerData<T>
    {
        var newRawArray = new RawArray(numberOfItems * T.SizeInBytes);
        newRawArray.AsSpan().Clear();
        _rawArrays.Add(newRawArray);
        return newRawArray;
    }

    public void DeallocateAllBuffers()
    {
        foreach (var rawArray in _rawArrays)
        {
            rawArray.Dispose();
        }
    }
}
