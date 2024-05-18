using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen : IBaseScreen
{
    private static LevelParameterSet _levelParameters = null!;
    private static TerrainManager _terrainManager = null!;
    private static TerrainPainter _terrainPainter = null!;
    private static LemmingManager _lemmingManager = null!;
    private static GadgetManager _gadgetManager = null!;
    private static SkillSetManager _skillSetManager = null!;
    private static LevelControlPanel _levelControlPanel = null!;
    private static UpdateScheduler _updateScheduler = null!;
    private static LevelCursor _levelCursor = null!;
    private static LevelInputController _levelInputController = null!;
    private static Viewport _levelViewport = null!;
    private static LevelScreenRenderer _levelScreenRenderer = null!;

    public static LevelParameterSet LevelParameters => _levelParameters;
    public static TerrainManager TerrainManager => _terrainManager;
    public static TerrainPainter TerrainPainter => _terrainPainter;
    public static LemmingManager LemmingManager => _lemmingManager;
    public static GadgetManager GadgetManager => _gadgetManager;
    public static SkillSetManager SkillSetManager => _skillSetManager;
    public static LevelControlPanel LevelControlPanel => _levelControlPanel;
    public static UpdateScheduler UpdateScheduler => _updateScheduler;
    public static LevelCursor LevelCursor => _levelCursor;
    public static LevelInputController LevelInputController => _levelInputController;
    public static Viewport LevelViewport => _levelViewport;
    public static LevelScreenRenderer LevelScreenRenderer => _levelScreenRenderer;

    public static int PixelChangeCount { get; set; }

    public static void SetLevelParameters(LevelParameterSet levelParameters)
    {
        _levelParameters = levelParameters;
    }

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        _terrainManager = terrainManager;
    }

    public static void SetTerrainPainter(TerrainPainter terrainPainter)
    {
        _terrainPainter = terrainPainter;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        _lemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        _gadgetManager = gadgetManager;
    }

    public static void SetSkillSetManager(SkillSetManager skillSetManager)
    {
        _skillSetManager = skillSetManager;
    }

    public static void SetLevelControlPanel(LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
    }

    public static void SetUpdateScheduler(UpdateScheduler updateScheduler)
    {
        _updateScheduler = updateScheduler;
    }

    public static void SetLevelCursor(LevelCursor levelCursor)
    {
        _levelCursor = levelCursor;
    }

    public static void SetLevelInputController(LevelInputController levelInputController)
    {
        _levelInputController = levelInputController;
    }

    public static void SetViewport(Viewport levelViewport)
    {
        _levelViewport = levelViewport;
    }

    public static void SetLevelScreenRenderer(LevelScreenRenderer levelScreenRenderer)
    {
        _levelScreenRenderer = levelScreenRenderer;
    }

    IScreenRenderer IBaseScreen.ScreenRenderer => _levelScreenRenderer;
    public string ScreenTitle { get; }
    public bool IsDisposed { get; private set; }

    public LevelScreen(LevelData levelData)
    {
        ScreenTitle = levelData.LevelTitle;

        PixelChangeCount = 0;
    }

    public void Tick(GameTime gameTime)
    {
        if (!IGameWindow.Instance.IsActive)
            return;

        _updateScheduler.Tick();
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _levelControlPanel.SetWindowDimensions(windowWidth, windowHeight);
        _levelViewport.SetWindowDimensions(windowWidth, windowHeight, _levelControlPanel.ScreenHeight);
        _levelScreenRenderer.OnWindowSizeChanged();

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

        DisposeOf(ref _levelParameters);
        DisposeOf(ref _terrainManager);
        DisposeOf(ref _lemmingManager);
        DisposeOf(ref _gadgetManager);
        DisposeOf(ref _skillSetManager);
        DisposeOf(ref _levelControlPanel);
        DisposeOf(ref _updateScheduler);
        DisposeOf(ref _levelCursor);
        DisposeOf(ref _levelInputController);
        DisposeOf(ref _levelViewport);
        DisposeOf(ref _levelScreenRenderer);

        IsDisposed = true;
    }

    private static void DisposeOf<T>(ref T obj)
        where T : class
    {
        if (obj is IDisposable disposable)
        {
            disposable.Dispose();
        }

        obj = null!;
    }
}