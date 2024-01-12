﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using Viewport = NeoLemmixSharp.Engine.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
	private readonly ContentManager _content;
	private readonly GraphicsDevice _graphicsDevice;
	private readonly ILevelReader _levelReader;
	private readonly TerrainPainter _terrainPainter;
	private readonly LevelObjectAssembler _levelObjectAssembler;

	public LevelBuilder(
		ContentManager content,
		GraphicsDevice graphicsDevice,
		SpriteBatch spriteBatch,
		ILevelReader levelReader)
	{
		_content = content;
		_graphicsDevice = graphicsDevice;
		_levelReader = levelReader;
		_terrainPainter = new TerrainPainter(graphicsDevice);
		_levelObjectAssembler = new LevelObjectAssembler(graphicsDevice, content, spriteBatch);
	}

	public LevelScreen BuildLevel(string levelFilePath)
	{
		_levelReader.ReadLevel(levelFilePath);

		_terrainPainter.PaintLevel(_levelReader.LevelData);

		var terrainTexture = _terrainPainter.GetTerrainTexture();

		var levelData = _levelReader.LevelData;
		var lemmingSpriteBank = _levelObjectAssembler.GetLemmingSpriteBank();

		var levelParameters = GetLevelParameters(levelData);
		var controlPanelParameters = GetControlPanelParameters(levelData);
		LevelScreen.SetLevelParameters(levelParameters);

		var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
		var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

		var hatchGroups = _levelObjectAssembler.GetHatchGroups();
		var levelLemmings = _levelObjectAssembler.GetLevelLemmings();
		var lemmingManager = new LemmingManager(levelData, hatchGroups, levelLemmings, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
		LevelScreen.SetLemmingManager(lemmingManager);

		var inputController = new LevelInputController(levelParameters);
		var skillSetManager = new SkillSetManager(levelData.SkillSetData);
		LevelScreen.SetSkillSetManager(skillSetManager);

		var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, inputController);
		LevelScreen.SetLevelCursor(levelCursor);

		LevelTimer levelTimer = levelData.TimeLimit.HasValue
			? new CountDownLevelTimer(levelData.TimeLimit.Value)
			: new CountUpLevelTimer();
		var controlPanel = new LevelControlPanel(controlPanelParameters, inputController, skillSetManager, lemmingManager, levelTimer);
		LevelScreen.SetLevelControlPanel(controlPanel);

		_levelObjectAssembler.AssembleLevelObjects(
			_content,
			_levelReader.LevelData);

		var levelGadgets = _levelObjectAssembler.GetLevelGadgets();
		var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
		LevelScreen.SetGadgetManager(gadgetManager);

		var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
		var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);
		var levelViewport = new Viewport(horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

		var updateScheduler = new UpdateScheduler(controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager, gadgetManager, skillSetManager);
		LevelScreen.SetUpdateScheduler(updateScheduler);

		var terrainRenderer = new TerrainRenderer(terrainTexture, levelViewport);

		var pixelData = _terrainPainter.GetPixelData();

		var terrainManager = new TerrainManager(
			pixelData,
			gadgetManager,
			terrainRenderer,
			horizontalBoundaryBehaviour,
			verticalBoundaryBehaviour);
		LevelScreen.SetTerrainManager(terrainManager);

		var gadgetSpriteBank = _levelObjectAssembler.GetGadgetSpriteBank();
		var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank();
		LevelRenderer.SetControlPanelSpriteBank(controlPanelSpriteBank);

		var levelSprites = _levelObjectAssembler.GetLevelSprites();

		var controlPanelRenderer = new ClassicControlPanelRenderer(controlPanelSpriteBank, controlPanel);

		var levelCursorSprite = CommonSpriteBank.Instance.GetLevelCursorSprite(levelCursor);

		var backgroundRenderer = new SolidColorBackgroundRenderer(levelViewport, new Color(24, 24, 60));

		var levelRenderer = new LevelRenderer(
			levelData.LevelWidth,
			levelData.LevelHeight,
			levelViewport,
			backgroundRenderer,
			terrainRenderer,
			levelSprites,
			levelCursorSprite,
			controlPanelRenderer,
			lemmingSpriteBank,
			gadgetSpriteBank);

		var levelRenderer2 = new LevelRendererAaa(
			_graphicsDevice,
			levelData,
			levelViewport,
			levelSprites,
			levelCursorSprite,
			lemmingSpriteBank,
			gadgetSpriteBank);

		lemmingManager.Initialise();
		gadgetManager.Initialise();

		return new LevelScreen(
			levelData,
			updateScheduler,
			inputController,
			controlPanel,
			levelViewport,
			levelRenderer);
	}

	private static LevelParameters GetLevelParameters(LevelData levelData)
	{
		return LevelParameters.TimedBombers |
			   //    LevelParameters.EnablePause |
			   LevelParameters.EnableNuke |
			   LevelParameters.EnableFastForward |
			   LevelParameters.EnableDirectionSelect |
			   LevelParameters.EnableClearPhysics |
			   LevelParameters.EnableSkillShadows |
			   LevelParameters.EnableFrameControl;
	}

	private static ControlPanelParameters GetControlPanelParameters(LevelData levelData)
	{
		return ControlPanelParameters.ShowPauseButton |
			   ControlPanelParameters.ShowNukeButton |
			   ControlPanelParameters.ShowFastForwardsButton |
			   ControlPanelParameters.ShowRestartButton |
			   ControlPanelParameters.ShowFrameNudgeButtons |
			   ControlPanelParameters.ShowDirectionSelectButtons |
			   ControlPanelParameters.ShowClearPhysicsAndReplayButton |
			   ControlPanelParameters.ShowReleaseRateButtonsIfPossible;
	}

	public void Dispose()
	{
		_levelReader.Dispose();
		_terrainPainter.Dispose();
	}
}