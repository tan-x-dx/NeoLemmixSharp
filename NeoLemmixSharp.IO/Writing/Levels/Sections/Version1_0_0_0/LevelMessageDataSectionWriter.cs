using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class LevelMessageDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public LevelMessageDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelTextDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(levelData.PreTextLines.Count + levelData.PostTextLines.Count);
    }

    public override void WriteSection(RawLevelFileDataWriter writer, LevelData levelData)
    {
        WriteStrings(writer, levelData.PreTextLines);

        WriteStrings(writer, levelData.PostTextLines);
    }

    private void WriteStrings(RawLevelFileDataWriter writer, List<string> stringList)
    {
        FileWritingException.WriterAssert(stringList.Count < 256, "Too many strings to serialise!");

        writer.Write8BitUnsignedInteger((byte)stringList.Count);

        foreach (var stringToWrite in stringList)
        {
            writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(stringToWrite));
        }
    }
}