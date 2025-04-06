using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class LevelData
{
    private int _levelWidth = -1;
    private int _levelHeight = -1;
    private int? _levelStartPositionX;
    private int? _levelStartPositionY;
    private int _maxNumberOfClonedLemmings = -1;
    private int _numberOfTeams = -1;

    public string LevelTitle { get; set; } = string.Empty;
    public string LevelAuthor { get; set; } = string.Empty;
    public ulong LevelId { get; set; }
    public ulong Version { get; set; }

    public void SetLevelWidth(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EngineConstants.MaxLevelWidth);

        _levelWidth = value;
    }

    public void SetLevelHeight(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, EngineConstants.MaxLevelHeight);

        _levelHeight = value;
    }

    public LevelSize LevelDimensions
    {
        get
        {
            if (_levelWidth < 0) throw new InvalidOperationException("Level width not set!");
            if (_levelHeight < 0) throw new InvalidOperationException("Level height not set!");

            return new LevelSize(_levelWidth, _levelHeight);
        }
    }

    public int? LevelStartPositionX
    {
        get => _levelStartPositionX;
        set
        {
            if (!value.HasValue)
            {
                _levelStartPositionX = null;
                return;
            }

            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level start position X must be non-negative!");
            if (value >= _levelWidth)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level start position X too big!");

            _levelStartPositionX = value;
        }
    }

    public int? LevelStartPositionY
    {
        get => _levelStartPositionY;
        set
        {
            if (!value.HasValue)
            {
                _levelStartPositionY = null;
                return;
            }

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level start position Y must be non-negative!");
            if (value > _levelHeight)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level start position Y too big!");

            _levelStartPositionY = value;
        }
    }

    public int MaxNumberOfClonedLemmings
    {
        get => _maxNumberOfClonedLemmings;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Max number of cloned lemmings must be non-negative!");

            var totalNumberOfLemmings =
                value +
                PrePlacedLemmingData.Count +
                HatchLemmingData.Count;
            if (totalNumberOfLemmings > EngineConstants.MaxNumberOfLemmings)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Too many lemmings in level!");

            _maxNumberOfClonedLemmings = value;
        }
    }

    public int NumberOfTeams
    {
        get => _numberOfTeams;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Number of teams must be greater than zero!");
            if (value > EngineConstants.MaxNumberOfTeams)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Too many teams!");

            _numberOfTeams = value;
        }
    }

    public string LevelTheme { get; set; } = null!;
    public BackgroundData? LevelBackground { get; set; }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public List<LevelObjective> LevelObjectives { get; } = [];
    public LevelParameterSet LevelParameters { get; } = LevelParameterHasher.CreateBitArraySet();
    public ControlPanelParameterSet ControlParameters { get; } = ControlPanelParameterHasher.CreateBitArraySet();
    public Dictionary<StylePiecePair, TerrainArchetypeData> TerrainArchetypeData { get; }
    public List<TerrainData> AllTerrainData { get; } = [];
    public List<TerrainGroupData> AllTerrainGroups { get; } = [];
    public List<HatchGroupData> AllHatchGroupData { get; } = [];
    public List<LemmingData> PrePlacedLemmingData { get; } = [];
    public List<LemmingData> HatchLemmingData { get; } = [];
    public Dictionary<StylePiecePair, IGadgetArchetypeBuilder> AllGadgetArchetypeBuilders { get; }
    public List<GadgetData> AllGadgetData { get; } = [];
    public List<SketchData> AllSketchData { get; } = [];

    public List<string> PreTextLines { get; } = [];
    public List<string> PostTextLines { get; } = [];

    public LevelData()
    {
        var comparer = new StylePiecePairEqualityComparer();
        TerrainArchetypeData = new Dictionary<StylePiecePair, TerrainArchetypeData>(comparer);
        AllGadgetArchetypeBuilders = new Dictionary<StylePiecePair, IGadgetArchetypeBuilder>(comparer);
    }

    public void Validate()
    {
        var error = TryGetError();

        if (error is null)
            return;

        throw new InvalidOperationException(error);
    }

    private string? TryGetError()
    {
        if (_levelWidth < 0) return "Level width not set!";
        if (_levelHeight < 0) return "Level height not set!";
        if (_maxNumberOfClonedLemmings < 0) return "Cloner counts not evaluated!";
        if (_numberOfTeams < 0) return "Number of teams not set!";
        if (PrePlacedLemmingData.Count == 0 && HatchLemmingData.Count == 0) return "Number of lemmings is invalid!";
        if (LevelTitle.Length == 0) return "Level title not set!";
        if (LevelAuthor.Length == 0) return "Level author not set!";
        if (LevelObjectives.Count == 0) return "Level objectives not set!";

        return null;
    }
}