using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class VersionHelper : ILevelDataSectionWriterVersionHelper
{
    public LevelDataSectionWriter[] GetLevelDataSectionWriters()
    {
        var stringIdLookup = new MutableFileWriterStringIdLookup();
        var terrainSectionWriter = new TerrainDataSectionWriter(stringIdLookup);

        LevelDataSectionWriter[] sectionWriters =
        [
            new StringDataSectionWriter(stringIdLookup),
            new LevelMetadataSectionWriter(stringIdLookup),
            new LevelMessageDataSectionWriter(stringIdLookup),
            new LevelObjectiveDataSectionWriter(stringIdLookup),
            new TribeDataSectionWriter(stringIdLookup),
            new HatchGroupDataSectionWriter(),
            new PrePlacedLemmingDataSectionWriter(),
            terrainSectionWriter,
            new TerrainGroupDataSectionWriter(stringIdLookup, terrainSectionWriter),
            new GadgetDataSectionWriter(stringIdLookup),
            new GadgetLinkDataSectionWriter(stringIdLookup),
        ];

        return sectionWriters;
    }
}
