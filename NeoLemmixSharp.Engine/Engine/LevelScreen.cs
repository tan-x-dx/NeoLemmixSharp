using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Engine.Engine.Terrain;
using NeoLemmixSharp.Engine.Engine.Terrain.Masks;
using NeoLemmixSharp.Engine.Engine.Timer;
using NeoLemmixSharp.Engine.Engine.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelScreen : IBaseScreen
{
    public static LevelScreen Current { get; private set; }

    private readonly IGadget[] _gadgets;

    private readonly UpdateScheduler _updateScheduler;
    private readonly LevelInputController _inputController;
    private readonly LevelTimer _levelTimer;
    private readonly SkillSetManager _skillSetManager;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelCursor _levelCursor;
    private readonly Viewport _viewport;
    private readonly LemmingManager _lemmingManager;
    private readonly TerrainManager _terrain;
    private readonly LevelRenderer _screenRenderer;

    public bool IsDisposed { get; set; }
    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => _screenRenderer;
    public string ScreenTitle { get; init; }

    public LevelTimer LevelTimer => _levelTimer;

    public LevelScreen(
        LevelData levelData,
        IGadget[] gadgets,
        UpdateScheduler updateScheduler,
        LevelInputController levelInputController,
        LevelTimer levelTimer,
        SkillSetManager skillSetManager,
        ILevelControlPanel controlPanel,
        LevelCursor cursor,
        Viewport viewport,
        LemmingManager lemmingManager,
        TerrainManager terrain,
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
        _terrain = terrain;
        _screenRenderer = levelRenderer;

        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);
        TerrainEraseMask.SetTerrain(terrain);
        TerrainAddMask.SetTerrain(terrain);
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
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
        TerrainEraseMask.SetTerrain(null);
        TerrainAddMask.SetTerrain(null);
        LevelCursor.LevelScreen = null;

        _screenRenderer.Dispose();
        GadgetCollections.ClearGadgets();
        Current = null;
#pragma warning restore CS8625
    }
}