using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections;

internal abstract class LevelDataSectionReader : IComparable<LevelDataSectionReader>
{
    public LevelFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    public LevelDataSectionReader(LevelFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ushort GetSectionIdentifier() => LevelFileSectionIdentifierHasher.GetSectionIdentifier(SectionIdentifier);

    public abstract void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection);

    public int CompareTo(LevelDataSectionReader? other)
    {
        if (other == null) return 1;

        return (int)SectionIdentifier - (int)other.SectionIdentifier;
    }
}
