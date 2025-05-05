using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;

public abstract class LevelDataSectionReader
{
    public abstract LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => LevelFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}
