using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class TerrainArchetypeDataSectionReader : StyleDataSectionReader
{
    public override bool IsNecessary { get; }

    public TerrainArchetypeDataSectionReader()
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection)
    {
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        throw new NotImplementedException();
    }
}
