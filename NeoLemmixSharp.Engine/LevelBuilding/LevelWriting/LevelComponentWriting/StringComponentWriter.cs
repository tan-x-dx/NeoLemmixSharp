using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public readonly ref struct StringComponentWriter
{
    private const int StringBufferSize = 1024;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public StringComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x26, 0x44];
        return sectionIdentifier;
    }

    private ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        GenerateStringIdLookup(levelData);

        return (ushort)_stringIdLookup.Count;
    }

    public void WriteSection(BinaryWriter writer, LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        Span<byte> buffer = new byte[StringBufferSize];

        var utf8Encoding = Encoding.UTF8;

        foreach (var (stringToWrite, id) in _stringIdLookup.OrderBy(kvp => kvp.Value))
        {
            writer.Write(id);

            var byteCount = utf8Encoding.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }

    private void GenerateStringIdLookup(LevelData levelData)
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

        TryAdd(levelData.PrimaryLevelObjective!.LevelObjectiveTitle);
        foreach (var levelObjective in levelData.SecondaryLevelObjectives)
        {
            TryAdd(levelObjective.LevelObjectiveTitle);
        }

        foreach (var terrainArchetypeData in levelData.TerrainArchetypeData)
        {
            TryAdd(terrainArchetypeData.Style!);
            TryAdd(terrainArchetypeData.TerrainPiece!);
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            TryAdd(gadgetData.Style);
            TryAdd(gadgetData.GadgetPiece);
        }

    }

    private void TryAdd(string s)
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