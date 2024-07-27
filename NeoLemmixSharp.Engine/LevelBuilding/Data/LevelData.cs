using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util.Collections;
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
    private int _numberOfLemmings = -1;

    public string LevelTitle { get; set; } = string.Empty;
    public string LevelAuthor { get; set; } = string.Empty;
    public ulong LevelId { get; set; }
    public ulong Version { get; set; }

    public int LevelWidth
    {
        get => _levelWidth;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level width must be positive!");
            if (value > LevelConstants.MaxLevelWidth)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level width too big!");

            _levelWidth = value;
        }
    }

    public int LevelHeight
    {
        get => _levelHeight;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level height must be positive!");
            if (value > LevelConstants.MaxLevelHeight)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level height too big!");

            _levelHeight = value;
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
            if (value >= LevelWidth)
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
            if (value > LevelHeight)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Level start position Y too big!");

            _levelStartPositionY = value;
        }
    }

    public string LevelTheme { get; set; } = null!;
    public BackgroundData? LevelBackground { get; set; }

    public int NumberOfLemmings
    {
        get => _numberOfLemmings;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Number of lemmings must be positive!");
            if (value > LevelConstants.MaxNumberOfLemmings)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Number of lemmings too big!");

            _numberOfLemmings = value;
        }
    }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public List<LevelObjective> LevelObjectives { get; } = [];
    public LevelParameterSet LevelParameters { get; } = PerfectEnumHasher<LevelParameters>.CreateSimpleSet();
    public ControlPanelParameterSet ControlParameters { get; } = PerfectEnumHasher<ControlPanelParameters>.CreateSimpleSet();
    public List<TerrainArchetypeData> TerrainArchetypeData { get; } = [];
    public List<TerrainData> AllTerrainData { get; } = [];
    public List<TerrainGroupData> AllTerrainGroups { get; } = [];
    public List<HatchGroupData> AllHatchGroupData { get; } = [];
    public List<LemmingData> PrePlacedLemmingData { get; } = [];
    public List<LemmingData> HatchLemmingData { get; } = [];
    public Dictionary<int, IGadgetBuilder> AllGadgetBuilders { get; } = [];
    public List<GadgetData> AllGadgetData { get; } = [];
    public List<SketchData> AllSketchData { get; } = [];

    public List<string> PreTextLines { get; } = [];
    public List<string> PostTextLines { get; } = [];

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
        if (_numberOfLemmings <= 0) return "Number of lemmings is invalid!";
        if (PrePlacedLemmingData.Count == 0 && HatchLemmingData.Count == 0) return "Number of lemmings is invalid!";
        if (LevelTitle.Length == 0) return "Level title not set!";
        if (LevelAuthor.Length == 0) return "Level author not set!";
        if (LevelObjectives.Count == 0) return "Level objectives not set!";

        return null;
    }
}