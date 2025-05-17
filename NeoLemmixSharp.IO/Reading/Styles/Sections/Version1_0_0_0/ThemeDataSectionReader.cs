using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionReader : StyleDataSectionReader
{
    public ThemeDataSectionReader(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, false)
    {
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        throw new NotImplementedException();
    }
}
