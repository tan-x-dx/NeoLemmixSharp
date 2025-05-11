using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections.Version1_0_0_0;

public sealed class ThemeDataSectionReader : StyleDataSectionReader
{
    public ThemeDataSectionReader(List<string> stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, false)
    {
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        throw new NotImplementedException();
    }
}
