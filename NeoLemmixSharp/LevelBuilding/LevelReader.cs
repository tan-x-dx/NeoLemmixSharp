using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Reading;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelReader : IDisposable
{
    private const string _rootDirectory = "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5";

    private readonly Dictionary<string, IDataReader> _dataReaders = new();

    private IDataReader? _currentDataReader;

    public LevelData LevelData { get; } = new();
    public ThemeData ThemeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();

    public LevelReader()
    {
        AddDataReader(new LevelDataReader(LevelData, false));
        AddDataReader(new LevelDataReader(LevelData, true));
        AddDataReader(new SkillSetReader());
        AddDataReader(new TerrainGroupReader(AllTerrainGroups));
        AddDataReader(new GadgetReader());
        AddDataReader(new TerrainReader(AllTerrainData));

        AddDataReader(new SpriteSetRecolouringReader(ThemeData.LemmingSpriteSetRecolouring));
        AddDataReader(new StateRecolouringReader(ThemeData));
        AddDataReader(new ShadesReader());
        AddDataReader(new AnimationDataReader(ThemeData));
    }

    private void AddDataReader(IDataReader dataReader)
    {
        _dataReaders.Add(dataReader.IdentifierToken, dataReader);
    }

    public void ReadLevel(string levelFilePath)
    {
        var lines = File.ReadAllLines(levelFilePath);

        foreach (var line in lines)
        {
            ProcessLine(line);
        }

        if (string.IsNullOrWhiteSpace(LevelData.LevelTitle))
        {
            LevelData.LevelTitle = "Untitled";
        }

        ReadStyle();
        ReadSpriteData();
    }

    private void ProcessLine(string line)
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

        throw new InvalidOperationException($"Unknown token: [{tokens[0]}] - line: \"{line}\"");
    }

    private void ReadStyle()
    {
        var theme = LevelData.LevelTheme;
        var themeFilePath = Path.Combine(_rootDirectory, "styles", theme, "theme.nxtm");

        var themeLines = File.ReadAllLines(themeFilePath);

        foreach (var line in themeLines)
        {
            ProcessThemeLine(line);
        }

        LevelData.LemmingSpritesFilePath = Path.Combine(_rootDirectory, "styles", ThemeData.BaseStyle, "lemmings");
    }

    private void ProcessThemeLine(string line)
    {
        if (!ReadingHelpers.TrySplitIntoTokens(line, out var tokens))
            return;

        switch (tokens[0])
        {
            case "LEMMINGS":
                ThemeData.BaseStyle = tokens[1];
                break;

            case "$COLORS":
                break;

            case "MASK":
                ThemeData.Mask = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "MINIMAP":
                ThemeData.Minimap = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "BACKGROUND":
                ThemeData.Background = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "ONE_WAYS":
                ThemeData.OneWays = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "PICKUP_BORDER":
                ThemeData.PickupBorder = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "PICKUP_INSIDE":
                ThemeData.PickupInside = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "END":
                break;
        }
    }

    private void ReadSpriteData()
    {
        var schemeFilePath = Path.Combine(LevelData.LemmingSpritesFilePath, "scheme.nxmi");

        var schemeLines = File.ReadAllLines(schemeFilePath);

        foreach (var line in schemeLines)
        {
            ProcessLine(line);
        }
    }

    public void Dispose()
    {
        foreach (var terrainGroup in AllTerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }
}