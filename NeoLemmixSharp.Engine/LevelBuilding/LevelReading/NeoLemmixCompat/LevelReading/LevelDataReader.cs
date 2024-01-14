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
                _levelData.LevelId = ReadingHelpers.ParseUnsignedNumericalValue<ulong>(secondToken);
                break;

            case "VERSION":
                _levelData.Version = ReadingHelpers.ParseUnsignedNumericalValue<ulong>(secondToken);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = int.Parse(secondToken);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = int.Parse(secondToken);
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
                _levelData.LevelWidth = int.Parse(secondToken);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = int.Parse(secondToken);
                break;

            case "LEMMINGS":
                _levelData.NumberOfLemmings = int.Parse(secondToken);
                break;

            case "SAVE_REQUIREMENT":
                _levelData.SaveRequirement = int.Parse(secondToken);
                break;

            case "TIME_LIMIT":
                _levelData.TimeLimit = int.Parse(secondToken);
                break;

            case "SPAWN_INTERVAL_LOCKED":

                break;

            case "MAX_SPAWN_INTERVAL":
                _levelData.MaxSpawnInterval = int.Parse(secondToken);

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