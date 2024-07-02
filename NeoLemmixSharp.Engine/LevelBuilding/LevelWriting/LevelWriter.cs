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

        // StringComponentWriter needs to be first as it will populate the stringIdLookup!
        StringComponentWriter.WriteSection(writer, stringIdLookup, _levelData);

        LevelDataComponentWriter.WriteSection(writer, stringIdLookup, _levelData);
        HatchGroupComponentWriter.WriteSection(writer, _levelData);
        LevelObjectiveComponentWriter.WriteSection(writer, stringIdLookup, _levelData);
        PrePlacedLemmingComponentWriter.WriteSection(writer, _levelData);
        TerrainComponentWriter.WriteSection(writer, stringIdLookup, _levelData);
        TerrainGroupComponentWriter.WriteSection(writer, stringIdLookup, _levelData);
        GadgetComponentWriter.WriteSection(writer, stringIdLookup, _levelData);
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