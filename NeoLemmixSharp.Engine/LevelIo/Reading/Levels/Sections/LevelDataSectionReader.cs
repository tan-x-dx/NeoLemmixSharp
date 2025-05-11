using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Sections;

public abstract class LevelDataSectionReader
{
    public LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public LevelDataSectionReader(LevelFileSectionIdentifier sectionIdentifier)
    {
        SectionIdentifier = sectionIdentifier;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => LevelFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}
