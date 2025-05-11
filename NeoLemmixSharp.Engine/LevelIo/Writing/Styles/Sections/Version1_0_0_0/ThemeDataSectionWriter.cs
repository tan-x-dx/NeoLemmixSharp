using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections.Version1_0_0_0;

public sealed class ThemeDataSectionReader : StyleDataSectionWriter
{
    public override bool IsNecessary { get; }

    public ThemeDataSectionReader()
        : base(StyleFileSectionIdentifier.ThemeDataSection)
    {
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        throw new NotImplementedException();
    }

    public override void WriteSection(RawStyleFileDataWriter writer, StyleData styleData)
    {
        throw new NotImplementedException();
    }
}
