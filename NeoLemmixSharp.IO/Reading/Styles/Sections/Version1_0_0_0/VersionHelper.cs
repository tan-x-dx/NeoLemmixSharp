using NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;
using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class VersionHelper : IStyleDataSectionReaderVersionHelper
{
    public StyleDataSectionReader[] GetStyleDataSectionReaders()
    {
        var stringIdLookup = new StringIdLookup();

        StyleDataSectionReader[] sectionReaders =
        [
            new StringDataSectionReader(stringIdLookup),

            new ThemeDataSectionReader(stringIdLookup),
            new TerrainArchetypeDataSectionReader(stringIdLookup),
            new GadgetArchetypeDataSectionReader(stringIdLookup)
        ];

        return sectionReaders;
    }
}
