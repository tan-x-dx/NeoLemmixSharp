using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

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
            var terrainArchetypeData = levelData.TerrainArchetypeData[terrainData.GetStylePiecePair()];

            WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }

    public void WriteTerrainData(
        BinaryWriter writer,
        TerrainArchetypeData terrainArchetypeData,
        TerrainData terrainData)
    {
        writer.Write(GetNumberOfBytesWritten(terrainData));

        writer.Write(_stringIdLookup[terrainArchetypeData.Style]);
        writer.Write(_stringIdLookup[terrainArchetypeData.TerrainPiece]);

        writer.Write((ushort)(terrainData.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(terrainData.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(terrainData.Orientation, terrainData.FacingDirection));

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
        var miscDataBits = (terrainData.Erase ? 1 << LevelReadWriteHelpers.TerrainDataEraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << LevelReadWriteHelpers.TerrainDataNoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << LevelReadWriteHelpers.TerrainDataTintBitShift : 0) |
                           (terrainData.Width.HasValue ? 1 << LevelReadWriteHelpers.TerrainDataResizeWidthBitShift : 0) |
                           (terrainData.Height.HasValue ? 1 << LevelReadWriteHelpers.TerrainDataResizeHeightBitShift : 0);

        writer.Write((byte)miscDataBits);

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