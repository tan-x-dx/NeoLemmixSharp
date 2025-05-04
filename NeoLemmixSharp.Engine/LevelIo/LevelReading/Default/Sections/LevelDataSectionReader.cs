using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public abstract class LevelDataSectionReader
{
    public abstract LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => SectionIdentifier.GetSectionIdentifierBytes();

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}