using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetLinkDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public GadgetLinkDataSectionWriter(FileWriterStringIdLookup stringIdLookup) : base(LevelFileSectionIdentifier.GadgetTriggerDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllGadgetLinkData.Count;
    }

    public override void WriteSection(RawLevelFileDataWriter writer, LevelData levelData)
    {
        foreach (var gadgetLinkDatum in levelData.AllGadgetLinkData)
        {
            WriteGadgetLinkData(writer, gadgetLinkDatum);
        }
    }

    private void WriteGadgetLinkData(
        RawLevelFileDataWriter writer,
        GadgetLinkData gadgetLinkDatum)
    {
        writer.Write16BitUnsignedInteger((ushort)gadgetLinkDatum.SourceGadgetIdentifier.GadgetId);
        writer.Write16BitUnsignedInteger((ushort)gadgetLinkDatum.SourceGadgetStateId);
        writer.Write16BitUnsignedInteger((ushort)gadgetLinkDatum.TargetGadgetIdentifier.GadgetId);
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetLinkDatum.TargetGadgetInputName));
    }
}
