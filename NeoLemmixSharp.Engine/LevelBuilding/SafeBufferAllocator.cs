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
        return AllocateRawArray(numberOfItems * T.SizeInBytes);
    }

    public void DeallocateAllBuffers()
    {
        foreach (var rawArray in _rawArrays)
        {
            rawArray.Dispose();
        }
    }
}
