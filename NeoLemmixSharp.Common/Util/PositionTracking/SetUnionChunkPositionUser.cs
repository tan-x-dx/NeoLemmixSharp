﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class SetUnionChunkPositionUser<T> : IChunkPositionUser
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly Dictionary<ChunkPosition, LargeSimpleSet<T>> _itemChunksLookup;

    public LargeSimpleSet<T> SetToUnionWith { private get; set; }

    public SetUnionChunkPositionUser(PositionHelper<T> manager)
    {
        _itemChunksLookup = manager.ItemChunks;
    }

    public void UseChunkPosition(ChunkPosition chunkPosition)
    {
        if (_itemChunksLookup.TryGetValue(chunkPosition, out var itemChunk))
        {
            SetToUnionWith.UnionWith(itemChunk);
        }
    }
}