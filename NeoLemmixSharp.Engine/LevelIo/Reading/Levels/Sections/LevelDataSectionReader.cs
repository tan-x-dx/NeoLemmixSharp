using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections;

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
