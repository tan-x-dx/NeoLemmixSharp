using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections.Version1_0_0_0;

public sealed class TerrainArchetypeDataSectionReader : StyleDataSectionWriter
{
    public override StyleFileSectionIdentifier SectionIdentifier { get; }
    public override bool IsNecessary { get; }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        throw new NotImplementedException();
    }

    public override void WriteSection(RawLevelFileDataWriter writer, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}
