using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;

public abstract class StyleDataSectionReader
{
    public abstract StyleFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData);
}
