using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class LevelDataReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;
    private readonly LevelData _levelData;

    private bool _lockSpawnInterval;
    private int _numberOfLemmings;
    private uint _maxSpawnInterval;
    private int _saveRequirement;
    private int? _timeLimitInSeconds;

    public int SaveRequirement => _saveRequirement;
    public int? TimeLimitInSeconds => _timeLimitInSeconds;
    public int NumberOfLemmings => _numberOfLemmings;

    public LevelDataReader(
        UniqueStringSet uniqueStringSet,
        LevelData levelData)
        : base("TITLE")
    {
        _uniqueStringSet = uniqueStringSet;
        _levelData = levelData;

        SetNumberOfTokens(16);

        RegisterTokenAction("TITLE", SetTitle);
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

        return true;
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

        return ProcessTokenPair(line, firstToken, secondToken, secondTokenIndex);
    }

    private void SetTitle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelTitle = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
        _levelData.LevelAuthor = string.IsNullOrWhiteSpace(levelTitle) ? "Untitled" : levelTitle;
    }

    private void SetAuthor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var levelAuthor = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
        _levelData.LevelAuthor = string.IsNullOrWhiteSpace(levelAuthor) ? "Unknown Author" : levelAuthor;
    }

    private void SetId(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var id = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
        _levelData.LevelId = new LevelIdentifier(id);
    }

    private void SetVersion(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var version = NxlvReadingHelpers.ParseHex<ulong>(secondToken);
        _levelData.Version = new LevelVersion(version);
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
        _levelData.LevelStyle = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
    }

    private void SetBackground(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.LevelBackground = ParseBackgroundData(line[secondTokenIndex..]);
    }

    private void SetMusic(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.SetLevelWidth(int.Parse(secondToken));
    }

    private void SetHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _levelData.SetLevelHeight(int.Parse(secondToken));
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

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeLimitInSeconds);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(timeLimitInSeconds, EngineConstants.MaxTimeLimitInSeconds);

        _timeLimitInSeconds = timeLimitInSeconds;
    }

    private void SetLockedSpawnInterval(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _lockSpawnInterval = true;
    }

    private void SetMaxSpawnInterval(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _maxSpawnInterval = uint.Parse(secondToken);
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

    private BackgroundData ParseBackgroundData(ReadOnlySpan<char> backgroundToken)
    {
        return new BackgroundData(_uniqueStringSet.GetUniqueStringInstance(backgroundToken));
    }
}
