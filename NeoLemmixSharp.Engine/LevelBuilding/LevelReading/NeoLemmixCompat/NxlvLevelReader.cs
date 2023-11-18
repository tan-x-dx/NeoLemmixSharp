using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly Dictionary<string, INeoLemmixDataReader> _dataReaders = new();

    private readonly List<NeoLemmixGadgetData> _neoLemmixGadgetData = new();

    private INeoLemmixDataReader? _currentDataReader;

    public LevelData LevelData { get; }

    public NxlvLevelReader()
    {
        LevelData = new LevelData();

        AddDataReader(new LevelDataReader(LevelData, false));
        AddDataReader(new LevelDataReader(LevelData, true));
        AddDataReader(new SkillSetReader(LevelData));
        AddDataReader(new TerrainGroupReader(LevelData.AllTerrainGroups));
        AddDataReader(new GadgetReader(_neoLemmixGadgetData));
        AddDataReader(new TerrainReader(LevelData.AllTerrainData));
        AddDataReader(new LemmingReader());

        AddDataReader(new SpriteSetRecoloringReader(LevelData.ThemeData.LemmingSpriteSetRecoloring));
        AddDataReader(new StateRecoloringReader(LevelData.ThemeData));
        AddDataReader(new ShadesReader());
        AddDataReader(new AnimationDataReader(LevelData.ThemeData));

        return;

        void AddDataReader(INeoLemmixDataReader dataReader)
        {
            _dataReaders.Add(dataReader.IdentifierToken, dataReader);
        }
    }

    public void ReadLevel(string levelFilePath)
    {
        var lines = File.ReadAllLines(levelFilePath);

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            ProcessLine(line, index);
        }

        if (string.IsNullOrWhiteSpace(LevelData.LevelTitle))
        {
            LevelData.LevelTitle = "Untitled";
        }

        ReadStyle();
        ReadSpriteData();
        SetUpGadgets();
    }

    private void ProcessLine(string line, int index)
    {
        if (!ReadingHelpers.TrySplitIntoTokens(line, out var tokens))
            return;

        if (_currentDataReader != null)
        {
            _currentDataReader.ReadNextLine(tokens);

            if (_currentDataReader.FinishedReading)
            {
                _currentDataReader = null;
            }
            return;
        }

        if (_dataReaders.TryGetValue(tokens[0], out var dataReader))
        {
            _currentDataReader = dataReader;
            _currentDataReader.BeginReading(tokens);

            return;
        }

        throw new InvalidOperationException($"Unknown token: [{tokens[0]}] - line {index}: \"{line}\"");
    }

    private void ReadStyle()
    {
        var theme = LevelData.LevelTheme;
        var themeFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", theme, "theme.nxtm");

        var themeLines = File.ReadAllLines(themeFilePath);

        foreach (var line in themeLines)
        {
            ProcessThemeLine(line);
        }

        LevelData.ThemeData.LemmingSpritesFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", LevelData.ThemeData.BaseStyle, "lemmings");
    }

    private void ProcessThemeLine(string line)
    {
        if (!ReadingHelpers.TrySplitIntoTokens(line, out var tokens))
            return;

        switch (tokens[0])
        {
            case "LEMMINGS":
                LevelData.ThemeData.BaseStyle = tokens[1];
                break;

            case "$COLORS":
                break;

            case "MASK":
                LevelData.ThemeData.Mask = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "MINIMAP":
                LevelData.ThemeData.Minimap = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "BACKGROUND":
                LevelData.ThemeData.Background = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "ONE_WAYS":
                LevelData.ThemeData.OneWays = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "PICKUP_BORDER":
                LevelData.ThemeData.PickupBorder = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "PICKUP_INSIDE":
                LevelData.ThemeData.PickupInside = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "END":
                break;
        }
    }

    private void ReadSpriteData()
    {
        var schemeFilePath = Path.Combine(LevelData.ThemeData.LemmingSpritesFilePath, "scheme.nxmi");

        var schemeLines = File.ReadAllLines(schemeFilePath);

        for (var index = 0; index < schemeLines.Length; index++)
        {
            var line = schemeLines[index];
            ProcessLine(line, index);
        }
    }

    private void SetUpGadgets()
    {

    }

    public void Dispose()
    {
        foreach (var terrainGroup in LevelData.AllTerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }
}
