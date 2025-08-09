using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class StringDataSectionReader : StyleDataSectionReader
{
    private readonly FileStringReader<RawStyleFileDataReader> _stringReader;

    public StringDataSectionReader(
        MutableFileReaderStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.StringDataSection, true)
    {
        _stringReader = new FileStringReader<RawStyleFileDataReader>(stringIdLookup);
    }

    public override void ReadSection(RawStyleFileDataReader reader, StyleData styleData, int numberOfItemsInSection)
    {
        _stringReader.ReadSection(reader, numberOfItemsInSection);
    }
}
