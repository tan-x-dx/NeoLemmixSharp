using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections;

internal abstract class StyleDataSectionWriter : IComparable<StyleDataSectionWriter>
{
    public StyleFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    public StyleDataSectionWriter(StyleFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(StyleData styleData);

    public abstract void WriteSection(RawStyleFileDataWriter writer, StyleData styleData);

    public int CompareTo(StyleDataSectionWriter? other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this, other)) return 0;

        return SectionIdentifier.CompareTo(other.SectionIdentifier);
    }
}
