using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

public sealed class LevelTextDataSectionWriter : LevelDataSectionWriter
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelTextDataSection;
    public override bool IsNecessary => false;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelTextDataSectionWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(levelData.PreTextLines.Count + levelData.PostTextLines.Count);
    }

    public override void WriteSection(RawFileData writer, LevelData levelData)
    {
        WriteStrings(writer, levelData.PreTextLines);

        WriteStrings(writer, levelData.PostTextLines);
    }

    private void WriteStrings(RawFileData writer, List<string> stringList)
    {
        writer.Write((byte)stringList.Count);

        foreach (var stringToWrite in stringList)
        {
            writer.Write(_stringIdLookup[stringToWrite]);
        }
    }
}