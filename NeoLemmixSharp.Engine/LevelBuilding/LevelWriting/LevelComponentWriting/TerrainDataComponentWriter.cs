﻿using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class TerrainDataComponentWriter : ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;

    public TerrainDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainDataSectionIdentifier;

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainData.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var terrainData in levelData.AllTerrainData)
        {
            var terrainArchetypeData = levelData.TerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
            if (terrainArchetypeData is null)
                throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

            WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }

    public void WriteTerrainData(
        BinaryWriter writer,
        TerrainArchetypeData terrainArchetypeData,
        TerrainData terrainData)
    {
        writer.Write(GetNumberOfBytesWritten(terrainData));

        writer.Write(_stringIdLookup[terrainArchetypeData.Style!]);
        writer.Write(_stringIdLookup[terrainArchetypeData.TerrainPiece!]);

        writer.Write((ushort)(terrainData.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(terrainData.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write(LevelReadWriteHelpers.GetOrientationByte(terrainData.RotNum, terrainData.Flip));

        WriteTerrainDataMisc(writer, terrainData);
    }

    private static byte GetNumberOfBytesWritten(TerrainData terrainData)
    {
        var tintBytes = terrainData.Tint.HasValue ? 3 : 0;
        var resizeBytes = terrainData.Width.HasValue ||
                          terrainData.Height.HasValue ? 4 : 0;

        return (byte)(LevelReadWriteHelpers.NumberOfBytesForMainTerrainData + tintBytes + resizeBytes);
    }

    private static void WriteTerrainDataMisc(BinaryWriter writer, TerrainData terrainData)
    {
        var hasResizeData = terrainData.Width.HasValue ||
                            terrainData.Height.HasValue;

        var miscDataBits = (terrainData.Erase ? 1 << LevelReadWriteHelpers.EraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << LevelReadWriteHelpers.NoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << LevelReadWriteHelpers.TintBitShift : 0) |
                           (hasResizeData ? 1 << LevelReadWriteHelpers.ResizeBitShift : 0);

        writer.Write((byte)miscDataBits);

        if (terrainData.Tint.HasValue)
        {
            var tint = terrainData.Tint.Value;
            writer.Write(tint.R);
            writer.Write(tint.G);
            writer.Write(tint.B);
        }

        if (hasResizeData)
        {
            writer.Write((ushort)(terrainData.Width ?? 0));
            writer.Write((ushort)(terrainData.Height ?? 0));
        }
    }
}