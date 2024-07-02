using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

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

        var terrainComponentWriter = new TerrainComponentWriter(stringIdLookup);

        ReadOnlySpan<ILevelDataWriter> levelDataWriters = new ILevelDataWriter[]
        {
            // StringComponentWriter needs to be first as it will populate the stringIdLookup!
            new StringComponentWriter(stringIdLookup),

            new LevelDataComponentWriter(stringIdLookup),
            new HatchGroupComponentWriter(),
            new LevelObjectiveComponentWriter(stringIdLookup),
            new PrePlacedLemmingComponentWriter(),
            terrainComponentWriter,
            new TerrainGroupComponentWriter(stringIdLookup, terrainComponentWriter),
            new GadgetComponentWriter(stringIdLookup),
        };

        foreach (var levelDataWriter in levelDataWriters)
        {
            var numberOfItemsInSection = levelDataWriter.CalculateNumberOfItemsInSection(_levelData);
            if (numberOfItemsInSection == 0)
                continue;

            writer.Write(levelDataWriter.GetSectionIdentifier());
            writer.Write(numberOfItemsInSection);

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