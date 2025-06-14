using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class LevelData
{
    public string LevelTitle { get; set; } = string.Empty;
    public string LevelAuthor { get; set; } = string.Empty;
    public LevelIdentifier LevelId { get; set; }
    public LevelVersion Version { get; set; }

    private int _levelWidth = -1;
    private int _levelHeight = -1;
    private int? _levelStartPositionX;
    private int? _levelStartPositionY;
    private int _maxNumberOfClonedLemmings = -1;

    private ObjectiveData? _levelObjective;

    public FileFormatType FileFormatType { get; }

    public LevelData(FileFormatType fileFormatType)
    {
        FileFormatType = fileFormatType;
    }

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

    public Size LevelDimensions
    {
        get
        {
            if (_levelWidth < 0) throw new InvalidOperationException("Level width not set!");
            if (_levelHeight < 0) throw new InvalidOperationException("Level height not set!");

            return new Size(_levelWidth, _levelHeight);
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

    public void SetObjectiveData(ObjectiveData objectiveData)
    {
        _levelObjective = objectiveData;
    }

    public StyleIdentifier LevelTheme { get; set; }
    public BackgroundData? LevelBackground { get; set; }

    public BoundaryBehaviourType HorizontalBoundaryBehaviour { get; set; }
    public BoundaryBehaviourType VerticalBoundaryBehaviour { get; set; }

    public ObjectiveData LevelObjective => _levelObjective is null
        ? throw new InvalidOperationException("Level objective not set!")
        : _levelObjective;
    public BitArraySet<LevelParameterHasher, BitBuffer32, LevelParameters> LevelParameters { get; } = LevelParameterHasher.CreateBitArraySet();
    public BitArraySet<ControlPanelParameterHasher, BitBuffer32, ControlPanelParameters> ControlParameters { get; } = ControlPanelParameterHasher.CreateBitArraySet();

    public List<LemmingData> PrePlacedLemmingData { get; } = [];
    public List<LemmingData> HatchLemmingData { get; } = [];
    public List<TribeIdentifier> TribeIdentifiers { get; } = [];

    public List<TerrainData> AllTerrainData { get; } = [];
    public List<TerrainGroupData> AllTerrainGroups { get; } = [];
    public List<HatchGroupData> AllHatchGroupData { get; } = [];

    public List<GadgetData> AllGadgetData { get; } = [];
    public List<SketchData> AllSketchData { get; } = [];

    public List<string> PreTextLines { get; } = [];
    public List<string> PostTextLines { get; } = [];

    public void AssertLevelDataIsValid()
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

        if (TribeIdentifiers.Count == 0) return "Level tribes not set!";
        if (TribeIdentifiers.Count != TribeIdentifiers.Distinct().Count()) return "Non-unique tribes specified!";

        if (PrePlacedLemmingData.Count == 0 && HatchLemmingData.Count == 0) return "Number of lemmings is invalid!";
        if (LevelTitle.Length == 0) return "Level title not set!";
        if (LevelAuthor.Length == 0) return "Level author not set!";

        return null;
    }
}
