using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public readonly ref struct TerrainComponentWriter
{
    private const int TerrainPiecePositionOffset = 512;

    private const int FlipBitShift = 2;

    private const int EraseBitShift = 0;
    private const int NoOverwriteBitShift = 1;
    private const int TintBitShift = 2;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public TerrainComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x60, 0xBB];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainData.Count;
    }

    public void WriteSection(BinaryWriter writer, LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        foreach (var terrainData in levelData.AllTerrainData)
        {
            WriteTerrainData(writer, levelData.TerrainArchetypeData, terrainData);
        }
    }

    private void WriteTerrainData(
        BinaryWriter memoryWriter,
        List<TerrainArchetypeData> allTerrainArchetypeData,
        TerrainData terrainData)
    {
        var terrainArchetypeData = allTerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
        if (terrainArchetypeData is null)
            throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

        memoryWriter.Write(GetNumberOfBytesWritten(terrainData));

        memoryWriter.Write(_stringIdLookup[terrainArchetypeData.Style!]);
        memoryWriter.Write(_stringIdLookup[terrainArchetypeData.TerrainPiece!]);

        memoryWriter.Write((ushort)(terrainData.X + TerrainPiecePositionOffset));
        memoryWriter.Write((ushort)(terrainData.Y + TerrainPiecePositionOffset));

        memoryWriter.Write(GetOrientationByte(terrainData));

        WriteTerrainDataMisc(memoryWriter, terrainData);
    }

    private static ushort GetNumberOfBytesWritten(TerrainData terrainData)
    {
        return (ushort)(9 + (terrainData.Tint.HasValue ? 4 : 1));
    }

    private static byte GetOrientationByte(TerrainData terrainData)
    {
        var orientationBits = (terrainData.RotNum & 3) |
                              (terrainData.Flip ? 1 << FlipBitShift : 0);

        return (byte)orientationBits;
    }

    private static void WriteTerrainDataMisc(BinaryWriter memoryWriter, TerrainData terrainData)
    {
        var miscDataBits = (terrainData.Erase ? 1 << EraseBitShift : 0) |
                           (terrainData.NoOverwrite ? 1 << NoOverwriteBitShift : 0) |
                           (terrainData.Tint.HasValue ? 1 << TintBitShift : 0);

        memoryWriter.Write((byte)miscDataBits);

        if (!terrainData.Tint.HasValue)
            return;

        var tint = terrainData.Tint.Value;
        memoryWriter.Write(tint.R);
        memoryWriter.Write(tint.G);
        memoryWriter.Write(tint.B);
    }
}