using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class ThemeDataSectionReader : StyleDataSectionReader
{
    public override bool IsNecessary { get; }

    public ThemeDataSectionReader()
        : base(StyleFileSectionIdentifier.ThemeDataSection)
    {
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        throw new NotImplementedException();
    }
}
