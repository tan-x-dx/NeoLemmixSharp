using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;
using NeoLemmixSharp.Engine.LevelIo.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections.Version1_0_0_0;

public sealed class GadgetDataSectionWriter : LevelDataSectionWriter
{
    private const int NumberOfBytesForMainGadgetData = 13;
    private const int NumberOfBytesPerInputName = 2;
    private const int NumberOfBytesPerGadgetProperty = 5;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public GadgetDataSectionWriter(Dictionary<string, ushort> stringIdLookup)
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
        writer.Write(_stringIdLookup[gadgetData.StyleIdentifier.ToString()]);
        writer.Write(_stringIdLookup[gadgetData.GadgetPiece.ToString()]);

        writer.Write((ushort)(gadgetData.Position.X + LevelReadWriteHelpers.PositionOffset));
        writer.Write((ushort)(gadgetData.Position.Y + LevelReadWriteHelpers.PositionOffset));
        writer.Write((byte)DihedralTransformation.Encode(gadgetData.Orientation, gadgetData.FacingDirection));

        writer.Write((byte)gadgetData.InitialStateId);
        writer.Write((byte)gadgetData.GadgetRenderMode);

        writer.Write((byte)gadgetData.InputNames.Length);
        foreach (var inputName in gadgetData.InputNames)
        {
            writer.Write(_stringIdLookup[inputName]);
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