using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionReader : StyleDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public ThemeDataSectionReader(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        throw new NotImplementedException();
    }
}
