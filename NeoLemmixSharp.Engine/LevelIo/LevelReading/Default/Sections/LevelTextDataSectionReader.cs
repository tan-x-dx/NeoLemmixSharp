using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Sections;

public sealed class LevelTextDataSectionReader : LevelDataSectionReader
{
    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.LevelTextDataSection;
    public override bool IsNecessary => false;

    private readonly List<string> _stringIdLookup;

    public LevelTextDataSectionReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData)
    {
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();

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