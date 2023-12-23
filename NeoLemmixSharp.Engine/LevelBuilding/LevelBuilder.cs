﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport.Background;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using Viewport = NeoLemmixSharp.Engine.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
	private readonly ContentManager _content;
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
		lemmingSpriteBank.SetTeamColors();

		var lemmingSpriteBankLookup = new Dictionary<Team, LemmingSpriteBank>();
		foreach (var team in Team.AllItems)
		{
			lemmingSpriteBankLookup[team] = lemmingSpriteBank;
		}

		var controlPanelButtonAvailability = GetControlPanelButtonAvailability(levelData);

		var inputController = new LevelInputController();
		var skillSetManager = new SkillSetManager(levelData.SkillSetData);
		LevelScreen.SetSkillSetManager(skillSetManager);

		LevelTimer levelTimer = levelData.TimeLimit.HasValue
			? new CountDownLevelTimer(levelData.TimeLimit.Value)
			: new CountUpLevelTimer();
		var controlPanel = new LevelControlPanel(levelData, controlPanelButtonAvailability, inputController, skillSetManager, levelTimer);

		foreach (var skillAssignButton in controlPanel.SkillAssignButtons)
		{
			var skillTrackingData = skillSetManager.GetSkillTrackingData(skillAssignButton.SkillTrackingDataId);
			if (skillTrackingData is not null)
			{
				skillAssignButton.UpdateSkillCount(skillTrackingData.SkillCount);
			}
		}

		var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
		var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

		var hatchGroups = _levelObjectAssembler.GetHatchGroups();
		var levelLemmings = _levelObjectAssembler.GetLevelLemmings();
		var lemmingManager = new LemmingManager(levelData, hatchGroups, levelLemmings, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
		LevelScreen.SetLemmingManager(lemmingManager);

		_levelObjectAssembler.AssembleLevelObjects(
			_content,
			_levelReader.LevelData);

		var levelGadgets = _levelObjectAssembler.GetLevelGadgets();
		var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
		LevelScreen.SetGadgetManager(gadgetManager);

		var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, controlPanel, inputController, lemmingManager, skillSetManager);

		var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
		var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);
		var levelViewport = new Viewport(levelCursor, horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

		var updateScheduler = new UpdateScheduler(controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager, gadgetManager, skillSetManager);

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

		var levelSprites = _levelObjectAssembler.GetLevelSprites(lemmingSpriteBankLookup);

		var controlPanelRenderer = new ClassicControlPanelRenderer(controlPanelSpriteBank, controlPanel);

		var levelCursorSprite = controlPanelSpriteBank.GetLevelCursorSprite(levelCursor);

		var backgroundRenderer = new SolidColorBackgroundRenderer(controlPanelSpriteBank, levelViewport, new Color(24, 24, 60));

		var levelRenderer = new LevelRenderer(
			levelData.LevelWidth,
			levelData.LevelHeight,
			levelViewport,
			backgroundRenderer,
			terrainRenderer,
			levelSprites,
			levelCursorSprite,
			controlPanelRenderer,
			controlPanelSpriteBank,
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

	private ControlPanelButtonAvailability GetControlPanelButtonAvailability(LevelData levelData)
	{
		return ControlPanelButtonAvailability.ShowPauseButton |
		       ControlPanelButtonAvailability.ShowNukeButton |
		       ControlPanelButtonAvailability.ShowFastForwardsButton |
		       ControlPanelButtonAvailability.ShowRestartButton |
		       ControlPanelButtonAvailability.ShowFrameNudgeButtons |
		       ControlPanelButtonAvailability.ShowDirectionSelectButtons |
		       ControlPanelButtonAvailability.ShowClearPhysicsAndReplayButton |
		       ControlPanelButtonAvailability.ShowReleaseRateButtonsIfPossible;
	}

	public void Dispose()
	{
		_levelReader.Dispose();
		_terrainPainter.Dispose();
	}
}