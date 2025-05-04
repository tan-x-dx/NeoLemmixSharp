using NeoLemmixSharp.Engine.LevelIo.Data;
using NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting;

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
        var writer = new RawLevelFileDataWriter();

        var stringIdLookup = new Dictionary<string, ushort>(LevelReadWriteHelpers.InitialStringListCapacity);

        var terrainSectionWriter = new TerrainDataSectionWriter(stringIdLookup);

        ReadOnlySpan<LevelDataSectionWriter> sectionWriters =
        [
            // StringDataSectionWriter needs to be first as it will populate the stringIdLookup!
            new StringDataSectionWriter(stringIdLookup),

            new LevelMetadataSectionWriter(stringIdLookup),
            new LevelTextDataSectionWriter(stringIdLookup),
            new HatchGroupDataSectionWriter(),
            new LevelObjectiveDataSectionWriter(stringIdLookup),
            new PrePlacedLemmingDataSectionWriter(),
            terrainSectionWriter,
            new TerrainGroupDataSectionWriter(stringIdLookup, terrainSectionWriter),
            new GadgetDataSectionWriter(stringIdLookup),
        ];

        WriteSections(writer, sectionWriters);

        writer.WriteToFile(filePath, _version);
    }

    private void WriteSections(
        RawLevelFileDataWriter writer,
        ReadOnlySpan<LevelDataSectionWriter> sectionWriters)
    {
        foreach (var sectionWriter in sectionWriters)
        {
            var numberOfItemsInSection = sectionWriter.CalculateNumberOfItemsInSection(_levelData);
            if (numberOfItemsInSection == 0)
            {
                LevelWritingException.WriterAssert(!sectionWriter.IsNecessary, "No data for necessary section!");
                continue;
            }

            writer.BeginWritingSection(sectionWriter.SectionIdentifier);
            writer.Write(sectionWriter.GetSectionIdentifierBytes());
            writer.Write(numberOfItemsInSection);

            sectionWriter.WriteSection(writer, _levelData);
            writer.EndWritingSection(sectionWriter.SectionIdentifier);
        }
    }
}