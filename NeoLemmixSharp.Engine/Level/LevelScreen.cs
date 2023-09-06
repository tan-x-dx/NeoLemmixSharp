using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen : IBaseScreen
{
    public static LevelScreen Current { get; private set; }

    public UpdateScheduler UpdateScheduler { get; }
    public LevelInputController InputController { get; }
    public LevelTimer LevelTimer { get; }
    public SkillSetManager SkillSetManager { get; }
    public ILevelControlPanel ControlPanel { get; }
    public LevelCursor LevelCursor { get; }
    public Viewport Viewport { get; }
    public LemmingManager LemmingManager { get; }
    public TerrainManager TerrainManager { get; }
    public GadgetManager GadgetManager { get; }
    public LevelRenderer ScreenRenderer { get; }

    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => ScreenRenderer;
    public string ScreenTitle { get; }
    public bool IsDisposed { get; private set; }

    public LevelScreen(
        LevelData levelData,
        UpdateScheduler updateScheduler,
        LevelInputController levelInputController,
        LevelTimer levelTimer,
        SkillSetManager skillSetManager,
        ILevelControlPanel controlPanel,
        LevelCursor cursor,
        Viewport viewport,
        LemmingManager lemmingManager,
        TerrainManager terrainManager,
        GadgetManager gadgetManager,
        LevelRenderer levelRenderer)
    {
        ScreenTitle = levelData.LevelTitle;

        UpdateScheduler = updateScheduler;
        InputController = levelInputController;
        LevelTimer = levelTimer;
        SkillSetManager = skillSetManager;
        ControlPanel = controlPanel;
        LevelCursor = cursor;
        Viewport = viewport;
        LemmingManager = lemmingManager;
        TerrainManager = terrainManager;
        GadgetManager = gadgetManager;
        ScreenRenderer = levelRenderer;

        Current = this;
        Orientation.SetTerrain(terrainManager);
        LemmingAction.SetHelpers(terrainManager, gadgetManager);
        LemmingSkill.SetTerrain(terrainManager);
        Lemming.SetHelpers(terrainManager, lemmingManager, gadgetManager);
        TerrainEraseMask.SetTerrain(terrainManager);
        TerrainAddMask.SetTerrain(terrainManager);
    }

    public void Tick()
    {
        if (!GameWindow.IsActive)
            return;

        UpdateScheduler.Tick();

        HandleKeyboardInput();

        return;
        /*

        _inputController.Tick();
        HandleKeyboardInput();
        // _levelCursor.OnNewFrame();
        // _lemmingManager.CheckLemmingsUnderCursor();

        _updateScheduler.CheckForQueuedAction();

        var shouldTickLemmings = false;// HandleMouseInput();

        if (!shouldTickLemmings)
            return;

        //    LevelTimer.Tick();

        for (var i = 0; i < _gadgets.Length; i++)
        {
            _gadgets[i].Tick();
        }

        // _lemmingManager.UpdateLemmings();

        _updateScheduler.Tick();
        */
    }

    private void HandleKeyboardInput()
    {
        if (InputController.Quit.IsPressed)
        {
            GameWindow.Escape();
        }

        if (InputController.ToggleFullScreen.IsPressed)
        {
            GameWindow.ToggleBorderless();
        }
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        ControlPanel.SetWindowDimensions(windowWidth, windowHeight);
        Viewport.SetWindowDimensions(windowWidth, windowHeight, ((LevelControlPanel)ControlPanel).ControlPanelScreenHeight);
        ScreenRenderer.OnWindowSizeChanged(windowWidth, windowHeight);
    }

    public void Dispose()
    {
#pragma warning disable CS8625
        Orientation.SetTerrain(null);
        LemmingAction.SetHelpers(null, null);
        LemmingSkill.SetTerrain(null);
        Lemming.SetHelpers(null, null, null);
        TerrainEraseMask.SetTerrain(null);
        TerrainAddMask.SetTerrain(null);

        ScreenRenderer.Dispose();
        IsDisposed = true;
        Current = null;
#pragma warning restore CS8625
    }
}