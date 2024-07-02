using NeoLemmixSharp.Engine.LevelBuilding.Components;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;

public readonly ref struct LevelWriter
{
    private readonly LevelData _levelData;
    private readonly Version _version;

    public LevelWriter(LevelData levelData, Version version)
    {
        _levelData = levelData;
        _version = version;
    }

    public void WriteToFile(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var writer = new BinaryWriter(fileStream);

        WriteVersion(writer);

        var stringIdLookup = new Dictionary<string, ushort>();

        var terrainComponentWriter = new TerrainComponentReaderWriter(stringIdLookup);

        ReadOnlySpan<ILevelDataWriter> levelDataWriters = new ILevelDataWriter[]
        {
            // StringComponentReaderWriter needs to be first as it will populate the stringIdLookup!
            new StringComponentReaderWriter(stringIdLookup),

            new LevelDataComponentReaderWriter(stringIdLookup),
            new HatchGroupComponentReaderWriter(),
            new LevelObjectiveComponentReaderWriter(stringIdLookup),
            new PrePlacedLemmingComponentReaderWriter(),
            terrainComponentWriter,
            new TerrainGroupComponentReaderWriter(stringIdLookup, terrainComponentWriter),
            new GadgetComponentReaderWriter(stringIdLookup),
        };

        foreach (var levelDataWriter in levelDataWriters)
        {
            writer.Write(levelDataWriter.GetSectionIdentifier());
            writer.Write(levelDataWriter.CalculateNumberOfItemsInSection(_levelData));

            levelDataWriter.WriteSection(writer, _levelData);
        }
    }

    private void WriteVersion(BinaryWriter writer)
    {
        writer.Write((ushort)_version.Major);
        writer.Write('.');
        writer.Write((ushort)_version.Minor);
        writer.Write('.');
        writer.Write((ushort)_version.Build);
        writer.Write('.');
        writer.Write((ushort)_version.Revision);
    }
}