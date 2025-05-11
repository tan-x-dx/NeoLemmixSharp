using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;

public abstract class StyleDataSectionWriter
{
    public StyleFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public StyleDataSectionWriter(StyleFileSectionIdentifier sectionIdentifier)
    {
        SectionIdentifier = sectionIdentifier;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(StyleData styleData);

    public abstract void WriteSection(RawStyleFileDataWriter writer, StyleData styleData);
}
