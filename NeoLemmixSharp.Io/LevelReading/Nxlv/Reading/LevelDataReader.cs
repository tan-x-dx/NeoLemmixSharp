using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Io.LevelReading.Nxlv.Reading;

public sealed class LevelDataReader : IDataReader
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

    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
        if (_indentedFormat)
            return;
        _levelData.LevelTitle = ReadingHelpers.ReadFormattedString(tokens[1..]);
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "TITLE":
                _levelData.LevelTitle = ReadingHelpers.ReadFormattedString(tokens[1..]); //string.Join(' ', tokens[1..]);
                break;

            case "AUTHOR":
                _levelData.LevelAuthor = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "ID":
                _levelData.LevelId = ReadingHelpers.ReadUlong(tokens[1]);
                break;

            case "VERSION":
                _levelData.Version = ReadingHelpers.ReadUlong(tokens[1]);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "THEME":
                _levelData.LevelTheme = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "BACKGROUND":
                _levelData.LevelBackground = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "MUSIC":

                break;

            case "WIDTH":
                _levelData.LevelWidth = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "LEMMINGS":
                _levelData.NumberOfLemmings = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "SAVE_REQUIREMENT":
                _levelData.SaveRequirement = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "TIME_LIMIT":
                _levelData.TimeLimit = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "SPAWN_INTERVAL_LOCKED":

                break;

            case "MAX_SPAWN_INTERVAL":
                _levelData.MaxSpawnInterval = ReadingHelpers.ReadInt(tokens[1]);

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
                    $"Unknown token when parsing LevelData ({indentedFormatString}): [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}