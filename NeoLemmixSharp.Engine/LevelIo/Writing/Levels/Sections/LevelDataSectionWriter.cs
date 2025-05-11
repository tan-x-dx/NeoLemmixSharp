using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;

public abstract class LevelDataSectionWriter
{
    public LevelFileSectionIdentifier SectionIdentifier { get; }
    public abstract bool IsNecessary { get; }

    public LevelDataSectionWriter(LevelFileSectionIdentifier sectionIdentifier)
    {
        SectionIdentifier = sectionIdentifier;
    }

    public ReadOnlySpan<byte> GetSectionIdentifierBytes() => LevelFileSectionIdentifierHasher.GetSectionIdentifierBytes(SectionIdentifier);

    public abstract ushort CalculateNumberOfItemsInSection(LevelData levelData);

    public abstract void WriteSection(RawLevelFileDataWriter writer, LevelData levelData);
}
