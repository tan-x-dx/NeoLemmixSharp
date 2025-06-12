using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class StringDataSectionWriter : LevelDataSectionWriter
{
    private readonly MutableStringIdLookup _stringIdLookup;

    public StringDataSectionWriter(MutableStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.StringDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        PopulateStringIdLookup(levelData);

        return (ushort)_stringIdLookup.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        _stringIdLookup.WriteStrings(writer);
    }

    private void PopulateStringIdLookup(LevelData levelData)
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
        _stringIdLookup.RecordString(levelData.LevelTheme);

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
            _stringIdLookup.RecordString(terrainData.StyleIdentifier);
            _stringIdLookup.RecordString(terrainData.PieceIdentifier);
        }
    }

    private void RecordTerrainGroupStrings(LevelData levelData)
    {
        foreach (var terrainGroup in levelData.AllTerrainGroups)
        {
            _stringIdLookup.RecordString(terrainGroup.GroupName!);

            foreach (var terrainData in levelData.AllTerrainData)
            {
                _stringIdLookup.RecordString(terrainData.StyleIdentifier);
                _stringIdLookup.RecordString(terrainData.PieceIdentifier);
            }
        }
    }

    private void RecordGadgetDataStrings(LevelData levelData)
    {
        foreach (var gadgetData in levelData.AllGadgetData)
        {
            _stringIdLookup.RecordString(gadgetData.StyleIdentifier);
            _stringIdLookup.RecordString(gadgetData.PieceIdentifier);

            foreach (var gadgetInputName in gadgetData.OverrideInputNames)
            {
                _stringIdLookup.RecordString(gadgetInputName);
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
