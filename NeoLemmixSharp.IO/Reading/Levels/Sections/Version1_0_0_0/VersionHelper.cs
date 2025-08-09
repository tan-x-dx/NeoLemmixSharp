using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class VersionHelper : ILevelDataSectionReaderVersionHelper
{
    public LevelDataSectionReader[] GetLevelDataSectionReaders()
    {
        var stringIdLookup = new MutableFileReaderStringIdLookup();

        var terrainComponentReader = new TerrainDataSectionReader(stringIdLookup);
        LevelDataSectionReader[] sectionReaders =
        [
            new StringDataSectionReader(stringIdLookup),
            new LevelMetadataSectionReader(stringIdLookup),
            new LevelMessageDataSectionReader(stringIdLookup),
            new LevelObjectiveDataSectionReader(stringIdLookup),
            new TribeDataSectionReader(stringIdLookup),
            new HatchGroupDataSectionReader(),
            new PrePlacedLemmingDataSectionReader(),
            terrainComponentReader,
            new TerrainGroupDataSectionReader(stringIdLookup, terrainComponentReader),
            new GadgetDataSectionReader(stringIdLookup),
            new GadgetTriggerDataSectionReader(stringIdLookup),
        ];

        return sectionReaders;
    }
}
