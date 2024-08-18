using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class LevelTextDataComponentReader : ILevelDataReader
{
    private readonly Dictionary<ushort, string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelTextDataSectionIdentifier;

    public LevelTextDataComponentReader(Dictionary<ushort, string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        var numberOfItemsInSection = reader.Read16BitUnsignedInteger();
        var numberOfPreTextItems = reader.Read8BitUnsignedInteger();
        var i = numberOfPreTextItems;

        while (i-- > 0)
        {
            var stringId = reader.Read16BitUnsignedInteger();
            levelData.PreTextLines.Add(_stringIdLookup[stringId]);
        }

        var numberOfPostTextItems = reader.Read8BitUnsignedInteger();
        i = numberOfPostTextItems;

        while (i-- > 0)
        {
            var stringId = reader.Read16BitUnsignedInteger();
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