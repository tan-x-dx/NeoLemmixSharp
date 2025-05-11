using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class GadgetArchetypeDataSectionWriter : StyleDataSectionWriter
{
    public GadgetArchetypeDataSectionWriter()
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection, false)
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
