using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class TerrainComponentWriter : ILevelDataWriter
{
    private const int EraseBitShift = 0;
    private const int NoOverwriteBitShift = 1;
    private const int TintBitShift = 2;

    private const int NumberOfBytesForMainTerrainData = 9;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public TerrainComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x60, 0xBB];
        return sectionIdentifier;
    }

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