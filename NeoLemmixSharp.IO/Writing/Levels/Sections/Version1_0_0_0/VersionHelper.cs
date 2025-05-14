using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class VersionHelper : ILevelDataSectionWriterVersionHelper
{
    public LevelDataSectionWriter[] GetLevelDataSectionWriters()
    {
        var stringIdLookup = new Dictionary<string, ushort>(ReadWriteHelpers.InitialStringListCapacity);
        var terrainSectionWriter = new TerrainDataSectionWriter(stringIdLookup);

        LevelDataSectionWriter[] sectionWriters =
        [
            // StringDataSectionWriter needs to be first as it will populate the stringIdLookup!
            new StringDataSectionWriter(stringIdLookup),

            new LevelMetadataSectionWriter(stringIdLookup),
            new LevelTextDataSectionWriter(stringIdLookup),
            new LevelObjectiveDataSectionWriter(stringIdLookup),
            new HatchGroupDataSectionWriter(),
            new PrePlacedLemmingDataSectionWriter(),
            terrainSectionWriter,
            new TerrainGroupDataSectionWriter(stringIdLookup, terrainSectionWriter),
            new GadgetDataSectionWriter(stringIdLookup),
        ];

        return sectionWriters;
    }
}
