using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public GadgetDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.GadgetDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllGadgetData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetData)
        {
            WriteGadgetData(writer, gadgetData);
        }
    }

    private void WriteGadgetData(
        RawLevelFileDataWriter writer,
        GadgetData gadgetData)
    {
        writer.Write((ushort)gadgetData.Id);
        writer.Write(_stringIdLookup.GetStringId(gadgetData.StyleIdentifier));
        writer.Write(_stringIdLookup.GetStringId(gadgetData.PieceIdentifier));

        writer.Write(_stringIdLookup.GetStringId(gadgetData.OverrideName));

        writer.Write(ReadWriteHelpers.EncodePoint(gadgetData.Position));
        writer.Write((byte)DihedralTransformation.Encode(gadgetData.Orientation, gadgetData.FacingDirection));

        writer.Write((byte)gadgetData.InitialStateId);
        writer.Write((byte)gadgetData.GadgetRenderMode);

        WriteOverrideInputNames(writer, gadgetData);
        WriteLayerColorData(writer, gadgetData);
        WriteOverrideHitBoxCriteriaData(writer, gadgetData);
        WriteGadgetProperties(writer, gadgetData);
    }

    private void WriteOverrideInputNames(
        RawLevelFileDataWriter writer,
        GadgetData gadgetData)
    {
        writer.Write((byte)gadgetData.OverrideInputNames.Length);
        foreach (var inputName in gadgetData.OverrideInputNames)
        {
            writer.Write(_stringIdLookup.GetStringId(inputName));
        }
    }

    private static void WriteLayerColorData(RawLevelFileDataWriter writer, GadgetData gadgetData)
    {
        writer.Write((byte)gadgetData.LayerColorData.Length);
        foreach (var layerColorData in gadgetData.LayerColorData)
        {
            writer.Write((byte)layerColorData.StateIndex);
            writer.Write((byte)layerColorData.LayerIndex);
            if (layerColorData.UsesSpecificColor)
            {
                writer.Write(true);

                Span<byte> buffer = [0, 0, 0, 0];
                ReadWriteHelpers.WriteArgbBytes(layerColorData.SpecificColor, buffer);
                writer.Write(buffer);
            }
            else
            {
                writer.Write(false);
                writer.Write((byte)layerColorData.TribeId);
                writer.Write((byte)layerColorData.SpriteLayerColorType);
            }
        }
    }

    private static void WriteOverrideHitBoxCriteriaData(RawLevelFileDataWriter writer, GadgetData gadgetData)
    {
        if (gadgetData.OverrideHitBoxCriteriaData is null)
        {
            writer.Write(false);
            return; 
        }

        writer.Write(true);
        new GadgetHitBoxCriteriaWriter<RawLevelFileDataWriter>(writer).WriteHitBoxCriteria(gadgetData.OverrideHitBoxCriteriaData);
    }

    private static void WriteGadgetProperties(
        RawLevelFileDataWriter writer,
        GadgetData gadgetData)
    {
        var gadgetPropertyEnumerator = gadgetData.GetProperties();
        while (gadgetPropertyEnumerator.MoveNext())
        {
            var (gadgetProperty, gadgetPropertyValue) = gadgetPropertyEnumerator.Current;

            writer.Write((byte)gadgetProperty);
            writer.Write(gadgetPropertyValue);
        }
    }
}
