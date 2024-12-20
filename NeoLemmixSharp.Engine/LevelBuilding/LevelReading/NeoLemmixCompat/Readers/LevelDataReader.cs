using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class LevelDataReader : NeoLemmixDataReader
{
    private readonly LevelData _levelData;

    private bool _lockSpawnInterval;
    private int _numberOfLemmings;
    private int _maxSpawnInterval;
    private int _saveRequirement;
    private int? _timeLimitInSeconds;

    public int SaveRequirement => _saveRequirement;
    public int? TimeLimitInSeconds => _timeLimitInSeconds;
    public int NumberOfLemmings => _numberOfLemmings;

    public LevelDataReader(
        LevelData levelData)
        : base("TITLE")
    {
        _levelData = levelData;

        RegisterTokenAction("AUTHOR", SetAuthor);
        RegisterTokenAction("ID", SetId);
        RegisterTokenAction("VERSION", SetVersion);
        RegisterTokenAction("START_X", SetStartX);
        RegisterTokenAction("START_Y", SetStartY);
        RegisterTokenAction("THEME", SetTheme);
        RegisterTokenAction("BACKGROUND", SetBackground);
        RegisterTokenAction("MUSIC", SetMusic);
        RegisterTokenAction("WIDTH", SetWidth);
        RegisterTokenAction("HEIGHT", SetHeight);
        RegisterTokenAction("LEMMINGS", SetNumberOfLemmings);
        RegisterTokenAction("SAVE_REQUIREMENT", SetSaveRequirement);
        RegisterTokenAction("TIME_LIMIT", SetTimeLimit);
        RegisterTokenAction("SPAWN_INTERVAL_LOCKED", SetLockedSpawnInterval);
        RegisterTokenAction("MAX_SPAWN_INTERVAL", SetMaxSpawnInterval);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        NxlvReadingHelpers.GetTokenPair(line, out _, out _, out var secondTokenIndex);

        var levelTitle = line.TrimAfterIndex(secondTokenIndex).ToString();
        _levelData.LevelTitle = string.IsNullOrWhiteSpace(levelTitle) ? "Untitled" : levelTitle;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken[0] == '$')
        {
            OnFinishedReading();
            FinishedReading = true;
            return true;
        }

        var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
        {
            tokenAction(line, secondToken, secondTokenIndex);
        }
        else
        {
            NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
        }

        return false;
    }

    private void SetAuthor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelAuthor = line.TrimAfterIndex(secondTokenIndex).ToString();
        _levelData.LevelAuthor = string.IsNullOrWhiteSpace(levelAuthor) ? "Unknown Author" : levelAuthor;
    }

    private void SetId(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelId = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
    }

    private void SetVersion(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.Version = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
    }

    private void SetStartX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelStartPositionX = int.Parse(secondToken);
    }

    private void SetStartY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelStartPositionY = int.Parse(secondToken);
    }

    private void SetTheme(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelTheme = line.TrimAfterIndex(secondTokenIndex).ToString();
    }

    private void SetBackground(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelBackground = ParseBackgroundData(line.TrimAfterIndex(secondTokenIndex));
    }

    private void SetMusic(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelWidth = int.Parse(secondToken);
    }

    private void SetHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelHeight = int.Parse(secondToken);
    }

    private void SetNumberOfLemmings(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _numberOfLemmings = int.Parse(secondToken);
    }

    private void SetSaveRequirement(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _saveRequirement = int.Parse(secondToken);
    }

    private void SetTimeLimit(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var timeLimitInSeconds = int.Parse(secondToken);

        if (timeLimitInSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeLimitInSeconds), timeLimitInSeconds, "Time limit must be positive!");
        if (timeLimitInSeconds > EngineConstants.MaxTimeLimitInSeconds)
            throw new ArgumentOutOfRangeException(nameof(timeLimitInSeconds), timeLimitInSeconds, "Time limit too big!");

        _timeLimitInSeconds = timeLimitInSeconds;
    }

    private void SetLockedSpawnInterval(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _lockSpawnInterval = true;
    }

    private void SetMaxSpawnInterval(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _maxSpawnInterval = int.Parse(secondToken);
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
                : EngineConstants.MinAllowedSpawnInterval
        };

        _levelData.AllHatchGroupData.Add(hatchGroupData);
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