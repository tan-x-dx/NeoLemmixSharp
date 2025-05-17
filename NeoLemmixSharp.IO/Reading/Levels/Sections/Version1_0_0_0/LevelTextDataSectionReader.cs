using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class LevelTextDataSectionReader : LevelDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;

    public LevelTextDataSectionReader(
        StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelTextDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        ReadTextLines(rawFileData, levelData.PreTextLines);

        ReadTextLines(rawFileData, levelData.PostTextLines);

        AssertLevelTextDataCountsMakeSense(
            numberOfItemsInSection,
            levelData.PreTextLines.Count,
            levelData.PostTextLines.Count);
    }

    private void ReadTextLines(RawLevelFileDataReader rawFileData, List<string> collection)
    {
        int numberOfTextItems = rawFileData.Read8BitUnsignedInteger();
        collection.Capacity = numberOfTextItems;
        int i = numberOfTextItems;

        while (i-- > 0)
        {
            int stringId = rawFileData.Read16BitUnsignedInteger();
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