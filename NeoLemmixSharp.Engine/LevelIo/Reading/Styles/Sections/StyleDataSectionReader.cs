using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;

public abstract class StyleDataSectionReader
{
    public StyleFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    protected StyleDataSectionReader(StyleFileSectionIdentifier sectionIdentifier)
    {
        SectionIdentifier = sectionIdentifier;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData);
}
