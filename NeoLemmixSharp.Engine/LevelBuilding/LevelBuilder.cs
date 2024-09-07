using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;

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

        LevelScreen.SetHorizontalBoundaryBehaviour(horizontalBoundaryBehaviour);
        LevelScreen.SetVerticalBoundaryBehaviour(verticalBoundaryBehaviour);

        var levelLemmings = _levelObjectAssembler.GetLevelLemmings(levelData);
        var hatchGroups = LevelObjectAssembler.GetHatchGroups(levelData);
        var lemmingManager = new LemmingManager(
            hatchGroups,
            levelLemmings,
            levelData.HatchLemmingData.Count,
            levelData.PrePlacedLemmingData.Count,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        LevelScreen.SetLemmingManager(lemmingManager);
        var levelGadgets = _levelObjectAssembler.GetLevelGadgets(levelData, lemmingManager);

        foreach (var hatchGroup in hatchGroups)
        {
            _levelObjectAssembler.SetHatchesForHatchGroup(hatchGroup);
        }

        var inputController = new LevelInputController();
        LevelScreen.SetLevelInputController(inputController);

        var primaryLevelObjective = levelData.LevelObjectives.Find(lo => lo.LevelObjectiveId == 0)!;

        var skillSetManager = new SkillSetManager(primaryLevelObjective);
        LevelScreen.SetSkillSetManager(skillSetManager);

        var levelCursor = new LevelCursor();
        LevelScreen.SetLevelCursor(levelCursor);

        var levelTimer = LevelBuildingHelpers.GetLevelTimer(levelData);
        LevelScreen.SetLevelTimer(levelTimer);

        var controlPanel = new LevelControlPanel(controlPanelParameters, inputController);
        // Need to call this here instead of initialising in LevelScreen
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowWidth, IGameWindow.Instance.WindowHeight);
        LevelScreen.SetLevelControlPanel(controlPanel);

        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        LevelScreen.SetGadgetManager(gadgetManager);

        var levelViewport = new Level.Viewport();
        LevelScreen.SetViewport(levelViewport);

        var terrainTexture = terrainBuilder.GetTerrainTexture();
        var pixelData = terrainBuilder.GetPixelData();
        var terrainColorData = terrainBuilder.GetTerrainColors();
        var terrainPainter = new TerrainPainter(terrainTexture, pixelData, terrainColorData, levelData.LevelWidth);
        var terrainRenderer = new TerrainRenderer(terrainTexture);
        LevelScreen.SetTerrainPainter(terrainPainter);

        var rewindManager = new RewindManager();
        LevelScreen.SetRewindManager(rewindManager);

        var updateScheduler = new UpdateScheduler();
        LevelScreen.SetUpdateScheduler(updateScheduler);

        var terrainManager = new TerrainManager(pixelData);
        LevelScreen.SetTerrainManager(terrainManager);

        var gadgetSpriteBank = _levelObjectAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(_contentManager);

        var levelCursorSprite = CommonSprites.GetLevelCursorSprite(levelCursor);
        var backgroundRenderer = LevelBuildingHelpers.GetBackgroundRenderer(levelData);

        _levelObjectAssembler.GetLevelSprites(out var behindTerrainSprites, out var inFrontOfTerrainSprites, out var lemmingSprites);
        var orderedLevelSprites = LevelBuildingHelpers.GetSortedRenderables(
            behindTerrainSprites,
            inFrontOfTerrainSprites,
            terrainRenderer,
            lemmingSprites);

        var levelRenderer = new LevelRenderer(
            _graphicsDevice,
            orderedLevelSprites,
            backgroundRenderer);

        var levelScreenRenderer = new LevelScreenRenderer(
            _graphicsDevice,
            controlPanel,
            levelRenderer,
            levelCursorSprite,
            lemmingSpriteBank,
            gadgetSpriteBank,
            controlPanelSpriteBank);

        LevelScreen.SetLevelScreenRenderer(levelScreenRenderer);

        return new LevelScreen(levelData);
    }

    public void Dispose()
    {
        _levelObjectAssembler.Dispose();
    }
}