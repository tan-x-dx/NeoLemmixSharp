using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;

public abstract class StyleDataSectionWriter
{
    public abstract StyleFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => StyleFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(RawLevelFileDataWriter writer, LevelData levelData);
}
