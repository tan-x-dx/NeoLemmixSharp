namespace NeoLemmixSharp.Common.Util.PositionTracking;

internal interface IChunkPositionUser
{
    void Clear();
    void UseChunkPosition(ChunkPosition chunkPosition);
}