using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class LevelDataReader : INeoLemmixDataReader
{
    private readonly LevelData _levelData;
    private readonly bool _indentedFormat;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => _indentedFormat
        ? "$LEVELDATA"
        : "TITLE";

    public LevelDataReader(
        LevelData levelData,
        bool indentedFormat)
    {
        _levelData = levelData;
        _indentedFormat = indentedFormat;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        if (_indentedFormat)
            return;

        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var rest = line[(1 + firstTokenIndex + firstToken.Length)..];
        _levelData.LevelTitle = rest.ToString();
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        var rest = line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "TITLE":
                _levelData.LevelTitle = rest.ToString();
                break;

            case "AUTHOR":
                _levelData.LevelAuthor = rest.ToString();
                break;

            case "ID":
                _levelData.LevelId = ReadingHelpers.ReadUlong(secondToken);
                break;

            case "VERSION":
                _levelData.Version = ReadingHelpers.ReadUlong(secondToken);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = ReadingHelpers.ReadInt(secondToken);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = ReadingHelpers.ReadInt(secondToken);
                break;

            case "THEME":
                _levelData.LevelTheme = rest.ToString();
                break;

            case "BACKGROUND":
                _levelData.LevelBackground = rest.ToString();
                break;

            case "MUSIC":

                break;

            case "WIDTH":
                _levelData.LevelWidth = ReadingHelpers.ReadInt(secondToken);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = ReadingHelpers.ReadInt(secondToken);
                break;

            case "LEMMINGS":
                _levelData.NumberOfLemmings = ReadingHelpers.ReadInt(secondToken);
                break;

            case "SAVE_REQUIREMENT":
                _levelData.SaveRequirement = ReadingHelpers.ReadInt(secondToken);
                break;

            case "TIME_LIMIT":
                _levelData.TimeLimit = ReadingHelpers.ReadInt(secondToken);
                break;

            case "SPAWN_INTERVAL_LOCKED":

                break;

            case "MAX_SPAWN_INTERVAL":
                _levelData.MaxSpawnInterval = ReadingHelpers.ReadInt(secondToken);

                if (_indentedFormat)
                    break;

                FinishedReading = true;
                break;

            case "$END":
                FinishedReading = true;
                break;

            default:
                var indentedFormatString = _indentedFormat
                    ? "Indented"
                    : "Not indented";
                throw new InvalidOperationException(
                    $"Unknown token when parsing LevelData ({indentedFormatString}): [{firstToken}] line: \"{line}\"");
        }
    }
}