using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections;

public abstract class LevelDataSectionReader
{
    public LevelFileSectionIdentifier SectionIdentifier { get; }
    public bool IsNecessary { get; }

    public LevelDataSectionReader(LevelFileSectionIdentifier sectionIdentifier, bool isNecessary)
    {
        SectionIdentifier = sectionIdentifier;
        IsNecessary = isNecessary;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => LevelFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}
