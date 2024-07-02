using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class GadgetComponentWriter : ILevelDataWriter
{
    private const int NumberOfBytesForMainGadgetData = 13;
    private const int NumberOfBytesPerGadgetProperty = 5;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public GadgetComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x3D, 0x98];
        return sectionIdentifier;
    }

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllGadgetData.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetData)
        {
            WriteGadgetData(writer, gadgetData);
        }
    }

    private void WriteGadgetData(
        BinaryWriter writer,
        GadgetData gadgetData)
    {
        writer.Write(GetNumberOfBytesWritten(gadgetData));

        writer.Write((ushort)gadgetData.Id);
        writer.Write(_stringIdLookup[gadgetData.Style]);
        writer.Write(_stringIdLookup[gadgetData.GadgetPiece]);

        writer.Write((ushort)(gadgetData.X + Helpers.PositionOffset));
        writer.Write((ushort)(gadgetData.Y + Helpers.PositionOffset));
        writer.Write(Helpers.GetOrientationByte(gadgetData.Orientation, gadgetData.FacingDirection));

        writer.Write((byte)gadgetData.InitialStateId);
        writer.Write((byte)gadgetData.GadgetRenderMode);

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
        return (ushort)(NumberOfBytesForMainGadgetData + (NumberOfBytesPerGadgetProperty * gadgetData.NumberOfGadgetProperties));
    }
}