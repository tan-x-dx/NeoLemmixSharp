using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;
using NeoLemmixSharp.Engine.LevelBuilding.LevelWriting;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.Components;

public sealed class StringComponentReaderWriter : ILevelDataReader, ILevelDataWriter
{
    private const int StringBufferSize = 1024;

    private readonly Dictionary<string, ushort> _stringIdLookup;
    private readonly byte[] _byteBuffer = new byte[StringBufferSize];

    public StringComponentReaderWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        _stringIdLookup.Add(string.Empty, 0);

        var numberOfItems = reader.Read16BitUnsignedInteger();

        var utf8Encoding = Encoding.UTF8;

        while (numberOfItems > 0)
        {
            var id = reader.Read16BitUnsignedInteger();
            Helpers.ReaderAssert(id == _stringIdLookup.Count, "Invalid string ids");

            var stringLength = reader.Read16BitUnsignedInteger();

            var stringBuffer = new Span<byte>(_byteBuffer, 0, stringLength);
            reader.ReadBytes(stringBuffer);

            var actualString = utf8Encoding.GetString(stringBuffer);

            _stringIdLookup.Add(actualString, id);

            numberOfItems--;
        }
    }

    public ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x26, 0x44];
        return sectionIdentifier;
    }

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        GenerateStringIdLookup(levelData);

        return (ushort)_stringIdLookup.Count;
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        var buffer = new Span<byte>(_byteBuffer);
        var utf8Encoding = Encoding.UTF8;

        foreach (var (stringToWrite, id) in _stringIdLookup.OrderBy(kvp => kvp.Value))
        {
            writer.Write(id);

            var byteCount = utf8Encoding.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }

    private void GenerateStringIdLookup(
        LevelData levelData)
    {
        TryAdd(levelData.LevelTitle);
        TryAdd(levelData.LevelAuthor);

        HandleBackgroundString(levelData);

        foreach (var text in levelData.PreTextLines)
        {
            TryAdd(text);
        }

        foreach (var text in levelData.PostTextLines)
        {
            TryAdd(text);
        }

        foreach (var levelObjective in levelData.LevelObjectives)
        {
            TryAdd(levelObjective.LevelObjectiveTitle);
        }

        foreach (var terrainArchetypeData in levelData.TerrainArchetypeData)
        {
            TryAdd(terrainArchetypeData.Style);
            TryAdd(terrainArchetypeData.TerrainPiece);
        }

        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            TryAdd(terrainGroup.GroupName!);
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            TryAdd(gadgetData.Style);
            TryAdd(gadgetData.GadgetPiece);
        }
    }

    private void TryAdd(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        // Add 1 to ensure ids start at 1, therefore can use value of zero as "Not found"
        _stringIdLookup.TryAdd(s, (ushort)(1 + _stringIdLookup.Count));
    }

    private void HandleBackgroundString(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;

        if (backgroundData is null || backgroundData.IsSolidColor)
            return;

        TryAdd(backgroundData.BackgroundImageName);
    }
}