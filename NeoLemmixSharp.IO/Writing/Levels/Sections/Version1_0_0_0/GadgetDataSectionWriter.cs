using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class GadgetDataSectionWriter : LevelDataSectionWriter
{
    private const int NumberOfBytesForMainGadgetData = 13;
    private const int NumberOfBytesPerInputName = 2;
    private const int NumberOfBytesPerGadgetProperty = 5;

    private readonly StringIdLookup _stringIdLookup;

    public GadgetDataSectionWriter(StringIdLookup stringIdLookup)
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
        writer.Write(GetNumberOfBytesWritten(gadgetData));

        writer.Write((ushort)gadgetData.Id);
        writer.Write(_stringIdLookup.GetStringId(gadgetData.StyleName.ToString()));
        writer.Write(_stringIdLookup.GetStringId(gadgetData.PieceName.ToString()));

        writer.Write((ushort)(gadgetData.Position.X + ReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(gadgetData.Position.Y + ReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(gadgetData.Orientation, gadgetData.FacingDirection));

        writer.Write((byte)gadgetData.InitialStateId);
        writer.Write((byte)gadgetData.GadgetRenderMode);

        writer.Write((byte)gadgetData.InputNames.Length);
        foreach (var inputName in gadgetData.InputNames)
        {
            writer.Write(_stringIdLookup.GetStringId(inputName.ToString()));
        }

        var gadgetPropertyEnumerator = gadgetData.GetProperties();
        while (gadgetPropertyEnumerator.MoveNext())
        {
            var (gadgetProperty, gadgetPropertyValue) = gadgetPropertyEnumerator.Current;

            writer.Write((byte)gadgetProperty);
            writer.Write(gadgetPropertyValue);
        }
    }

    private static ushort GetNumberOfBytesWritten(GadgetData gadgetData)
    {
        var result = NumberOfBytesForMainGadgetData;
        result += 1 + NumberOfBytesPerInputName * gadgetData.InputNames.Length;
        result += NumberOfBytesPerGadgetProperty * gadgetData.NumberOfGadgetProperties;

        return (ushort)result;
    }
}