using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections;

public abstract class StyleDataSectionReader
{
    public StyleFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    protected StyleDataSectionReader(StyleFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData);
}
