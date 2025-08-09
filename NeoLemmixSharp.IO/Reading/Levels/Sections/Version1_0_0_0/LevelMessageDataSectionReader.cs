using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class LevelMessageDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public LevelMessageDataSectionReader(
        FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelTextDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        ReadTextLines(reader, levelData.PreTextLines);

        ReadTextLines(reader, levelData.PostTextLines);

        AssertLevelTextDataCountsMakeSense(
            numberOfItemsInSection,
            levelData.PreTextLines.Count,
            levelData.PostTextLines.Count);
    }

    private void ReadTextLines(RawLevelFileDataReader reader, List<string> collection)
    {
        int numberOfTextItems = reader.Read8BitUnsignedInteger();
        collection.Capacity = numberOfTextItems;

        while (numberOfTextItems-- > 0)
        {
            int stringId = reader.Read16BitUnsignedInteger();
            collection.Add(_stringIdLookup[stringId]);
        }
    }

    private static void AssertLevelTextDataCountsMakeSense(
        int numberOfItemsInSection,
        int numberOfPreTextItems,
        int numberOfPostTextItems)
    {
        if (numberOfItemsInSection == numberOfPreTextItems + numberOfPostTextItems)
            return;

        throw new FileReadingException(
            "Wrong number of items for level text data section! " +
            $"Expected {numberOfItemsInSection} items total. Read: {numberOfPreTextItems} pre text items, {numberOfPostTextItems} post text items");
    }
}