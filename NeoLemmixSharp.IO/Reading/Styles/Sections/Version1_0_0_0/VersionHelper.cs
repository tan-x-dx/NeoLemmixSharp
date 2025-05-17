using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class VersionHelper : IStyleDataSectionReaderVersionHelper
{
    public StyleDataSectionReader[] GetStyleDataSectionReaders()
    {
        var stringIdLookup = new List<string>(ReadWriteHelpers.InitialStringListCapacity);

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
