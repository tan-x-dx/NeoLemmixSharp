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
	public static TerrainManager TerrainManager { get; private set; } = null!;
	public static LemmingManager LemmingManager { get; private set; } = null!;
	public static GadgetManager GadgetManager { get; private set; } = null!;
	public static SkillSetManager SkillSetManager { get; private set; } = null!;

	public static void SetTerrainManager(TerrainManager terrainManager)
	{
		TerrainManager = terrainManager;
	}

	public static void SetLemmingManager(LemmingManager lemmingManager)
	{
		LemmingManager = lemmingManager;
	}

	public static void SetGadgetManager(GadgetManager gadgetManager)
	{
		GadgetManager = gadgetManager;
	}

	public static void SetSkillSetManager(SkillSetManager skillSetManager)
	{
		SkillSetManager = skillSetManager;
	}

	private readonly UpdateScheduler _updateScheduler;
	private readonly LevelInputController _inputController;
	private readonly ILevelControlPanel _controlPanel;
	private readonly Viewport _viewport;
	private readonly LevelRenderer _levelRenderer;

	IScreenRenderer IBaseScreen.ScreenRenderer => _levelRenderer;
	public string ScreenTitle { get; }
	public bool IsDisposed { get; private set; }

	public LevelScreen(
		LevelData levelData,
		UpdateScheduler updateScheduler,
		LevelInputController levelInputController,
		ILevelControlPanel controlPanel,
		Viewport viewport,
		LevelRenderer levelRenderer)
	{
		ScreenTitle = levelData.LevelTitle;

		_updateScheduler = updateScheduler;
		_inputController = levelInputController;
		_controlPanel = controlPanel;
		_viewport = viewport;
		_levelRenderer = levelRenderer;
	}

	public void Tick(GameTime gameTime)
	{
		if (!IGameWindow.Instance.IsActive)
			return;

		_updateScheduler.Tick();

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
		if (_inputController.Quit.IsPressed)
		{
			IGameWindow.Instance.Escape();
		}

		if (_inputController.ToggleFullScreen.IsPressed)
		{
			IGameWindow.Instance.ToggleBorderless();
		}
	}

	public void OnWindowSizeChanged()
	{
		var windowWidth = IGameWindow.Instance.WindowWidth;
		var windowHeight = IGameWindow.Instance.WindowHeight;

		_controlPanel.SetWindowDimensions(windowWidth, windowHeight);
		_viewport.SetWindowDimensions(windowWidth, windowHeight, ((LevelControlPanel)_controlPanel).ControlPanelScreenHeight);
		_levelRenderer.OnWindowSizeChanged();
	}

	public void Dispose()
	{
#pragma warning disable CS8625
		SetTerrainManager(null);
		SetLemmingManager(null);
		SetGadgetManager(null);
		SetSkillSetManager(null);

		_levelRenderer.Dispose();
		IsDisposed = true;
#pragma warning restore CS8625
	}
}