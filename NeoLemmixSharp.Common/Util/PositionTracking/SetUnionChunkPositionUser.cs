using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class SetUnionChunkPositionUser<T> : IChunkPositionUser
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly LargeSimpleSet<T> _setToUnionWith;
    private readonly Dictionary<ChunkPosition, LargeSimpleSet<T>> _itemChunkLookup;

    public SetUnionChunkPositionUser(
        ISimpleHasher<T> hasher,
        Dictionary<ChunkPosition, LargeSimpleSet<T>> itemChunkLookup)
    {
        _setToUnionWith = new LargeSimpleSet<T>(hasher);
        _itemChunkLookup = itemChunkLookup;
    }

    public void UseChunkPosition(ChunkPosition chunkPosition)
    {
        ref var itemChunk = ref CollectionsMarshal.GetValueRefOrNullRef(_itemChunkLookup, chunkPosition);
        if (!Unsafe.IsNullRef(ref itemChunk))
        {
            _setToUnionWith.UnionWith(itemChunk);
        }
    }

    public void Clear()
    {
        _setToUnionWith.Clear();
    }

    public LargeSimpleSet<T> GetSet() => _setToUnionWith;
}