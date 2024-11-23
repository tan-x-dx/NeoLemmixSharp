using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable, IComparer<IViewportObjectRenderer>
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

        levelData.HorizontalBoundaryBehaviour = BoundaryBehaviourType.Wrap;
        levelData.VerticalBoundaryBehaviour = BoundaryBehaviourType.Wrap;

        var horizontalBoundaryBehaviour =
            levelData.HorizontalBoundaryBehaviour.GetHorizontalBoundaryBehaviour(levelData.LevelWidth);
        var verticalBoundaryBehaviour =
            levelData.VerticalBoundaryBehaviour.GetVerticalBoundaryBehaviour(levelData.LevelHeight);

        var levelLemmings = _levelObjectAssembler.GetLevelLemmings(levelData);
        var hatchGroups = LevelObjectAssembler.GetHatchGroups(levelData);
        var lemmingManager = new LemmingManager(
            hatchGroups,
            levelLemmings,
            levelData.HatchLemmingData.Count,
            levelData.PrePlacedLemmingData.Count,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        var levelGadgets = _levelObjectAssembler.GetLevelGadgets(levelData, lemmingManager);

        foreach (var hatchGroup in hatchGroups)
        {
            _levelObjectAssembler.SetHatchesForHatchGroup(hatchGroup);
        }

        var inputController = new LevelInputController();

        var primaryLevelObjective = levelData.LevelObjectives.Find(lo => lo.LevelObjectiveId == 0)!;

        var skillSetManager = new SkillSetManager(primaryLevelObjective);

        var levelCursor = new LevelCursor();

        var levelTimer = GetLevelTimer(levelData);

        var controlPanel = new LevelControlPanel(controlPanelParameters, inputController, lemmingManager, skillSetManager);
        // Need to call this here instead of initialising in LevelScreen
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowWidth, IGameWindow.Instance.WindowHeight);

        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        var levelViewport = new Level.Viewport();

        var terrainTexture = terrainBuilder.GetTerrainTexture();
        var pixelData = terrainBuilder.GetPixelData();
        var terrainColorData = terrainBuilder.GetTerrainColors();
        var terrainPainter = new TerrainPainter(terrainTexture, pixelData, terrainColorData, levelData.LevelWidth);
        var terrainRenderer = new TerrainRenderer(terrainTexture);

        var rewindManager = new RewindManager(lemmingManager, gadgetManager, skillSetManager);

        var updateScheduler = new UpdateScheduler();

        var terrainManager = new TerrainManager(pixelData);

        var gadgetSpriteBank = _levelObjectAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(_contentManager);

        var levelCursorSprite = GetLevelCursorSprite(levelCursor);
        var backgroundRenderer = GetBackgroundRenderer(levelData);

        _levelObjectAssembler.GetLevelSprites(
            out var behindTerrainSprites,
            out var inFrontOfTerrainSprites,
            out var lemmingSprites);
        var orderedLevelSprites = GetSortedRenderables(
            behindTerrainSprites,
            inFrontOfTerrainSprites,
            terrainRenderer,
            lemmingSprites);

        var levelRenderer = new LevelRenderer(
            _graphicsDevice,
            orderedLevelSprites,
            backgroundRenderer,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        var levelScreenRenderer = new LevelScreenRenderer(
            _graphicsDevice,
            controlPanel,
            levelRenderer,
            levelCursorSprite,
            lemmingSpriteBank,
            gadgetSpriteBank,
            controlPanelSpriteBank);

        var result = new LevelScreen(
            levelData,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour,
            levelParameters,
            terrainManager,
            terrainPainter,
            lemmingManager,
            gadgetManager,
            skillSetManager,
            controlPanel,
            updateScheduler,
            levelCursor,
            levelTimer,
            inputController,
            levelViewport,
            rewindManager,
            levelScreenRenderer);

        GC.Collect(3, GCCollectionMode.Forced);

        return result;
    }

    private static LevelTimer GetLevelTimer(LevelData levelData)
    {
        var primaryObjective = levelData.LevelObjectives.Find(lo => lo.LevelObjectiveId == 0)!;
        foreach (var requirement in primaryObjective.Requirements)
        {
            if (requirement is TimeRequirement timeRequirement)
                return new LevelTimer(timeRequirement.TimeLimitInSeconds);
        }

        return new LevelTimer();
    }

    private static IBackgroundRenderer GetBackgroundRenderer(
        LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
            return new SolidColorBackgroundRenderer(EngineConstants.ClassicLevelBackgroundColor);

        if (backgroundData.IsSolidColor)
            return new SolidColorBackgroundRenderer(backgroundData.Color);

        return new SolidColorBackgroundRenderer(new Color(24, 24, 60));
    }

    private List<IViewportObjectRenderer> GetSortedRenderables(
        List<IViewportObjectRenderer> behindTerrainSprites,
        List<IViewportObjectRenderer> inFrontOfTerrainSprites,
        TerrainRenderer terrainRenderer,
        List<IViewportObjectRenderer> lemmingSprites)
    {
        var listCapacity = behindTerrainSprites.Count +
                           1 + // Terrain renderer
                           inFrontOfTerrainSprites.Count +
                           lemmingSprites.Count;
        var result = new List<IViewportObjectRenderer>(listCapacity);

        behindTerrainSprites.Sort(this);
        inFrontOfTerrainSprites.Sort(this);
        lemmingSprites.Sort(this);

        result.AddRange(behindTerrainSprites);
        result.Add(terrainRenderer);
        result.AddRange(inFrontOfTerrainSprites);
        result.AddRange(lemmingSprites);

        var resultSpan = CollectionsMarshal.AsSpan(result);

        var i = 0;
        foreach (var renderer in resultSpan)
        {
            renderer.RendererId = i++;
        }

        return result;
    }

    private static LevelCursorSprite GetLevelCursorSprite(LevelCursor levelCursor)
    {
        return new LevelCursorSprite(
            levelCursor,
            CommonSprites.CursorCrossHair,
            CommonSprites.CursorHandHiRes);
    }

    int IComparer<IViewportObjectRenderer>.Compare(IViewportObjectRenderer? x, IViewportObjectRenderer? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.ItemId.CompareTo(y.ItemId);
    }

    public void Dispose()
    {
        _levelObjectAssembler.Dispose();
    }
}