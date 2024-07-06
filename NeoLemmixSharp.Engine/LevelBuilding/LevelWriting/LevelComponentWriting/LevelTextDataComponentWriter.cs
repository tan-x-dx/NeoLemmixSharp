using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class LevelTextDataComponentWriter : ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelTextDataSectionIdentifier;

    public LevelTextDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(levelData.PreTextLines.Count + levelData.PostTextLines.Count);
    }

    public void WriteSection(BinaryWriter writer, LevelData levelData)
    {
        WriteStrings(writer, levelData.PreTextLines);

        WriteStrings(writer, levelData.PostTextLines);
    }

    private void WriteStrings(BinaryWriter writer, List<string> stringList)
    {
        writer.Write((byte)stringList.Count);

        foreach (var stringToWrite in stringList)
        {
            writer.Write(_stringIdLookup[stringToWrite]);
        }
    }
}