using NeoLemmixSharp.IO.Versions;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class VersionHelper : IStyleDataSectionWriterVersionHelper
{
    public StyleDataSectionWriter[] GetStyleDataSectionWriters()
    {
        var stringIdLookup = new StringIdLookup();

        StyleDataSectionWriter[] sectionWriters =
        [
            new StringDataSectionWriter(stringIdLookup),
            new ThemeDataSectionWriter(stringIdLookup),
            new TerrainArchetypeDataSectionWriter(stringIdLookup),
            new GadgetArchetypeDataSectionWriter(stringIdLookup)
        ];

        return sectionWriters;
    }
}
