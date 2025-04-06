﻿using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Common.Util.PositionTracking;

public enum ChunkSize
{
    ChunkSize16 = 4,
    ChunkSize32 = 5,
    ChunkSize64 = 6,
}

internal static class ChunkSizeHelpers
{
    public static int GetChunkSizeBitShift(this ChunkSize chunkSize) => chunkSize switch
    {
        ChunkSize.ChunkSize16 => (int)ChunkSize.ChunkSize16,
        ChunkSize.ChunkSize32 => (int)ChunkSize.ChunkSize32,
        ChunkSize.ChunkSize64 => (int)ChunkSize.ChunkSize64,

        _ => ThrowUnknownChunkSizeException(chunkSize)
    };

    [DoesNotReturn]
    private static int ThrowUnknownChunkSizeException(ChunkSize chunkSize)
    {
        throw new ArgumentOutOfRangeException(nameof(chunkSize), chunkSize, "Unknown chunk size");
    }
}