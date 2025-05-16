using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class TerrainArchetypeDataSectionReader : StyleDataSectionWriter
{
    public TerrainArchetypeDataSectionReader()
        : base(StyleFileSectionIdentifier.TerrainArchetypeDataSection, false)
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
