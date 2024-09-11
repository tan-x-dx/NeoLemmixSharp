using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen :
    IBaseScreen,
    IInitialisable,
    IItemManager<LemmingManager>,
    IItemManager<GadgetManager>
{
    public static LevelScreen Instance { get; private set; } = null!;

    private BoundaryBehaviour _horizontalBoundaryBehaviour = null!;
    private BoundaryBehaviour _verticalBoundaryBehaviour = null!;

    private LevelParameterSet _levelParameters = null!;
    private TerrainManager _terrainManager = null!;
    private TerrainPainter _terrainPainter = null!;
    private LemmingManager _lemmingManager = null!;
    private GadgetManager _gadgetManager = null!;
    private SkillSetManager _skillSetManager = null!;
    private LevelControlPanel _levelControlPanel = null!;
    private UpdateScheduler _updateScheduler = null!;
    private LevelCursor _levelCursor = null!;
    private LevelTimer _levelTimer = null!;
    private LevelInputController _levelInputController = null!;
    private Viewport _levelViewport = null!;
    private RewindManager _rewindManager = null!;
    private LevelScreenRenderer _levelScreenRenderer = null!;

    public static BoundaryBehaviour HorizontalBoundaryBehaviour => Instance._horizontalBoundaryBehaviour;
    public static BoundaryBehaviour VerticalBoundaryBehaviour => Instance._verticalBoundaryBehaviour;

    public static LevelParameterSet LevelParameters => Instance._levelParameters;
    public static TerrainManager TerrainManager => Instance._terrainManager;
    public static TerrainPainter TerrainPainter => Instance._terrainPainter;
    public static LemmingManager LemmingManager => Instance._lemmingManager;
    public static GadgetManager GadgetManager => Instance._gadgetManager;
    public static SkillSetManager SkillSetManager => Instance._skillSetManager;
    public static LevelControlPanel LevelControlPanel => Instance._levelControlPanel;
    public static UpdateScheduler UpdateScheduler => Instance._updateScheduler;
    public static LevelCursor LevelCursor => Instance._levelCursor;
    public static LevelTimer LevelTimer => Instance._levelTimer;
    public static LevelInputController LevelInputController => Instance._levelInputController;
    public static RewindManager RewindManager => Instance._rewindManager;
    public static Viewport LevelViewport => Instance._levelViewport;

    public void SetHorizontalBoundaryBehaviour(BoundaryBehaviour horizontalBoundaryBehaviour)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
    }

    public void SetVerticalBoundaryBehaviour(BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    public void SetLevelParameters(LevelParameterSet levelParameters)
    {
        _levelParameters = levelParameters;
    }

    public void SetTerrainManager(TerrainManager terrainManager)
    {
        _terrainManager = terrainManager;
    }

    public void SetTerrainPainter(TerrainPainter terrainPainter)
    {
        _terrainPainter = terrainPainter;
    }

    public void SetLemmingManager(LemmingManager lemmingManager)
    {
        _lemmingManager = lemmingManager;
    }

    public void SetGadgetManager(GadgetManager gadgetManager)
    {
        _gadgetManager = gadgetManager;
    }

    public void SetSkillSetManager(SkillSetManager skillSetManager)
    {
        _skillSetManager = skillSetManager;
    }

    public void SetLevelControlPanel(LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
    }

    public void SetUpdateScheduler(UpdateScheduler updateScheduler)
    {
        _updateScheduler = updateScheduler;
    }

    public void SetLevelCursor(LevelCursor levelCursor)
    {
        _levelCursor = levelCursor;
    }

    public void SetLevelTimer(LevelTimer levelTimer)
    {
        _levelTimer = levelTimer;
    }

    public void SetLevelInputController(LevelInputController levelInputController)
    {
        _levelInputController = levelInputController;
    }

    public void SetRewindManager(RewindManager rewindManager)
    {
        _rewindManager = rewindManager;
    }

    public void SetViewport(Viewport levelViewport)
    {
        _levelViewport = levelViewport;
    }

    public void SetLevelScreenRenderer(LevelScreenRenderer levelScreenRenderer)
    {
        _levelScreenRenderer = levelScreenRenderer;
    }

    public static int LevelWidth => HorizontalBoundaryBehaviour.LevelLength;
    public static int LevelHeight => VerticalBoundaryBehaviour.LevelLength;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            HorizontalBoundaryBehaviour.Normalise(levelPosition.X),
            VerticalBoundaryBehaviour.Normalise(levelPosition.Y));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return (uint)levelPosition.X >= (uint)HorizontalBoundaryBehaviour.LevelLength ||
               (uint)levelPosition.Y >= (uint)VerticalBoundaryBehaviour.LevelLength;
    }

    public IScreenRenderer ScreenRenderer => _levelScreenRenderer;
    public string ScreenTitle { get; }
    public bool IsDisposed { get; private set; }

    public LevelScreen(LevelData levelData)
    {
        ScreenTitle = levelData.LevelTitle;

        Instance = this;
    }

    public void Initialise()
    {
        var fields = typeof(LevelScreen)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var actualObject = field.GetValue(this);

            if (actualObject is null)
                throw new InvalidOperationException($"Field has not been initialised: {field.Name}");

            if (actualObject is IInitialisable objectToInitialise)
            {
                objectToInitialise.Initialise();
            }
        }
    }

    public void Tick(GameTime gameTime)
    {
        if (!IGameWindow.Instance.IsActive)
            return;

        UpdateScheduler.Tick();
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        LevelControlPanel.SetWindowDimensions(windowWidth, windowHeight);
        LevelViewport.SetWindowDimensions(windowWidth, windowHeight, LevelControlPanel.ScreenHeight);
        ScreenRenderer.OnWindowSizeChanged();

        IGameWindow.Instance.CaptureCursor();
    }

    public void OnActivated()
    {
        IGameWindow.Instance.CaptureCursor();
    }

    public void OnSetScreen()
    {
        IGameWindow.Instance.CaptureCursor();
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        var fields = typeof(LevelScreen)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var actualObject = field.GetValue(this);

            if (actualObject is IDisposable disposable)
            {
                disposable.Dispose();
            }

            field.SetValue(this, null);
        }

        Instance = null!;

        IsDisposed = true;
    }

    int IItemManager<LemmingManager>.NumberOfItems => 1;
    ReadOnlySpan<LemmingManager> IItemManager<LemmingManager>.AllItems => new(in _lemmingManager);

    int IItemManager<GadgetManager>.NumberOfItems => 1;
    ReadOnlySpan<GadgetManager> IItemManager<GadgetManager>.AllItems => new(in _gadgetManager);
}