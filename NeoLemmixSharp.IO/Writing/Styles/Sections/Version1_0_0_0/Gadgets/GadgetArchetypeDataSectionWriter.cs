using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0.Gadgets;

internal sealed class GadgetArchetypeDataSectionWriter : StyleDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public GadgetArchetypeDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        return (ushort)styleData.GadgetArchetypeDataLookup.Count;
    }

    public override void WriteSection(
        RawStyleFileDataWriter writer,
        StyleData styleData)
    {
        foreach (var kvp in styleData.GadgetArchetypeDataLookup)
        {
            //     WriteGadgetArchetypeData(writer, kvp.Value);
        }
    }
    /*
    private void WriteGadgetArchetypeData(
        RawStyleFileDataWriter writer,
        IGadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetArchetypeData.PieceIdentifier));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetArchetypeData.GadgetName));

        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.GadgetType);
        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.ResizeType);

        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.BaseSpriteSize.W);
        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.BaseSpriteSize.H);

        WriteNineSliceData(writer, gadgetArchetypeData);

        WriteGadgetStates(writer, gadgetArchetypeData);

        WriteMiscData(writer, gadgetArchetypeData);
    }

    private static void WriteNineSliceData(
        RawStyleFileDataWriter writer,
        IGadgetArchetypeData gadgetArchetypeData)
    {
        var nineSliceData = gadgetArchetypeData.NineSliceData;
        writer.Write16BitUnsignedInteger((ushort)nineSliceData.X);
        writer.Write16BitUnsignedInteger((ushort)nineSliceData.W);
        writer.Write16BitUnsignedInteger((ushort)nineSliceData.Y);
        writer.Write16BitUnsignedInteger((ushort)nineSliceData.H);
    }

    private void WriteGadgetStates(RawStyleFileDataWriter writer, IGadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.AllGadgetStateData.Length);

        new GadgetStateWriter(writer, _stringIdLookup).WriteStateData(gadgetArchetypeData);
    }

    private static void WriteMiscData(RawStyleFileDataWriter writer, IGadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write8BitUnsignedInteger((byte)gadgetArchetypeData.MiscData.Count);

        foreach (var kvp in gadgetArchetypeData.MiscData)
        {
            var enumValue = kvp.Key;
            var miscDataValue = kvp.Value;

            writer.Write8BitUnsignedInteger((byte)enumValue);
            writer.Write32BitSignedInteger(miscDataValue);
        }
    }*/
}
