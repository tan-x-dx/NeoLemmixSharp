using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class LevelTextDataComponentReader : ILevelDataReader
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelTextDataSectionIdentifier;

    public LevelTextDataComponentReader(List<string> stringIdLookup)
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

        LevelReadWriteHelpers.ReaderAssert(
            numberOfItemsInSection == numberOfPreTextItems + numberOfPostTextItems,
            "Wrong number of items for level text data section! " +
            $"Expected {numberOfItemsInSection} items total. Read: {numberOfPreTextItems} pre text items, {numberOfPostTextItems} post text items");
    }
}