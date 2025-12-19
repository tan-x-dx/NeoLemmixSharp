using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen : IBaseScreen
{
    public static LevelScreen Instance { get; private set; } = null!;

    public LevelData LevelData { get; }

    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly LevelParameterSet _levelParameters;
    private readonly TerrainManager _terrainManager;
    private readonly TerrainPainter _terrainPainter;
    private readonly LemmingManager _lemmingManager;
    private readonly GadgetManager _gadgetManager;
    private readonly TribeManager _tribeManager;
    private readonly SkillSetManager _skillSetManager;
    private readonly LevelObjectiveManager _levelObjectiveManager;
    private readonly LevelControlPanel _levelControlPanel;
    private readonly UpdateScheduler _updateScheduler;
    private readonly LevelCursor _levelCursor;
    private readonly LevelTimer _levelTimer;
    private readonly LevelInputController _levelInputController;
    private readonly Viewport _levelViewport;
    private readonly RewindManager _rewindManager;
    private readonly LemmingSpriteBank _lemmingSpriteBank;
    private readonly LevelScreenRenderer _levelScreenRenderer;

    private bool _isDisposed;

    public static BoundaryBehaviour HorizontalBoundaryBehaviour => Instance._horizontalBoundaryBehaviour;
    public static BoundaryBehaviour VerticalBoundaryBehaviour => Instance._verticalBoundaryBehaviour;

    public static LevelParameterSet LevelParameters => Instance._levelParameters;
    public static TerrainManager TerrainManager => Instance._terrainManager;
    public static TerrainPainter TerrainPainter => Instance._terrainPainter;
    public static ref readonly LemmingManager LemmingManager => ref Instance._lemmingManager;
    public static ref readonly GadgetManager GadgetManager => ref Instance._gadgetManager;
    public static TribeManager TribeManager => Instance._tribeManager;
    public static SkillSetManager SkillSetManager => Instance._skillSetManager;
    public static LevelObjectiveManager LevelObjectiveManager => Instance._levelObjectiveManager;
    public static LevelControlPanel LevelControlPanel => Instance._levelControlPanel;
    public static UpdateScheduler UpdateScheduler => Instance._updateScheduler;
    public static LevelCursor LevelCursor => Instance._levelCursor;
    public static ref readonly LevelTimer LevelTimer => ref Instance._levelTimer;
    public static LevelInputController LevelInputController => Instance._levelInputController;
    public static RewindManager RewindManager => Instance._rewindManager;
    public static LemmingSpriteBank LemmingSpriteBank => Instance._lemmingSpriteBank;
    public static Viewport LevelViewport => Instance._levelViewport;

    [Pure]
    public static Point NormalisePosition(Point levelPosition)
    {
        return new Point(
            HorizontalBoundaryBehaviour.Normalise(levelPosition.X),
            VerticalBoundaryBehaviour.Normalise(levelPosition.Y));
    }

    [Pure]
    public static Point GetNormalisedDelta(Point p0, Point p1)
    {
        return new Point(
            HorizontalBoundaryBehaviour.GetNormalisedDelta(p0.X, p1.X),
            VerticalBoundaryBehaviour.GetNormalisedDelta(p0.Y, p1.Y));
    }

    [Pure]
    public static bool RegionContainsPoint(RectangularRegion region, Point point)
    {
        return HorizontalBoundaryBehaviour.IntervalContainsPoint(region.GetHorizontalInterval(), point.X) &&
               VerticalBoundaryBehaviour.IntervalContainsPoint(region.GetVerticalInterval(), point.Y);
    }

    [Pure]
    public static bool RegionContainsEitherPoint(RectangularRegion region, Point p1, Point p2)
    {
        var horizontalInterval = region.GetHorizontalInterval();
        var verticalInterval = region.GetVerticalInterval();
        var horizontalBoundaryBehaviour = HorizontalBoundaryBehaviour;
        var verticalBoundaryBehaviour = VerticalBoundaryBehaviour;

        return (horizontalBoundaryBehaviour.IntervalContainsPoint(horizontalInterval, p1.X) &&
                verticalBoundaryBehaviour.IntervalContainsPoint(verticalInterval, p1.Y)) ||
               (horizontalBoundaryBehaviour.IntervalContainsPoint(horizontalInterval, p2.X) &&
                verticalBoundaryBehaviour.IntervalContainsPoint(verticalInterval, p2.Y));
    }

    [Pure]
    public static bool RegionsOverlap(RectangularRegion r1, RectangularRegion r2)
    {
        return HorizontalBoundaryBehaviour.IntervalsOverlap(r1.GetHorizontalInterval(), r2.GetHorizontalInterval()) &&
               VerticalBoundaryBehaviour.IntervalsOverlap(r1.GetVerticalInterval(), r2.GetVerticalInterval());
    }

    public IScreenRenderer ScreenRenderer => _levelScreenRenderer;
    public string ScreenTitle => LevelData.LevelTitle;

    public LevelScreen(
        LevelData levelData,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour,
        LevelParameterSet levelParameters,
        TerrainManager terrainManager,
        TerrainPainter terrainPainter,
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        TribeManager tribeManager,
        SkillSetManager skillSetManager,
        LevelObjectiveManager levelObjectiveManager,
        LevelControlPanel levelControlPanel,
        UpdateScheduler updateScheduler,
        LevelCursor levelCursor,
        LevelTimer levelTimer,
        LevelInputController levelInputController,
        Viewport levelViewport,
        RewindManager rewindManager,
        LemmingSpriteBank lemmingSpriteBank,
        LevelScreenRenderer levelScreenRenderer)
    {
        LevelData = levelData;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _levelParameters = levelParameters;
        _terrainManager = terrainManager;
        _terrainPainter = terrainPainter;
        _lemmingManager = lemmingManager;
        _gadgetManager = gadgetManager;
        _tribeManager = tribeManager;
        _skillSetManager = skillSetManager;
        _levelObjectiveManager = levelObjectiveManager;
        _levelControlPanel = levelControlPanel;
        _updateScheduler = updateScheduler;
        _levelCursor = levelCursor;
        _levelTimer = levelTimer;
        _levelInputController = levelInputController;
        _levelViewport = levelViewport;
        _rewindManager = rewindManager;
        _lemmingSpriteBank = lemmingSpriteBank;
        _levelScreenRenderer = levelScreenRenderer;

        Instance = this;
    }

    public void Initialise()
    {
        this.InitialiseFields();
    }

    public void Tick(Microsoft.Xna.Framework.GameTime gameTime)
    {
        if (!IGameWindow.Instance.IsActive)
            return;

        _updateScheduler.Tick();
    }

    public void OnWindowSizeChanged()
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        _levelControlPanel.SetWindowDimensions(windowSize);
        _levelViewport.SetWindowDimensions(windowSize, _levelControlPanel.ControlPanelScreenSize.H);
        _levelScreenRenderer.OnWindowSizeChanged();

        IGameWindow.Instance.CaptureCursor();
    }

    public void OnActivated()
    {
        IGameWindow.Instance.CaptureCursor();
    }

    public void ExitLevel()
    {
        IGameWindow.Instance.ExitLevel();

        Dispose();
        _levelScreenRenderer.Dispose();
    }

    public object GenerateSuccessOutcome()
    {
        return null!;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            this.DisposeOfFields();
            TextureCache.DisposeOfLevelSpecificTextures();

            Instance = null!;
        }

        GC.SuppressFinalize(this);
    }
}
