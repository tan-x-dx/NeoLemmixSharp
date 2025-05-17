using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class StringDataSectionReader : StyleDataSectionReader
{
    private readonly FileStringReader<StyleFileSectionIdentifierHasher, StyleFileSectionIdentifier> _stringReader;

    public StringDataSectionReader(
        StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.StringDataSection, true)
    {
        _stringReader = new FileStringReader<StyleFileSectionIdentifierHasher, StyleFileSectionIdentifier>(stringIdLookup);
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        _stringReader.ReadSection(rawFileData, numberOfItemsInSection);
    }
}