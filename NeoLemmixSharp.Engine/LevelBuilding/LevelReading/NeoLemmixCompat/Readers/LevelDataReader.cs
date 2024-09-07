using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class LevelDataReader : INeoLemmixDataReader
{
    private readonly LevelData _levelData;

    private bool _lockSpawnInterval;
    private int _numberOfLemmings;
    private int _maxSpawnInterval;
    private int _saveRequirement;
    private int? _timeLimitInSeconds;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "TITLE";
    public int SaveRequirement => _saveRequirement;
    public int? TimeLimitInSeconds => _timeLimitInSeconds;
    public int NumberOfLemmings => _numberOfLemmings;

    public LevelDataReader(LevelData levelData)
    {
        _levelData = levelData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        NxlvReadingHelpers.GetTokenPair(line, out _, out _, out var secondTokenIndex);

        var levelTitle = line.TrimAfterIndex(secondTokenIndex).ToString();
        _levelData.LevelTitle = string.IsNullOrWhiteSpace(levelTitle) ? "Untitled" : levelTitle;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken[0] == '$')
        {
            OnFinishedReading();
            FinishedReading = true;
            return true;
        }

        switch (firstToken)
        {
            case "AUTHOR":
                var levelAuthor = line.TrimAfterIndex(secondTokenIndex).ToString();
                _levelData.LevelAuthor = string.IsNullOrWhiteSpace(levelAuthor) ? "Unknown Author" : levelAuthor;
                break;

            case "ID":
                _levelData.LevelId = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
                break;

            case "VERSION":
                _levelData.Version = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = int.Parse(secondToken);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = int.Parse(secondToken);
                break;

            case "THEME":
                _levelData.LevelTheme = line.TrimAfterIndex(secondTokenIndex).ToString();
                break;

            case "BACKGROUND":
                _levelData.LevelBackground = ParseBackgroundData(line.TrimAfterIndex(secondTokenIndex));
                break;

            case "MUSIC":

                break;

            case "WIDTH":
                _levelData.LevelWidth = int.Parse(secondToken);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = int.Parse(secondToken);
                break;

            case "LEMMINGS":
                _numberOfLemmings = int.Parse(secondToken);
                break;

            case "SAVE_REQUIREMENT":

                _saveRequirement = int.Parse(secondToken);
                break;

            case "TIME_LIMIT":
                var timeLimitInSeconds = int.Parse(secondToken);

                if (timeLimitInSeconds <= 0)
                    throw new ArgumentOutOfRangeException(nameof(timeLimitInSeconds), timeLimitInSeconds, "Time limit must be positive!");
                if (timeLimitInSeconds > LevelConstants.MaxTimeLimitInSeconds)
                    throw new ArgumentOutOfRangeException(nameof(timeLimitInSeconds), timeLimitInSeconds, "Time limit too big!");

                _timeLimitInSeconds = timeLimitInSeconds;
                break;

            case "SPAWN_INTERVAL_LOCKED":
                _lockSpawnInterval = true;
                break;

            case "MAX_SPAWN_INTERVAL":
                _maxSpawnInterval = int.Parse(secondToken);
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException("Level Data", firstToken, line);
                break;
        }

        return false;
    }

    private void OnFinishedReading()
    {
        _levelData.HorizontalBoundaryBehaviour = BoundaryBehaviourType.Void;
        _levelData.VerticalBoundaryBehaviour = BoundaryBehaviourType.Void;

        var hatchGroupData = new HatchGroupData
        {
            HatchGroupId = 0,

            MaxSpawnInterval = _maxSpawnInterval,
            InitialSpawnInterval = _maxSpawnInterval,
            MinSpawnInterval = _lockSpawnInterval
                ? _maxSpawnInterval
                : LevelConstants.MinAllowedSpawnInterval
        };

        _levelData.AllHatchGroupData.Add(hatchGroupData);
    }

    public static bool TryReadLevelTitle(ReadOnlySpan<char> line, out string? levelTitle)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out _, out var secondTokenIndex);

        if (firstToken is "TITLE")
        {
            levelTitle = line.TrimAfterIndex(secondTokenIndex).ToString();
            return true;
        }

        levelTitle = null;
        return false;
    }

    private static BackgroundData ParseBackgroundData(ReadOnlySpan<char> backgroundToken)
    {
        return new BackgroundData
        {
            IsSolidColor = false,
            Color = Color.Black,
            BackgroundImageName = backgroundToken.ToString()
        };
    }
}