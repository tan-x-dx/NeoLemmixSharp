using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen : IBaseScreen
{
    public static LevelScreen Current { get; private set; } = null!;

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

        LemmingManager.Initialise();
        GadgetManager.Initialise();
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

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        Orientation.SetTerrainManager(terrainManager);
        LemmingAction.SetTerrainManager(terrainManager);
        LemmingSkill.SetTerrainManager(terrainManager);
        Lemming.SetTerrainManager(terrainManager);
        LemmingMovementHelper.SetTerrainManager(terrainManager);
        TerrainEraseMask.SetTerrainManager(terrainManager);
        TerrainAddMask.SetTerrainManager(terrainManager);
        GadgetBase.SetTerrainManager(terrainManager);
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        LemmingAction.SetLemmingManager(lemmingManager);
        Lemming.SetLemmingManager(lemmingManager);
        LemmingState.SetLemmingManager(lemmingManager);
        GadgetBase.SetLemmingManager(lemmingManager);
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        LemmingAction.SetGadgetManager(gadgetManager);
        Lemming.SetGadgetManager(gadgetManager);
        GadgetBase.SetGadgetManager(gadgetManager);
    }

    public static void SetSkillSetManager(SkillSetManager skillSetManager)
    {
        SkillModifierBehaviour.SetSkillSetManager(skillSetManager);
    }

    public void Dispose()
    {
#pragma warning disable CS8625
        SetTerrainManager(null);
        SetLemmingManager(null);
        SetGadgetManager(null);
        SetSkillSetManager(null);

        ScreenRenderer.Dispose();
        IsDisposed = true;
        Current = null;
#pragma warning restore CS8625
    }
}