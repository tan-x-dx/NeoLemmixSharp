using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class NxlvLevelReader : ILevelReader
{
    private readonly INeoLemmixDataReader[] _dataReaders;
    private readonly List<NeoLemmixGadgetData> _neoLemmixGadgetData = new();

    private INeoLemmixDataReader? _currentDataReader;

    public LevelData LevelData { get; }

    public NxlvLevelReader()
    {
        LevelData = new LevelData();

        _dataReaders =
        [
            new LevelDataReader(LevelData, false),
            new LevelDataReader(LevelData, true),
            new SkillSetReader(LevelData),
            new TerrainGroupReader(LevelData.AllTerrainGroups),
            new GadgetReader(_neoLemmixGadgetData),
            new TerrainReader(LevelData.AllTerrainData),
            new LemmingReader(),
            new SpriteSetRecoloringReader(LevelData.ThemeData.LemmingSpriteSetRecoloring),
            new StateRecoloringReader(LevelData.ThemeData),
            new ShadesReader(),

            new DudDataReader("$ANIMATIONS")
        ];
    }

    public void ReadLevel(string levelFilePath)
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

        if (string.IsNullOrWhiteSpace(LevelData.LevelTitle))
        {
            LevelData.LevelTitle = "Untitled";
        }

        ReadStyle();
        ReadSpriteData();
        SetUpGadgets();
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
        var theme = LevelData.LevelTheme;
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

        LevelData.ThemeData.LemmingSpritesFilePath = Path.Combine(RootDirectoryManager.RootDirectory, "styles", LevelData.ThemeData.BaseStyle, "lemmings");
    }

    private void ProcessThemeLine(string line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "LEMMINGS":
                LevelData.ThemeData.BaseStyle = secondToken.ToString();
                break;

            case "$COLORS":
                break;

            case "MASK":
                LevelData.ThemeData.Mask = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "MINIMAP":
                LevelData.ThemeData.Minimap = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "BACKGROUND":
                LevelData.ThemeData.Background = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "ONE_WAYS":
                LevelData.ThemeData.OneWays = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "PICKUP_BORDER":
                LevelData.ThemeData.PickupBorder = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "PICKUP_INSIDE":
                LevelData.ThemeData.PickupInside = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "END":
                break;
        }
    }

    private void ReadSpriteData()
    {
        var schemeFilePath = Path.Combine(LevelData.ThemeData.LemmingSpritesFilePath, "scheme.nxmi");

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
        foreach (var terrainGroup in LevelData.AllTerrainGroups)
        {
            terrainGroup.Dispose();
        }
    }

    private static bool LineIsBlankOrComment(string line)
    {
        return string.IsNullOrWhiteSpace(line) || line[0] == '#';
    }
}
