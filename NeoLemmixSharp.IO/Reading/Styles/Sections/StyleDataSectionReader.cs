using NeoLemmixSharp.IO.Data;
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

    public ushort GetSectionIdentifier() => StyleFileSectionIdentifierHasher.GetSectionIdentifier(SectionIdentifier);

    public abstract void ReadSection(RawStyleFileDataReader reader, StyleData styleData, int numberOfItemsInSection);

    public int CompareTo(StyleDataSectionReader? other)
    {
        if (other == null) return 1;

        return (int)SectionIdentifier - (int)other.SectionIdentifier;
    }
}
