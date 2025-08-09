using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections;

internal abstract class LevelDataSectionWriter : IComparable<LevelDataSectionWriter>
{
    public LevelFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    public LevelDataSectionWriter(LevelFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ushort GetSectionIdentifier() => LevelFileSectionIdentifierHasher.GetSectionIdentifier(SectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(RawLevelFileDataWriter writer, LevelData levelData);

    public int CompareTo(LevelDataSectionWriter? other)
    {
        if (other == null) return 1;

        return (int)SectionIdentifier - (int)other.SectionIdentifier;
    }
}
