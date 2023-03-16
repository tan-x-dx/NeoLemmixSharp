using NeoLemmixSharp.LevelBuilding.Data;
using System;

namespace NeoLemmixSharp.LevelBuilding.Reading;

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
        _levelData.LevelTitle = string.Join(' ', tokens[1..]);
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "TITLE":
                _levelData.LevelTitle = string.Join(' ', tokens[1..]);
                break;

            case "AUTHOR":
                _levelData.LevelAuthor = string.Join(' ', tokens[1..]);
                break;

            case "ID":
                var idHexPart = tokens[1];
                if (idHexPart[0] == 'x')
                {
                    idHexPart = $"0{idHexPart}";
                }
                _levelData.LevelId = Convert.ToUInt64(idHexPart, 16);
                break;

            case "VERSION":
                var versionHexPart = tokens[1];
                if (versionHexPart[0] == 'x')
                {
                    versionHexPart = $"0{versionHexPart}";
                }
                _levelData.Version = Convert.ToUInt64(versionHexPart, 16);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = int.Parse(tokens[1]);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = int.Parse(tokens[1]);
                break;

            case "THEME":
                _levelData.LevelTheme = string.Join(' ', tokens[1..]);
                break;

            case "BACKGROUND":
                _levelData.LevelBackground = string.Join(' ', tokens[1..]);
                break;

            case "MUSIC":

                break;

            case "WIDTH":
                _levelData.LevelWidth = int.Parse(tokens[1]);
                break;

            case "HEIGHT":
                _levelData.LevelHeight = int.Parse(tokens[1]);
                break;

            case "LEMMINGS":
                _levelData.NumberOfLemmings = int.Parse(tokens[1]);
                break;

            case "SAVE_REQUIREMENT":
                _levelData.SaveRequirement = int.Parse(tokens[1]);
                break;

            case "TIME_LIMIT":
                _levelData.TimeLimit = int.Parse(tokens[1]);
                break;

            case "SPAWN_INTERVAL_LOCKED":

                break;

            case "MAX_SPAWN_INTERVAL":
                _levelData.MaxSpawnInterval = int.Parse(tokens[1]);

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