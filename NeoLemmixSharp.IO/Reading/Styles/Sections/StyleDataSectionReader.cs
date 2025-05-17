using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections;

internal abstract class StyleDataSectionReader : IComparable<StyleDataSectionReader>
{
    public StyleFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    protected StyleDataSectionReader(StyleFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection);

    public int CompareTo(StyleDataSectionReader? other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this, other)) return 0;

        return SectionIdentifier.CompareTo(other.SectionIdentifier);
    }
}
