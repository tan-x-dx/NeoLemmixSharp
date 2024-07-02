using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public static class TerrainComponentWriter
{
    private const int EraseBitShift = 0;
    private const int NoOverwriteBitShift = 1;
    private const int TintBitShift = 2;

    private const int NumberOfBytesForMainTerrainData = 9;

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x60, 0xBB];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainData.Count;
    }

    public static void WriteSection(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        foreach (var terrainData in levelData.AllTerrainData)
        {
            var terrainArchetypeData = levelData.TerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
            if (terrainArchetypeData is null)
                throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

            WriteTerrainData(writer, stringIdLookup, terrainArchetypeData, terrainData);
        }
    }

    public static void WriteTerrainData(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        TerrainArchetypeData terrainArchetypeData,
        TerrainData terrainData)
    {
        writer.Write(GetNumberOfBytesWritten(terrainData));

        writer.Write(stringIdLookup[terrainArchetypeData.Style!]);
        writer.Write(stringIdLookup[terrainArchetypeData.TerrainPiece!]);

        writer.Write((ushort)(terrainData.X + Helpers.PositionOffset));
        writer.Write((ushort)(terrainData.Y + Helpers.PositionOffset));
        writer.Write(Helpers.GetOrientationByte(terrainData.RotNum, terrainData.Flip));

        WriteTerrainDataMisc(writer, terrainData);
    }

    private static ushort GetNumberOfBytesWritten(TerrainData terrainData)
    {
        return (ushort)(NumberOfBytesForMainTerrainData + (terrainData.Tint.HasValue ? 4 : 1));
    }

    private static void WriteTerrainDataMisc(BinaryWriter writer, TerrainData terrainData)
    {
        var miscDataBits = (terrainData.Erase ? 1 << EraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << NoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << TintBitShift : 0);

        writer.Write((byte)miscDataBits);

        if (!terrainData.Tint.HasValue)
            return;

        var tint = terrainData.Tint.Value;
        writer.Write(tint.R);
        writer.Write(tint.G);
        writer.Write(tint.B);
    }
}