using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class SetUnionChunkIndexUser<T> : IChunkIndexUser
    where T : class, IIdEquatable<T>, IRectangularBounds
{
    private readonly LargeSimpleSet<T>?[] _itemChunks;

    public LargeSimpleSet<T> SetToUnionWith { private get; set; }

    public SetUnionChunkIndexUser(PositionHelper<T> manager)
    {
        _itemChunks = manager.ItemChunks;
    }

    public void UseChunkIndex(int index)
    {
        var itemSet = _itemChunks[index];
        if (itemSet is not null)
        {
            SetToUnionWith.UnionWith(itemSet);
        }
    }
}