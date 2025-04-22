using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.Components;

public sealed class LevelTextDataComponentWriter : LevelDataComponentWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelTextDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
        : base(LevelReadWriteHelpers.LevelTextDataSectionIdentifierIndex)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(levelData.PreTextLines.Count + levelData.PostTextLines.Count);
    }

    public override void WriteSection(BinaryWriter writer, LevelData levelData)
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