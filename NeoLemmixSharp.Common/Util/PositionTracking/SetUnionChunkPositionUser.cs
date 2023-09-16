using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class SetUnionChunkPositionUser<T> : IChunkPositionUser
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly LargeSimpleSet<T> _setToUnionWith;
    private readonly Dictionary<ChunkPosition, LargeSimpleSet<T>> _itemChunksLookup;

    public SetUnionChunkPositionUser(
        ISimpleHasher<T> hasher,
        Dictionary<ChunkPosition, LargeSimpleSet<T>> itemChunksLookup)
    {
        _setToUnionWith = new LargeSimpleSet<T>(hasher);
        _itemChunksLookup = itemChunksLookup;
    }

    public void UseChunkPosition(ChunkPosition chunkPosition)
    {
        if (_itemChunksLookup.TryGetValue(chunkPosition, out var itemChunk))
        {
            _setToUnionWith.UnionWith(itemChunk);
        }
    }

    public void Clear()
    {
        _setToUnionWith.Clear();
    }

    public LargeSimpleSet<T>.Enumerator GetEnumerator() => _setToUnionWith.GetEnumerator();
}