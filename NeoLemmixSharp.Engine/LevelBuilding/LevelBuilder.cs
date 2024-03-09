using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.Rendering;
using Viewport = NeoLemmixSharp.Engine.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ILevelReader _levelReader;
    private readonly TerrainPainter _terrainPainter;
    private readonly LevelObjectAssembler _levelObjectAssembler;

    public LevelBuilder(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        ILevelReader levelReader)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _levelReader = levelReader;
        _terrainPainter = new TerrainPainter(graphicsDevice);
        _levelObjectAssembler = new LevelObjectAssembler(graphicsDevice);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        var levelData = _levelReader.ReadLevel(levelFilePath, _graphicsDevice);
        levelData.Validate();

        _terrainPainter.PaintLevel(levelData);

        var lemmingSpriteBank = _levelObjectAssembler.GetLemmingSpriteBank();

        var levelParameters = levelData.LevelParameters;
        var controlPanelParameters = levelData.ControlParameters;
        LevelScreen.SetLevelParameters(levelParameters);

        var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
        var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

        var levelLemmings = _levelObjectAssembler.GetLevelLemmings(levelData);
        var hatchGroups = LevelObjectAssembler.GetHatchGroups(levelData);
        var lemmingManager = new LemmingManager(levelData, hatchGroups, levelLemmings, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetLemmingManager(lemmingManager);
        var levelGadgets = _levelObjectAssembler.GetLevelGadgets(levelData, lemmingManager);

        foreach (var hatchGroup in hatchGroups)
        {
            _levelObjectAssembler.SetHatchesForHatchGroup(hatchGroup);
        }

        var inputController = new LevelInputController(levelParameters);
        var skillSetManager = new SkillSetManager(levelData.SkillSetData);
        LevelScreen.SetSkillSetManager(skillSetManager);

        var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, inputController);
        LevelScreen.SetLevelCursor(levelCursor);

        LevelTimer levelTimer = levelData.TimeLimit.HasValue
            ? new CountDownLevelTimer(levelData.TimeLimit.Value)
            : new CountUpLevelTimer();
        var controlPanel = new LevelControlPanel(controlPanelParameters, inputController, skillSetManager, lemmingManager, levelTimer);
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowWidth, IGameWindow.Instance.WindowHeight);
        LevelScreen.SetLevelControlPanel(controlPanel);

        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetGadgetManager(gadgetManager);

        var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
        var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);
        var levelViewport = new Viewport(horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        var updateScheduler = new UpdateScheduler(controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager, gadgetManager, skillSetManager);
        LevelScreen.SetUpdateScheduler(updateScheduler);

        var terrainTexture = _terrainPainter.GetTerrainTexture();
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
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(_contentManager);

        var levelSprites = _levelObjectAssembler.GetLevelSprites();
        var levelCursorSprite = CommonSpriteBank.Instance.GetLevelCursorSprite(levelCursor);

        var levelScreenRenderer = new LevelScreenRenderer(
            _graphicsDevice,
            levelData,
            controlPanel,
            levelViewport,
            levelSprites,
            terrainRenderer,
            levelCursorSprite,
            lemmingSpriteBank,
            gadgetSpriteBank,
            controlPanelSpriteBank);

        lemmingManager.Initialise();
        gadgetManager.Initialise();
        updateScheduler.Initialise();

        return new LevelScreen(
            levelData,
            updateScheduler,
            inputController,
            controlPanel,
            levelViewport,
            levelScreenRenderer);
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _terrainPainter.Dispose();
    }
}