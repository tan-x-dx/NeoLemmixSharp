using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0.Gadgets;

internal sealed class GadgetArchetypeDataSectionWriter : StyleDataSectionWriter
{
    private readonly StringIdLookup _stringIdLookup;

    public GadgetArchetypeDataSectionWriter(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.GadgetArchetypeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        return (ushort)styleData.GadgetArchetypeData.Count;
    }

    public override void WriteSection(
        RawStyleFileDataWriter writer,
        StyleData styleData)
    {
        foreach (var kvp in styleData.GadgetArchetypeData)
        {
            WriteGadgetArchetypeData(writer, kvp.Value);
        }
    }

    private void WriteGadgetArchetypeData(
        RawStyleFileDataWriter writer,
        GadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write(_stringIdLookup.GetStringId(gadgetArchetypeData.PieceIdentifier));
        writer.Write(_stringIdLookup.GetStringId(gadgetArchetypeData.GadgetName));

        writer.Write((byte)gadgetArchetypeData.GadgetType);
        writer.Write((byte)gadgetArchetypeData.ResizeType);

        writer.Write((ushort)gadgetArchetypeData.BaseSpriteSize.W);
        writer.Write((ushort)gadgetArchetypeData.BaseSpriteSize.H);

        WriteNineSliceData(writer, gadgetArchetypeData);

        WriteGadgetStates(writer, gadgetArchetypeData);

        WriteMiscData(writer, gadgetArchetypeData);
    }

    private static void WriteNineSliceData(
        RawStyleFileDataWriter writer,
        GadgetArchetypeData gadgetArchetypeData)
    {
        var nineSliceData = gadgetArchetypeData.NineSliceData;
        writer.Write((ushort)nineSliceData.X);
        writer.Write((ushort)nineSliceData.W);
        writer.Write((ushort)nineSliceData.Y);
        writer.Write((ushort)nineSliceData.H);
    }

    private void WriteGadgetStates(RawStyleFileDataWriter writer, GadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write((byte)gadgetArchetypeData.AllGadgetStateData.Length);

        new GadgetStateWriter(writer, _stringIdLookup).WriteStateData(gadgetArchetypeData);
    }

    private static void WriteMiscData(RawStyleFileDataWriter writer, GadgetArchetypeData gadgetArchetypeData)
    {
        writer.Write((byte)gadgetArchetypeData.MiscData.Count);

        foreach (var kvp in gadgetArchetypeData.MiscData)
        {
            var enumValue = kvp.Key;
            var miscDataValue = kvp.Value;

            writer.Write((byte)enumValue);
            writer.Write(miscDataValue);
        }
    }
}
