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
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using Viewport = NeoLemmixSharp.Engine.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly LevelObjectAssembler _levelObjectAssembler;

    public LevelBuilder(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _levelObjectAssembler = new LevelObjectAssembler(graphicsDevice);
    }

    public LevelScreen BuildLevel(LevelData levelData)
    {
        var terrainBuilder = new TerrainBuilder(_graphicsDevice, levelData);
        terrainBuilder.BuildTerrain();

        var lemmingSpriteBank = _levelObjectAssembler.GetLemmingSpriteBank();

        var levelParameters = levelData.LevelParameters;
        var controlPanelParameters = levelData.ControlParameters;
        LevelScreen.SetLevelParameters(levelParameters);

        levelData.HorizontalBoundaryBehaviour = BoundaryBehaviourType.Wrap;
        levelData.VerticalBoundaryBehaviour = BoundaryBehaviourType.Wrap;

        var horizontalBoundaryBehaviour = levelData.HorizontalBoundaryBehaviour.GetHorizontalBoundaryBehaviour(levelData.LevelWidth);
        var verticalBoundaryBehaviour = levelData.VerticalBoundaryBehaviour.GetVerticalBoundaryBehaviour(levelData.LevelHeight);

        var levelLemmings = _levelObjectAssembler.GetLevelLemmings(levelData);
        var hatchGroups = LevelObjectAssembler.GetHatchGroups(levelData);
        var lemmingManager = new LemmingManager(hatchGroups, levelLemmings, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetLemmingManager(lemmingManager);
        var levelGadgets = _levelObjectAssembler.GetLevelGadgets(levelData, lemmingManager);

        foreach (var hatchGroup in hatchGroups)
        {
            _levelObjectAssembler.SetHatchesForHatchGroup(hatchGroup);
        }

        var inputController = new LevelInputController(levelParameters);
        LevelScreen.SetLevelInputController(inputController);

        var primaryLevelObjective = levelData.PrimaryLevelObjective!;

        var skillSetManager = new SkillSetManager(primaryLevelObjective);
        LevelScreen.SetSkillSetManager(skillSetManager);

        var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, inputController);
        LevelScreen.SetLevelCursor(levelCursor);

        var levelTimer = LevelBuildingHelpers.GetLevelTimer(levelData);
        var controlPanel = new LevelControlPanel(controlPanelParameters, inputController, skillSetManager, lemmingManager, levelTimer);
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowWidth, IGameWindow.Instance.WindowHeight);
        LevelScreen.SetLevelControlPanel(controlPanel);

        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetGadgetManager(gadgetManager);

        var levelViewport = new Viewport(horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetViewport(levelViewport);

        var terrainTexture = terrainBuilder.GetTerrainTexture();
        var pixelData = terrainBuilder.GetPixelData();
        var terrainColorData = terrainBuilder.GetTerrainColors();
        var terrainPainter = new TerrainPainter(terrainTexture, pixelData, terrainColorData, levelData.LevelWidth);
        var terrainRenderer = new TerrainRenderer(terrainTexture);
        LevelScreen.SetTerrainPainter(terrainPainter);

        var updateScheduler = new UpdateScheduler(
            controlPanel,
            levelViewport,
            levelCursor,
            inputController,
            levelTimer,
            lemmingManager,
            gadgetManager,
            skillSetManager,
            terrainPainter);
        LevelScreen.SetUpdateScheduler(updateScheduler);

        var terrainManager = new TerrainManager(
            pixelData,
            terrainPainter,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        LevelScreen.SetTerrainManager(terrainManager);

        var gadgetSpriteBank = _levelObjectAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(_contentManager);

        var levelCursorSprite = CommonSprites.GetLevelCursorSprite(levelCursor);
        var backgroundRenderer = LevelBuildingHelpers.GetBackgroundRenderer(levelData, levelViewport);

        _levelObjectAssembler.GetLevelSprites(out var behindTerrainSprites, out var inFrontOfTerrainSprites, out var lemmingSprites);
        var orderedLevelSprites = LevelBuildingHelpers.GetSortedRenderables(
            behindTerrainSprites,
            inFrontOfTerrainSprites,
            terrainRenderer,
            lemmingSprites);

        var levelRenderer = new LevelRenderer(
            _graphicsDevice,
            levelViewport,
            orderedLevelSprites,
            backgroundRenderer);

        var levelScreenRenderer = new LevelScreenRenderer(
            _graphicsDevice,
            controlPanel,
            levelViewport,
            levelRenderer,
            levelCursorSprite,
            lemmingSpriteBank,
            gadgetSpriteBank,
            controlPanelSpriteBank);

        LevelScreen.SetLevelScreenRenderer(levelScreenRenderer);

        lemmingManager.Initialise();
        gadgetManager.Initialise();
        updateScheduler.Initialise();

        return new LevelScreen(levelData);
    }

    public void Dispose()
    {
        _levelObjectAssembler.Dispose();
    }
}