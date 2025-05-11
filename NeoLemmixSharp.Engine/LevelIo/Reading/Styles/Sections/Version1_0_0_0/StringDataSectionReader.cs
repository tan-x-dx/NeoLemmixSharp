using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class StringDataSectionReader : StyleDataSectionReader
{
    public override bool IsNecessary => true;

    private readonly FileStringReader<StyleFileSectionIdentifierHasher, StyleFileSectionIdentifier> _stringReader;

    public StringDataSectionReader(
        List<string> stringIdLookup)
        : base(StyleFileSectionIdentifier.StringDataSection)
    {
        _stringReader = new FileStringReader<StyleFileSectionIdentifierHasher, StyleFileSectionIdentifier>(stringIdLookup);
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        _stringReader.ReadSection(rawFileData);
    }
}