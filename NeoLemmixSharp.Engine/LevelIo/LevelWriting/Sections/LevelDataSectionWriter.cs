using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

public abstract class LevelDataSectionWriter
{
    public abstract LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => SectionIdentifier.GetSectionIdentifierBytes();

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(
        RawFileData writer,
        LevelData levelData);
}