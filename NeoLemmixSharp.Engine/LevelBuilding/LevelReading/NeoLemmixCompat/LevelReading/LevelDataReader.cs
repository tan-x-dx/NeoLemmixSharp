using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class LevelDataReader : INeoLemmixDataReader
{
    private string? _levelTitle;
    private string? _levelAuthor;
    private ulong _levelId;
    private ulong _version;
    private int _levelWidth;
    private int _levelHeight;
    private int _levelStartPositionX;
    private int _levelStartPositionY;
    private string? _levelTheme;
    private string? _levelBackground;
    private int _numberOfLemmings;
    private int _saveRequirement;
    private int? _timeLimit;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "TITLE";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var rest = line[(1 + firstTokenIndex + firstToken.Length)..];
        _levelTitle = rest.ToString();
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);

        if (firstToken[0] == '$')
        {
            FinishedReading = true;
            return true;
        }

        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        var rest = secondToken.IsEmpty
            ? ReadOnlySpan<char>.Empty
            : line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "AUTHOR":
                _levelAuthor = rest.ToString();
                break;

            case "ID":
                _levelId = ReadingHelpers.ParseUnsignedNumericalValue<ulong>(secondToken);
                break;

            case "VERSION":
                _version = ReadingHelpers.ParseUnsignedNumericalValue<ulong>(secondToken);
                break;

            case "START_X":
                _levelStartPositionX = int.Parse(secondToken);
                break;

            case "START_Y":
                _levelStartPositionY = int.Parse(secondToken);
                break;

            case "THEME":
                _levelTheme = rest.ToString();
                break;

            case "BACKGROUND":
                _levelBackground = rest.ToString();
                break;

            case "MUSIC":

                break;

            case "WIDTH":
                _levelWidth = int.Parse(secondToken);
                break;

            case "HEIGHT":
                _levelHeight = int.Parse(secondToken);
                break;

            case "LEMMINGS":
                _numberOfLemmings = int.Parse(secondToken);
                break;

            case "SAVE_REQUIREMENT":
                _saveRequirement = int.Parse(secondToken);
                break;

            case "TIME_LIMIT":
                _timeLimit = int.Parse(secondToken);
                break;

            case "SPAWN_INTERVAL_LOCKED":

                break;

            case "MAX_SPAWN_INTERVAL":
                //     _MaxSpawnInterval = int.Parse(secondToken);

                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing LevelData: [{firstToken}] line: \"{line}\"");
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.LevelTitle = string.IsNullOrEmpty(_levelTitle) ? "Untitled" : _levelTitle;
        levelData.LevelAuthor = string.IsNullOrEmpty(_levelAuthor) ? "Unknown Author" : _levelAuthor;
        levelData.LevelId = _levelId;
        levelData.Version = _version;
        levelData.LevelWidth = _levelWidth;
        levelData.LevelHeight = _levelHeight;
        levelData.LevelStartPositionX = _levelStartPositionX;
        levelData.LevelStartPositionY = _levelStartPositionY;
        levelData.LevelTheme = _levelTheme;
        levelData.LevelBackground = _levelBackground;
        levelData.NumberOfLemmings = _numberOfLemmings;
        levelData.SaveRequirement = _saveRequirement;
        levelData.TimeLimit = _timeLimit;
    }

    public void Dispose()
    {
    }
}