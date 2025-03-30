using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelTextDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelTextDataSectionIdentifier;

    public LevelTextDataComponentReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();
        int numberOfPreTextItems = rawFileData.Read8BitUnsignedInteger();
        levelData.PreTextLines.Capacity = numberOfPreTextItems;
        int i = numberOfPreTextItems;

        while (i-- > 0)
        {
            int stringId = rawFileData.Read16BitUnsignedInteger();
            levelData.PreTextLines.Add(_stringIdLookup[stringId]);
        }

        int numberOfPostTextItems = rawFileData.Read8BitUnsignedInteger();
        levelData.PostTextLines.Capacity = numberOfPostTextItems;
        i = numberOfPostTextItems;

        while (i-- > 0)
        {
            int stringId = rawFileData.Read16BitUnsignedInteger();
            levelData.PostTextLines.Add(_stringIdLookup[stringId]);
        }

        AssertLevelTextDataBytesMakeSense(
            numberOfItemsInSection,
            numberOfPreTextItems,
            numberOfPostTextItems);
    }

    private static void AssertLevelTextDataBytesMakeSense(
        int numberOfItemsInSection,
        int numberOfPreTextItems,
        int numberOfPostTextItems)
    {
        if (numberOfItemsInSection == numberOfPreTextItems + numberOfPostTextItems)
            return;

        throw new LevelReadingException(
            "Wrong number of items for level text data section! " +
            $"Expected {numberOfItemsInSection} items total. Read: {numberOfPreTextItems} pre text items, {numberOfPostTextItems} post text items");
    }
}