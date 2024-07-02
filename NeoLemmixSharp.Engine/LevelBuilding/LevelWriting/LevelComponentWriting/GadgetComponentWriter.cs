using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public static class GadgetComponentWriter
{
    private const int NumberOfBytesForMainGadgetData = 13;
    private const int NumberOfBytesPerGadgetProperty = 5;

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x3D, 0x98];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.AllGadgetData.Count;
    }

    public static void WriteSection(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            WriteGadgetData(writer, stringIdLookup, gadgetData);
        }
    }

    private static void WriteGadgetData(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        GadgetData gadgetData)
    {
        writer.Write(GetNumberOfBytesWritten(gadgetData));

        writer.Write((ushort)gadgetData.Id);
        writer.Write(stringIdLookup[gadgetData.Style]);
        writer.Write(stringIdLookup[gadgetData.GadgetPiece]);

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