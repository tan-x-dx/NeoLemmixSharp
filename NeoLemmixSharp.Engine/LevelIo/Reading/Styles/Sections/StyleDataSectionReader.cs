using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;

public abstract class StyleDataSectionReader
{
    public abstract StyleFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => SectionIdentifier.GetSectionIdentifierBytes();

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}
