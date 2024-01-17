using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly INeoLemmixDataReader[] _dataReaders;
    private readonly List<NeoLemmixGadgetData> _neoLemmixGadgetData = new();

    private readonly LevelData _levelData = new();

    private INeoLemmixDataReader? _currentDataReader;

    public NxlvLevelReader()
    {
        _dataReaders =
        [
            new LevelDataReader(_levelData, false),
            new LevelDataReader(_levelData, true),
            new SkillSetReader(_levelData),
            new TerrainGroupReader(_levelData.AllTerrainGroups),
            new GadgetReader(_neoLemmixGadgetData),
            new TerrainReader(_levelData.AllTerrainData),
            new LemmingReader(),
            new SpriteSetRecoloringReader(_levelData.ThemeData.LemmingSpriteSetRecoloring),
            new StateRecoloringReader(_levelData.ThemeData),
            new ShadesReader(),

            new DudDataReader("$ANIMATIONS")
        ];
    }

    public LevelData ReadLevel(string levelFilePath)
    {
        using (var stream = new FileStream(levelFilePath, FileMode.Open))
        {
            using var streamReader = new StreamReader(stream);

            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (LineIsBlankOrComment(line))
                    continue;

                ProcessLine(line);
            }
        }

        if (string.IsNullOrWhiteSpace(_levelData.LevelTitle))
        {
            _levelData.LevelTitle = "Untitled";
        }

        ReadStyle();
        ReadSpriteData();
        SetUpGadgets();

        return _levelData;
    }

    private void ProcessLine(string line)
    {
        if (_currentDataReader != null)
        {
            _currentDataReader.ReadNextLine(line);

            if (_currentDataReader.FinishedReading)
            {
                _currentDataReader = null;
            }
            return;
        }

        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        if (!TryGetWithSpan(firstToken, out var dataReader)) // If there's no applicable reader, just skip
            return;

        _currentDataReader = dataReader;
        _currentDataReader.BeginReading(line);
    }

    private bool TryGetWithSpan(ReadOnlySpan<char> token, out INeoLemmixDataReader dataReader)
    {
        foreach (var item in _dataReaders)
        {
            var itemSpan = item.IdentifierToken.AsSpan();

            if (itemSpan.SequenceEqual(token))
            {
                dataReader = item;
                return true;
            }
        }

        dataReader = null!;
        return false;
    }

    private void ReadStyle()
    {
        var theme = _levelData.LevelTheme;
        var themeFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", theme, "theme.nxtm");

        using (var stream = new FileStream(themeFilePath, FileMode.Open))
        {
            using var streamReader = new StreamReader(stream);

            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (LineIsBlankOrComment(line))
                    continue;

                ProcessThemeLine(line);
            }
        }

        _levelData.ThemeData.LemmingSpritesFilePath = Path.Combine(
            RootDirectoryManager.RootDirectory,
            "styles",
            _levelData.ThemeData.BaseStyle,
            "lemmings");
    }

    private void ProcessThemeLine(string line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "LEMMINGS":
                _levelData.ThemeData.BaseStyle = secondToken.ToString();
                break;

            case "$COLORS":
                break;

            case "MASK":
                _levelData.ThemeData.Mask = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "MINIMAP":
                _levelData.ThemeData.Minimap = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "BACKGROUND":
                _levelData.ThemeData.Background = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "ONE_WAYS":
                _levelData.ThemeData.OneWays = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "PICKUP_BORDER":
                _levelData.ThemeData.PickupBorder = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "PICKUP_INSIDE":
                _levelData.ThemeData.PickupInside = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "END":
                break;
        }
    }

    private void ReadSpriteData()
    {
        var schemeFilePath = Path.Combine(
            _levelData.ThemeData.LemmingSpritesFilePath,
            "scheme.nxmi");

        using var stream = new FileStream(schemeFilePath, FileMode.Open);
        using var streamReader = new StreamReader(stream);

        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            if (LineIsBlankOrComment(line))
                continue;
            ProcessLine(line);
        }
    }

    private void SetUpGadgets()
    {

    }

    public void Dispose()
    {
        foreach (var terrainGroup in _levelData.AllTerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }

    private static bool LineIsBlankOrComment(string line)
    {
        return string.IsNullOrWhiteSpace(line) || line[0] == '#';
    }
}
