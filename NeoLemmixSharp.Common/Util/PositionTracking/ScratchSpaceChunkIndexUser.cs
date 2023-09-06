using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal sealed class ScratchSpaceChunkIndexUser : IChunkIndexUser
{
    private readonly LargeBitArray _indicesOfItemChunksScratchSpace;

    public ScratchSpaceChunkIndexUser(LargeBitArray indicesOfItemChunksScratchSpace)
    {
        _indicesOfItemChunksScratchSpace = indicesOfItemChunksScratchSpace;
    }

    public void UseChunkIndex(int index)
    {
        _indicesOfItemChunksScratchSpace.SetBit(index);
    }
}