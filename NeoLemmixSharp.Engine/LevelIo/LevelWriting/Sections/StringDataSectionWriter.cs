using NeoLemmixSharp.Engine.LevelIo.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting.Sections;

public sealed class StringDataSectionWriter : LevelDataSectionWriter
{
    private const int MaxStackByteBufferSize = 256;

    public override LevelFileSectionIdentifier SectionIdentifier => LevelFileSectionIdentifier.StringDataSection;
    public override bool IsNecessary => true;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public StringDataSectionWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        GenerateStringIdLookup(levelData);

        return (ushort)_stringIdLookup.Count;
    }

    [SkipLocalsInit]
    public override void WriteSection(
        RawFileData writer,
        LevelData levelData)
    {
        var utf8Encoding = Encoding.UTF8;
        var bufferSize = CalculateBufferSize(utf8Encoding);

        Span<byte> buffer = bufferSize > MaxStackByteBufferSize
            ? new byte[bufferSize]
            : stackalloc byte[bufferSize];

        foreach (var (stringToWrite, id) in _stringIdLookup.OrderBy(kvp => kvp.Value))
        {
            writer.Write(id);

            var byteCount = utf8Encoding.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }

    private int CalculateBufferSize(Encoding utf8Encoding)
    {
        var maxBufferSize = 0;

        foreach (var (stringToWrite, _) in _stringIdLookup)
        {
            maxBufferSize = Math.Max(maxBufferSize, utf8Encoding.GetByteCount(stringToWrite));
        }

        return maxBufferSize;
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

        foreach (var (_, terrainArchetypeData) in levelData.TerrainArchetypeData)
        {
            TryAdd(terrainArchetypeData.Style);
            TryAdd(terrainArchetypeData.TerrainPiece);
        }

        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            TryAdd(terrainGroup.GroupName!);
        }

        foreach (var (_, gadgetBuilder) in levelData.GadgetArchetypeData)
        {
            TryAdd(gadgetBuilder.StyleName);
            TryAdd(gadgetBuilder.PieceName);
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            foreach (var gadgetInputName in gadgetData.InputNames)
            {
                TryAdd(gadgetInputName);
            }
        }
    }

    private void TryAdd(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        // Ids start at 1, therefore can use value of zero as "Not found"
        var value = (ushort)(1 + _stringIdLookup.Count);

        ref var valueToAdd = ref CollectionsMarshal.GetValueRefOrAddDefault(_stringIdLookup, s, out var exists);
        if (!exists)
        {
            valueToAdd = value;
        }
    }

    private void HandleBackgroundString(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;

        if (backgroundData is null || backgroundData.IsSolidColor)
            return;

        TryAdd(backgroundData.BackgroundImageName);
    }
}