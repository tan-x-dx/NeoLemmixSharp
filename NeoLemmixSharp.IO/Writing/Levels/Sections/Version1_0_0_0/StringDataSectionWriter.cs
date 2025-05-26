using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class StringDataSectionWriter : LevelDataSectionWriter
{
    private const int MaxStackByteBufferSize = 256;

    private readonly StringIdLookup _stringIdLookup;

    public StringDataSectionWriter(StringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection, true)
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
        var bufferSize = _stringIdLookup.CalculateBufferSize();

        FileWritingException.WriterAssert(bufferSize <= ushort.MaxValue, "Cannot serialize a string larger than 65535 bytes!");

        Span<byte> buffer = bufferSize > MaxStackByteBufferSize
            ? new byte[bufferSize]
            : stackalloc byte[bufferSize];

        foreach (var kvp in _stringIdLookup.OrderedPairs)
        {
            var stringToWrite = kvp.Key;
            var id = kvp.Value;

            writer.Write(id);

            var byteCount = Encoding.UTF8.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }

    private void GenerateStringIdLookup(LevelData levelData)
    {
        FileWritingException.WriterAssert(_stringIdLookup.Count == 0, "Expected string id lookup to be empty!");

        RecordLevelMetadataStrings(levelData);
        RecordLevelTextMessageStrings(levelData);
        RecordLevelObjectiveStrings(levelData);
        RecordTerrainDataStrings(levelData);
        RecordTerrainGroupStrings(levelData);
        RecordGadgetDataStrings(levelData);
    }

    private void RecordLevelMetadataStrings(LevelData levelData)
    {
        _stringIdLookup.RecordString(levelData.LevelTitle);
        _stringIdLookup.RecordString(levelData.LevelAuthor);
        _stringIdLookup.RecordString(levelData.LevelTheme.ToString());

        HandleBackgroundString(levelData);
    }

    private void RecordLevelTextMessageStrings(LevelData levelData)
    {
        foreach (var text in levelData.PreTextLines)
        {
            _stringIdLookup.RecordString(text);
        }

        foreach (var text in levelData.PostTextLines)
        {
            _stringIdLookup.RecordString(text);
        }
    }

    private void RecordLevelObjectiveStrings(LevelData levelData)
    {
        /*foreach (var levelObjective in levelData.LevelObjectives)
        {
            _stringIdLookup. RecordString(levelObjective.LevelObjectiveTitle);
        }*/
    }

    private void RecordTerrainDataStrings(LevelData levelData)
    {
        foreach (var terrainData in levelData.AllTerrainData)
        {
            _stringIdLookup.RecordString(terrainData.StyleName.ToString());
            _stringIdLookup.RecordString(terrainData.PieceName.ToString());
        }
    }

    private void RecordTerrainGroupStrings(LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            _stringIdLookup.RecordString(terrainGroup.GroupName!);

            foreach (var terrainData in levelData.AllTerrainData)
            {
                _stringIdLookup.RecordString(terrainData.StyleName.ToString());
                _stringIdLookup.RecordString(terrainData.PieceName.ToString());
            }
        }
    }

    private void RecordGadgetDataStrings(LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetData)
        {
            _stringIdLookup.RecordString(gadgetData.StyleName.ToString());
            _stringIdLookup.RecordString(gadgetData.PieceName.ToString());

            foreach (var gadgetInputName in gadgetData.InputNames)
            {
                _stringIdLookup.RecordString(gadgetInputName.ToString());
            }
        }
    }

    private void HandleBackgroundString(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;

        if (backgroundData is null || backgroundData.IsSolidColor)
            return;

        _stringIdLookup.RecordString(backgroundData.BackgroundImageName);
    }
}
