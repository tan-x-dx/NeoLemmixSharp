using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;

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
        return (ushort)levelData.AllGadgetInstanceData.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetInstanceData)
        {
            WriteGadgetData(writer, gadgetData);
        }
    }

    private void WriteGadgetData(
        RawLevelFileDataWriter writer,
        GadgetInstanceData gadgetData)
    {
        writer.Write16BitUnsignedInteger((ushort)gadgetData.Identifier.GadgetId);
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetData.StyleIdentifier));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetData.PieceIdentifier));

        writer.Write8BitUnsignedInteger((byte)gadgetData.GadgetTypeInstanceData.GadgetType);

        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetData.OverrideName));

        writer.Write32BitSignedInteger(ReadWriteHelpers.EncodePoint(gadgetData.Position));
        writer.Write8BitUnsignedInteger((byte)DihedralTransformation.Encode(gadgetData.Orientation, gadgetData.FacingDirection));

        writer.WriteBool(gadgetData.IsFastForward);

        writer.Write8BitUnsignedInteger((byte)gadgetData.GadgetRenderMode);

        WriteGadgetTypeInstanceData(writer, gadgetData.GadgetTypeInstanceData);
    }

    private void WriteGadgetTypeInstanceData(RawLevelFileDataWriter writer, IGadgetTypeInstanceData gadgetTypeInstanceData)
    {
        switch (gadgetTypeInstanceData.GadgetType)
        {
            case GadgetType.HitBoxGadget:
                break;
            case GadgetType.HatchGadget:
                break;
            case GadgetType.LogicGate:
                break;
            case GadgetType.LevelTimerObserver:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(gadgetTypeInstanceData.GadgetType), gadgetTypeInstanceData.GadgetType, "Unknown gadget type!");
        }
    }

    private void WriteOverrideInputNames(
        RawLevelFileDataWriter writer,
        GadgetInstanceData gadgetData)
    {
        /*     writer.Write8BitUnsignedInteger((byte)gadgetData.OverrideInputNames.Length);
             foreach (var inputName in gadgetData.OverrideInputNames)
             {
                 writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(inputName));
             }*/
    }

    private static void WriteLayerColorData(RawLevelFileDataWriter writer, GadgetInstanceData gadgetData)
    {
        /*      writer.Write8BitUnsignedInteger((byte)gadgetData.LayerColorData.Length);
              foreach (var layerColorData in gadgetData.LayerColorData)
              {
                  writer.Write8BitUnsignedInteger((byte)layerColorData.StateIndex);
                  writer.Write8BitUnsignedInteger((byte)layerColorData.LayerIndex);
                  if (layerColorData.UsesSpecificColor)
                  {
                      writer.WriteBool(true);

                      Span<byte> buffer = [0, 0, 0, 0];
                      ReadWriteHelpers.WriteArgbBytes(layerColorData.SpecificColor, buffer);
                      writer.WriteBytes(buffer);
                  }
                  else
                  {
                      writer.WriteBool(false);
                      writer.Write8BitUnsignedInteger((byte)layerColorData.TribeId);
                      writer.Write8BitUnsignedInteger((byte)layerColorData.SpriteLayerColorType);
                  }
              }*/
    }

    private static void WriteOverrideHitBoxCriteriaData(RawLevelFileDataWriter writer, GadgetInstanceData gadgetData)
    {
        /*      if (gadgetData.OverrideHitBoxCriteriaData is null)
              {
                  writer.WriteBool(false);
                  return; 
              }*/

        //     writer.WriteBool(true);
        //     new GadgetHitBoxCriteriaWriter<RawLevelFileDataWriter>(writer).WriteHitBoxCriteria(gadgetData.OverrideHitBoxCriteriaData);
    }

    private static void WriteGadgetProperties(
        RawLevelFileDataWriter writer,
        GadgetInstanceData gadgetData)
    {
        /*    var gadgetPropertyEnumerator = gadgetData.GetProperties();
            while (gadgetPropertyEnumerator.MoveNext())
            {
                var (gadgetProperty, gadgetPropertyValue) = gadgetPropertyEnumerator.Current;

                writer.Write8BitUnsignedInteger((byte)gadgetProperty);
                writer.Write32BitSignedInteger(gadgetPropertyValue);
            }*/
    }
}
