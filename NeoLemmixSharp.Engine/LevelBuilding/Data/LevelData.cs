using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class LevelData
{
    private int _levelWidth = -1;
    private int _levelHeight = -1;
    private int? _levelStartPositionX;
    private int? _levelStartPositionY;
    private int _numberOfLemmings = -1;
    private int? _timeLimit;

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
    public string LevelBackground { get; set; } = null!;

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

    public int SaveRequirement { get; set; }
    public int? TimeLimit
    {
        get => _timeLimit;
        set
        {
            if (!value.HasValue)
            {
                _timeLimit = null;
                return;
            }

            var timeLimitValue = value.Value;

            if (timeLimitValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), timeLimitValue, "Time limit must be positive!");
            if (timeLimitValue > LevelConstants.MaxTimeLimitInSeconds)
                throw new ArgumentOutOfRangeException(nameof(value), timeLimitValue, "Time limit too big!");

            _timeLimit = timeLimitValue;
        }
    }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public BoundaryBehaviourType HorizontalViewPortBehaviour { get; set; }
    public BoundaryBehaviourType VerticalViewPortBehaviour { get; set; }

    public LevelParameterSet LevelParameters { get; } = LevelParameterHelpers.CreateSimpleSet();
    public ControlPanelParameterSet ControlParameters { get; } = ControlPanelParameterHelpers.CreateSimpleSet();
    public List<SkillSetData> SkillSetData { get; } = new();
    public ThemeData ThemeData { get; } = new();
    public List<TerrainArchetypeData> TerrainArchetypeData { get; } = new();
    public List<TerrainData> AllTerrainData { get; } = new();
    public List<TerrainGroup> AllTerrainGroups { get; } = new();
    public List<HatchGroupData> AllHatchGroupData { get; } = new();
    public List<LemmingData> AllLemmingData { get; } = new();
    public Dictionary<int, IGadgetBuilder> AllGadgetBuilders { get; } = new();
    public List<GadgetData> AllGadgetData { get; } = new();
    public List<SketchData> AllSketchData { get; } = new();

    public List<string> PreTextLines { get; } = new();
    public List<string> PostTextLines { get; } = new();

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
        if (AllLemmingData.Count == 0) return "Number of lemmings is invalid!";
        if (LevelTitle.Length == 0) return "Level title not set!";
        if (LevelAuthor.Length == 0) return "Level author not set!";

        return null;
    }

    public bool LevelContainsAnyZombies()
    {
        var lemmingSpan = CollectionsMarshal.AsSpan(AllLemmingData);

        foreach (var lemmingData in lemmingSpan)
        {
            var zombieFlag = lemmingData.State >> LemmingState.ZombieBitIndex;

            if ((zombieFlag & 1U) != 0U)
                return true;
        }

        foreach (var gadgetData in AllGadgetData)
        {
            var gadgetBuilderId = gadgetData.GadgetBuilderId;
            var gadgetBuilder = AllGadgetBuilders[gadgetBuilderId];

            if (gadgetBuilder is HatchGadgetBuilder)
            {
                if (HatchIsZombieHatch(gadgetData))
                    return true;
            }
        }

        return false;

        static bool HatchIsZombieHatch(GadgetData hatchGadgetData)
        {
            if (!hatchGadgetData.TryGetProperty(GadgetProperty.RawLemmingState, out var rawLemmingState))
                return false;

            var zombieFlag = (uint)rawLemmingState >> LemmingState.ZombieBitIndex;

            return (zombieFlag & 1U) != 0U;
        }
    }
}