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

    private readonly Gadget[] _gadgets;

    private readonly UpdateScheduler _updateScheduler;
    private readonly LevelInputController _inputController;
    private readonly LevelTimer _levelTimer;
    private readonly SkillSetManager _skillSetManager;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelCursor _levelCursor;
    private readonly Viewport _viewport;
    private readonly LemmingManager _lemmingManager;
    private readonly TerrainManager _terrainManager;
    private readonly GadgetManager _gadgetManager;
    private readonly LevelRenderer _screenRenderer;

    public bool IsDisposed { get; set; }
    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => _screenRenderer;
    public string ScreenTitle { get; init; }

    public LevelTimer LevelTimer => _levelTimer;
    public GadgetManager GadgetManager => _gadgetManager;

    public LevelScreen(
        LevelData levelData,
        Gadget[] gadgets,
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

        _gadgets = gadgets;

        _updateScheduler = updateScheduler;
        _inputController = levelInputController;
        _levelTimer = levelTimer;
        _skillSetManager = skillSetManager;
        _controlPanel = controlPanel;
        _levelCursor = cursor;
        _viewport = viewport;
        _lemmingManager = lemmingManager;
        _terrainManager = terrainManager;
        _gadgetManager = gadgetManager;
        _screenRenderer = levelRenderer;

        Orientation.SetTerrain(terrainManager);
        LemmingAction.SetHelpers(terrainManager, gadgetManager);
        LemmingSkill.SetTerrain(terrainManager);
        TerrainEraseMask.SetTerrain(terrainManager);
        TerrainAddMask.SetTerrain(terrainManager);
        LevelCursor.LevelScreen = this;
        Current = this;
    }

    public void Tick()
    {
        if (!GameWindow.IsActive)
            return;

        _updateScheduler.Tick();

        HandleKeyboardInput();

        return;


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
    }

    private void HandleKeyboardInput()
    {
        if (_inputController.Quit.IsPressed)
        {
            GameWindow.Escape();
        }

        if (_inputController.ToggleFullScreen.IsPressed)
        {
            GameWindow.ToggleBorderless();
        }
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _controlPanel.SetWindowDimensions(windowWidth, windowHeight);
        _viewport.SetWindowDimensions(windowWidth, windowHeight, ((LevelControlPanel)_controlPanel).ControlPanelScreenHeight);
        _screenRenderer.OnWindowSizeChanged(windowWidth, windowHeight);
    }

    public void Dispose()
    {
#pragma warning disable CS8625
        Orientation.SetTerrain(null);
        LemmingAction.SetHelpers(null, null);
        LemmingSkill.SetTerrain(null);
        TerrainEraseMask.SetTerrain(null);
        TerrainAddMask.SetTerrain(null);
        LevelCursor.LevelScreen = null;

        _screenRenderer.Dispose();
        Current = null;
#pragma warning restore CS8625
    }
}