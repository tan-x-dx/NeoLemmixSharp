using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Versions;
using NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0.Gadgets;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class VersionHelper : IStyleDataSectionWriterVersionHelper
{
    public FileFormatVersion FileFormatVersion => new(1, 0, 0, 0);

    public StyleDataSectionWriter[] GetStyleDataSectionWriters()
    {
        var stringIdLookup = new MutableFileWriterStringIdLookup();

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
