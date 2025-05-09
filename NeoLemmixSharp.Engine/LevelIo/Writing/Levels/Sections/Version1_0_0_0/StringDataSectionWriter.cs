using NeoLemmixSharp.Engine.LevelIo.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections.Version1_0_0_0;

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
        RawLevelFileDataWriter writer,
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

        foreach (var kvp in _stringIdLookup)
        {
            maxBufferSize = Math.Max(maxBufferSize, utf8Encoding.GetByteCount(kvp.Key));
        }

        return maxBufferSize;
    }

    private void GenerateStringIdLookup(
        LevelData levelData)
    {
        RecordLevelMetadataStrings(levelData);
        RecordLevelTextMessageStrings(levelData);
        RecordLevelObjectiveStrings(levelData);
        RecordTerrainArchetypeDataStrings(levelData);
        RecordTerrainGroupStrings(levelData);
        RecordGadgetArchetypeDataStrings(levelData);
        RecordGadgetStrings(levelData);
    }

    private void RecordLevelMetadataStrings(LevelData levelData)
    {
        RecordString(levelData.LevelTitle);
        RecordString(levelData.LevelAuthor);

        HandleBackgroundString(levelData);
    }

    private void RecordLevelTextMessageStrings(LevelData levelData)
    {
        foreach (var text in levelData.PreTextLines)
        {
            RecordString(text);
        }

        foreach (var text in levelData.PostTextLines)
        {
            RecordString(text);
        }
    }

    private void RecordLevelObjectiveStrings(LevelData levelData)
    {
        foreach (var levelObjective in levelData.LevelObjectives)
        {
            RecordString(levelObjective.LevelObjectiveTitle);
        }
    }

    private void RecordTerrainArchetypeDataStrings(LevelData levelData)
    {
        foreach (var (_, terrainArchetypeData) in levelData.TerrainArchetypeData)
        {
            RecordString(terrainArchetypeData.Style);
            RecordString(terrainArchetypeData.TerrainPiece);
        }
    }

    private void RecordTerrainGroupStrings(LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            RecordString(terrainGroup.GroupName!);
        }
    }

    private void RecordGadgetArchetypeDataStrings(LevelData levelData)
    {
        foreach (var (_, gadgetBuilder) in levelData.GadgetArchetypeData)
        {
            RecordString(gadgetBuilder.StyleName);
            RecordString(gadgetBuilder.PieceName);
        }
    }

    private void RecordGadgetStrings(LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetData)
        {
            foreach (var gadgetInputName in gadgetData.InputNames)
            {
                RecordString(gadgetInputName);
            }
        }
    }

    private void RecordString(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        // Ids start at 1, therefore can use value of zero as "Not found"
        var nextStringId = (ushort)(1 + _stringIdLookup.Count);

        ref var correspondingStringId = ref CollectionsMarshal.GetValueRefOrAddDefault(_stringIdLookup, s, out var exists);
        if (!exists)
        {
            correspondingStringId = nextStringId;
        }
    }

    private void HandleBackgroundString(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;

        if (backgroundData is null || backgroundData.IsSolidColor)
            return;

        RecordString(backgroundData.BackgroundImageName);
    }
}