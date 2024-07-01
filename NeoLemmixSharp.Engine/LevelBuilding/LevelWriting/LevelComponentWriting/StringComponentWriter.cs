using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public static class StringComponentWriter
{
    private const int StringBufferSize = 1024;

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x26, 0x44];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        GenerateStringIdLookup(stringIdLookup, levelData);

        return (ushort)stringIdLookup.Count;
    }

    public static void WriteSection(
        BinaryWriter writer,
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(stringIdLookup, levelData));

        Span<byte> buffer = new byte[StringBufferSize];

        var utf8Encoding = Encoding.UTF8;

        foreach (var (stringToWrite, id) in stringIdLookup.OrderBy(kvp => kvp.Value))
        {
            writer.Write(id);

            var byteCount = utf8Encoding.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }

    private static void GenerateStringIdLookup(
        Dictionary<string, ushort> stringIdLookup,
        LevelData levelData)
    {
        TryAdd(levelData.LevelTitle);
        TryAdd(levelData.LevelAuthor);

        HandleBackgroundString();

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
            TryAdd(terrainArchetypeData.Style!);
            TryAdd(terrainArchetypeData.TerrainPiece!);
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            TryAdd(gadgetData.Style);
            TryAdd(gadgetData.GadgetPiece);
        }

        return;

        void TryAdd(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;

            // Add 1 to ensure ids start at 1, therefore can use value of zero as "Not found"
            stringIdLookup.TryAdd(s, (ushort)(1 + stringIdLookup.Count));
        }

        void HandleBackgroundString()
        {
            var backgroundData = levelData.LevelBackground;

            if (backgroundData is null || backgroundData.IsSolidColor)
                return;

            TryAdd(backgroundData.BackgroundImageName);
        }
    }
}