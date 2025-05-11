using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections;

internal abstract class StyleDataSectionWriter
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
}
