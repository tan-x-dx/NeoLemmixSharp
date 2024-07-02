using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class TerrainGroupComponentReaderWriter : ILevelDataReader, ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly TerrainComponentReaderWriter _terrainComponentReaderWriter;

    public TerrainGroupComponentReaderWriter(Dictionary<string, ushort> stringIdLookup, TerrainComponentReaderWriter terrainComponentReaderWriter)
    {
        _stringIdLookup = stringIdLookup;
        _terrainComponentReaderWriter = terrainComponentReaderWriter;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x7C, 0x5C];
        return sectionIdentifier;
    }

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllTerrainGroups.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            WriteTerrainGroupData(writer, levelData.TerrainArchetypeData, terrainGroup);
        }
    }

    private void WriteTerrainGroupData(
        BinaryWriter writer,
        List<TerrainArchetypeData> allTerrainArchetypeData,
        TerrainGroup terrainGroup)
    {
        writer.Write(_stringIdLookup[terrainGroup.GroupName!]);
        writer.Write(terrainGroup.TerrainDatas.Count);

        foreach (var terrainData in terrainGroup.TerrainDatas)
        {
            var terrainArchetypeData = allTerrainArchetypeData.Find(a => a.TerrainArchetypeId == terrainData.TerrainArchetypeId);
            if (terrainArchetypeData is null)
                throw new InvalidOperationException($"Could not locate TerrainArchetypeData with id {terrainData.TerrainArchetypeId}");

            _terrainComponentReaderWriter.WriteTerrainData(writer, terrainArchetypeData, terrainData);
        }
    }
}