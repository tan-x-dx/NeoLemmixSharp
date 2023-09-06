namespace NeoLemmixSharp.Common.Util.PositionTracking;

public enum ChunkSizeType
{
    ChunkSize32 = 5,
    ChunkSize64 = 6,
}

internal static class ChunkSizeTypeHelpers
{
    public static int ChunkSizeBitShiftFromType(this ChunkSizeType chunkSizeType) => chunkSizeType switch
    {
        ChunkSizeType.ChunkSize32 => 5,
        ChunkSizeType.ChunkSize64 => 6,

        _ => throw new ArgumentOutOfRangeException(nameof(chunkSizeType), chunkSizeType, "Unknown chunk size type")
    };
}