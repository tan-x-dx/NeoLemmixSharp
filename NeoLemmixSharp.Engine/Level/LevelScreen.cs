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
	public static LevelParameters LevelParameters { get; private set; }
	public static TerrainManager TerrainManager { get; private set; } = null!;
	public static LemmingManager LemmingManager { get; private set; } = null!;
	public static GadgetManager GadgetManager { get; private set; } = null!;
	public static SkillSetManager SkillSetManager { get; private set; } = null!;
	public static ILevelControlPanel LevelControlPanel { get; private set; } = null!;
	public static UpdateScheduler UpdateScheduler { get; private set; } = null!;
	public static LevelCursor LevelCursor { get; private set; } = null!;

	public static void SetLevelParameters(LevelParameters levelParameters)
	{
		LevelParameters = levelParameters;
	}

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

	public static void SetLevelControlPanel(ILevelControlPanel levelControlPanel)
	{
		LevelControlPanel = levelControlPanel;
	}

	public static void SetUpdateScheduler(UpdateScheduler updateScheduler)
	{
		UpdateScheduler = updateScheduler;
	}

	public static void SetLevelCursor(LevelCursor levelCursor)
	{
		LevelCursor = levelCursor;
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

		// Do these checks after main game loop
		HandleKeyboardInput();
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
		if (IsDisposed)
			return;

		LemmingManager.Dispose();
		GadgetManager.Dispose();
		SkillSetManager.Dispose();

#pragma warning disable CS8625
		SetTerrainManager(null);
		SetLemmingManager(null);
		SetGadgetManager(null);
		SetSkillSetManager(null);
		SetLevelControlPanel(null);
		SetUpdateScheduler(null);
		SetLevelCursor(null);

		_levelRenderer.Dispose();
		IsDisposed = true;
#pragma warning restore CS8625
	}
}