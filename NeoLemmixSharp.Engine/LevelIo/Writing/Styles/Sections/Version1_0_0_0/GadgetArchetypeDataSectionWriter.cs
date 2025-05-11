using NeoLemmixSharp.Engine.LevelIo.Data.Style;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections.Version1_0_0_0;

public sealed class GadgetArchetypeDataSectionWriter : StyleDataSectionWriter
{
    public override bool IsNecessary { get; }

    public GadgetArchetypeDataSectionWriter()
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection)
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
