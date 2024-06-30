using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public static class StringComponentWriter
{
    private const int StringBufferSize = 1024;

    [SkipLocalsInit]
    public static void WriteStringSection(BinaryWriter writer, LevelData levelData)
    {
        WriteSectionIdentifier(writer);

        var stringIdLookup = GenerateStringIdLookup(levelData);

        writer.Write((ushort)stringIdLookup.Count);

        Span<byte> buffer = new byte[StringBufferSize];

        var utf8Encoding = Encoding.UTF8;

        foreach (var (stringToWrite, id) in stringIdLookup.OrderBy(kvp => kvp.Value))
        {
            writer.Write((ushort)id);

            var byteCount = utf8Encoding.GetBytes(stringToWrite, buffer);

            writer.Write(buffer[..byteCount]);
        }
    }

    private static void WriteSectionIdentifier(BinaryWriter writer)
    {
        ReadOnlySpan<byte> sectionIdentifier = [0x26, 0x44];
        writer.Write(sectionIdentifier);
    }

    private static Dictionary<string, int> GenerateStringIdLookup(LevelData levelData)
    {
        var result = new Dictionary<string, int>();

        TryAdd(levelData.LevelTitle);
        TryAdd(levelData.LevelAuthor);

        foreach (var text in levelData.PreTextLines)
        {
            TryAdd(text);
        }

        foreach (var text in levelData.PostTextLines)
        {
            TryAdd(text);
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

        return result;

        void TryAdd(string s)
        {
            result.TryAdd(s, result.Count);
        }
    }
}