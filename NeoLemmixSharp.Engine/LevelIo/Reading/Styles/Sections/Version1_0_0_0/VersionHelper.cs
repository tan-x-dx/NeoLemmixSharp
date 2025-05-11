using NeoLemmixSharp.Engine.LevelIo.Versions;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class VersionHelper : IStyleDataSectionReaderVersionHelper
{
    public StyleDataSectionReader[] GetStyleDataSectionReaders()
    {
        var stringIdLookup = new List<string>(LevelReadWriteHelpers.InitialStringListCapacity);

        StyleDataSectionReader[] sectionReaders =
        [
            // Always process string data first
            new StringDataSectionReader(stringIdLookup),

            new ThemeDataSectionReader(stringIdLookup),
            new TerrainArchetypeDataSectionReader(stringIdLookup),
            new GadgetArchetypeDataSectionReader(stringIdLookup)
        ];

        return sectionReaders;
    }
}
