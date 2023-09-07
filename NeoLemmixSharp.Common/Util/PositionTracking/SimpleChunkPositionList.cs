namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class SimpleChunkPositionList : IChunkPositionUser
{
    private const int InitialArraySize = 4;
    private const int ArrayGrowthAmount = 4;

    private ChunkPosition[] _chunkPositions = new ChunkPosition[InitialArraySize];

    private int _count;

    public void UseChunkPosition(ChunkPosition chunkPosition)
    {
        var currentArrayLength = _chunkPositions.Length;

        if (_count == currentArrayLength)
        {
            var newArray = new ChunkPosition[currentArrayLength + ArrayGrowthAmount];
            Array.Copy(_chunkPositions, newArray, currentArrayLength);
            _chunkPositions = newArray;
        }

        _chunkPositions[_count++] = chunkPosition;
    }

    public void Clear()
    {
        _count = 0;
    }

    public ReadOnlySpan<ChunkPosition> GetSpan() => new(_chunkPositions, 0, _count);
}