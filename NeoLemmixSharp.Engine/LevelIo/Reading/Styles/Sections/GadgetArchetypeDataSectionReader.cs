using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;

public sealed class GadgetArchetypeDataSectionReader : StyleDataSectionReader
{
    public override StyleFileSectionIdentifier SectionIdentifier { get; }
    public override bool IsNecessary { get; }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData)
    {
        throw new NotImplementedException();
    }
}
