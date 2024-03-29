﻿using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class LevelDataReader : INeoLemmixDataReader
{
    private readonly LevelData _levelData;

    private bool _lockSpawnInterval;
    private int _maxSpawnInterval;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "TITLE";

    public LevelDataReader(LevelData levelData)
    {
        _levelData = levelData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        ReadingHelpers.GetTokenPair(line, out _, out _, out var secondTokenIndex);

        var levelTitle = line.TrimAfterIndex(secondTokenIndex).GetString();
        _levelData.LevelTitle = string.IsNullOrWhiteSpace(levelTitle) ? "Untitled" : levelTitle;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken[0] == '$')
        {
            OnFinishedReading();
            FinishedReading = true;
            return true;
        }

        switch (firstToken)
        {
            case "AUTHOR":
                var levelAuthor = line.TrimAfterIndex(secondTokenIndex).GetString();
                _levelData.LevelAuthor = string.IsNullOrWhiteSpace(levelAuthor) ? "Unknown Author" : levelAuthor;
                break;

            case "ID":
                _levelData.LevelId = ReadingHelpers.ParseHex<ulong>(secondToken);
                break;

            case "VERSION":
                _levelData.Version = ReadingHelpers.ParseHex<ulong>(secondToken);
                break;

            case "START_X":
                _levelData.LevelStartPositionX = int.Parse(secondToken);
                break;

            case "START_Y":
                _levelData.LevelStartPositionY = int.Parse(secondToken);
                break;

            case "THEME":
                _levelData.LevelTheme = line.TrimAfterIndex(secondTokenIndex).GetString();
                break;

            case "BACKGROUND":
                _levelData.LevelBackground = line.TrimAfterIndex(secondTokenIndex).GetString();
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
                _lockSpawnInterval = true;
                break;

            case "MAX_SPAWN_INTERVAL":
                _maxSpawnInterval = int.Parse(secondToken);
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("Level Data", firstToken, line);
                break;
        }

        return false;
    }

    private void OnFinishedReading()
    {
        _levelData.HorizontalBoundaryBehaviour = BoundaryBehaviourType.Void;
        _levelData.VerticalBoundaryBehaviour = BoundaryBehaviourType.Void;
        _levelData.HorizontalViewPortBehaviour = BoundaryBehaviourType.Void;
        _levelData.VerticalViewPortBehaviour = BoundaryBehaviourType.Void;

        var hatchGroupData = new HatchGroupData
        {
            MaxSpawnInterval = _maxSpawnInterval,
            InitialSpawnInterval = _maxSpawnInterval,
            MinSpawnInterval = _lockSpawnInterval
                ? _maxSpawnInterval
                : LevelConstants.MinAllowedSpawnInterval
        };

        _levelData.AllHatchGroupData.Add(hatchGroupData);
    }
}