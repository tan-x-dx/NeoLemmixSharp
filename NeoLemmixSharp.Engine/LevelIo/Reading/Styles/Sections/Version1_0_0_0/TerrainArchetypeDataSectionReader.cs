using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class TerrainArchetypeDataSectionReader : StyleDataSectionReader
{
    public override StyleFileSectionIdentifier SectionIdentifier { get; }
    public override bool IsNecessary { get; }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}
