﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainDataSectionWriter : LevelDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public TerrainDataSectionWriter(StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.TerrainDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var terrainData in levelData.AllTerrainData)
        {
            WriteTerrainData(writer, terrainData);
        }
    }

    public void WriteTerrainData(
        RawLevelFileDataWriter writer,
        TerrainData terrainData)
    {
        writer.Write(GetNumberOfBytesWritten(terrainData));

        writer.Write(_stringIdLookup.GetStringId(terrainData.StyleName.ToString()));
        writer.Write(_stringIdLookup.GetStringId(terrainData.PieceName.ToString()));

        writer.Write((ushort)(terrainData.Position.X + ReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(terrainData.Position.Y + ReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(terrainData.Orientation, terrainData.FacingDirection));

        WriteTerrainDataMisc(writer, terrainData);
    }

    private static byte GetNumberOfBytesWritten(
        TerrainData terrainData)
    {
        var tintBytes = terrainData.Tint.HasValue ? 3 : 0;
        var resizeBytes = terrainData.Width.HasValue ||
                          terrainData.Height.HasValue ? 4 : 0;

        return (byte)(ReadWriteHelpers.NumberOfBytesForMainTerrainData + tintBytes + resizeBytes);
    }

    private static void WriteTerrainDataMisc(
        RawLevelFileDataWriter writer,
        TerrainData terrainData)
    {
        var miscDataBits = ReadWriteHelpers.EncodeTerrainArchetypeDataByte(terrainData);

        writer.Write(miscDataBits);

        if (terrainData.Tint.HasValue)
        {
            var tint = terrainData.Tint.Value;
            writer.Write(tint.R);
            writer.Write(tint.G);
            writer.Write(tint.B);
        }

        if (terrainData.Width.HasValue)
        {
            writer.Write((ushort)terrainData.Width.Value);
        }

        if (terrainData.Height.HasValue)
        {
            writer.Write((ushort)terrainData.Height.Value);
        }
    }
}