using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public enum ChunkSizeType
{
    ChunkSize16 = 4,
    ChunkSize32 = 5,
    ChunkSize64 = 6,
}

internal static class ChunkSizeTypeHelpers
{
    public static int ChunkSizeBitShiftFromType(this ChunkSizeType chunkSizeType) => chunkSizeType switch
    {
        ChunkSizeType.ChunkSize16 => 4,
        ChunkSizeType.ChunkSize32 => 5,
        ChunkSizeType.ChunkSize64 => 6,

        _ => ThrowUnknownChunkSizeTypeException(chunkSizeType)
    };

    [DoesNotReturn]
    private static int ThrowUnknownChunkSizeTypeException(ChunkSizeType chunkSizeType)
    {
        throw new ArgumentOutOfRangeException(nameof(chunkSizeType), chunkSizeType, "Unknown chunk size type");
    }
}