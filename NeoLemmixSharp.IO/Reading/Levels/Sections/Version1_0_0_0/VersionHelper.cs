using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class VersionHelper : ILevelDataSectionReaderVersionHelper
{
    public LevelDataSectionReader[] GetLevelDataSectionReaders()
    {
        var stringIdLookup = new List<string>(LevelReadWriteHelpers.InitialStringListCapacity);

        var terrainComponentReader = new TerrainDataSectionReader(stringIdLookup);
        LevelDataSectionReader[] sectionReaders =
        [
            // Always process string data first
            new StringDataSectionReader(stringIdLookup),

            new LevelMetadataSectionReader(stringIdLookup),
            new LevelTextDataSectionReader(stringIdLookup),
            new LevelObjectiveDataSectionReader(stringIdLookup),
            new HatchGroupDataSectionReader(),
            new PrePlacedLemmingDataSectionReader(),
            terrainComponentReader,
            new TerrainGroupDataSectionReader(stringIdLookup, terrainComponentReader),
            new GadgetDataSectionReader(stringIdLookup)
        ];

        return sectionReaders;
    }
}
