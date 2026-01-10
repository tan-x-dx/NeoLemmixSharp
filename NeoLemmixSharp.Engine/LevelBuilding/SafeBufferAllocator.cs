using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public readonly struct SafeBufferAllocator
{
    private readonly List<RawArray> _rawArrays = [];

    public SafeBufferAllocator()
    {
    }

    public RawArray AllocateRawArray(int length)
    {
        var newRawArray = new RawArray(length);
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
