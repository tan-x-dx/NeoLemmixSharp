using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IComparer<IViewportObjectRenderer>
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    public LevelBuilder(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
    }

    public LevelScreen BuildLevel(LevelData levelData)
    {
        StyleCache.EnsureStylesAreLoadedForLevel(levelData);
        using var levelObjectAssembler = new LevelObjectAssembler(_graphicsDevice, levelData);

        var terrainBuilder = new TerrainBuilder(_graphicsDevice, levelData);
        terrainBuilder.BuildTerrain();

        var lemmingSpriteBank = levelObjectAssembler.GetLemmingSpriteBank();

        var levelParameters = levelData.LevelParameters;
        var controlPanelParameters = levelData.ControlParameters;

        var teamManager = GetTeamManager(
            levelData,
            lemmingSpriteBank);

        levelData.HorizontalBoundaryBehaviour = BoundaryBehaviourType.Wrap;
        levelData.VerticalBoundaryBehaviour = BoundaryBehaviourType.Wrap;

        var levelDimensions = levelData.LevelDimensions;
        var horizontalBoundaryBehaviour = levelData.HorizontalBoundaryBehaviour.GetHorizontalBoundaryBehaviour(levelDimensions.W);
        var verticalBoundaryBehaviour = levelData.VerticalBoundaryBehaviour.GetVerticalBoundaryBehaviour(levelDimensions.H);

        var levelLemmings = levelObjectAssembler.GetLevelLemmings();
        var hatchGroups = levelObjectAssembler.GetHatchGroups();
        var lemmingManager = new LemmingManager(
            hatchGroups,
            levelLemmings,
            levelData.HatchLemmingData.Count,
            levelData.PrePlacedLemmingData.Count,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        var levelGadgets = levelObjectAssembler.GetLevelGadgets(lemmingManager, teamManager);

        foreach (var hatchGroup in hatchGroups)
        {
            levelObjectAssembler.SetHatchesForHatchGroup(hatchGroup);
        }

        var inputController = new LevelInputController();

        var levelObjectiveManager = new LevelObjectiveManager([]/*levelData.LevelObjectives*/, 0);

        var skillSetManager = new SkillSetManager(levelObjectiveManager.PrimaryLevelObjective);
        var levelCursor = new LevelCursor();
        var levelTimer = CreateLevelTimer(levelObjectiveManager);

        var controlPanel = new LevelControlPanel(controlPanelParameters, inputController, lemmingManager, skillSetManager);
        // Need to call this here instead of initialising in LevelScreen
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowSize);

        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        var levelViewport = new Level.Viewport();

        var terrainTexture = terrainBuilder.GetTerrainTexture();
        var pixelData = terrainBuilder.GetPixelData();
        var terrainColorData = terrainBuilder.GetTerrainColors();
        var terrainPainter = new TerrainPainter(terrainTexture, pixelData, terrainColorData);
        var terrainRenderer = new TerrainRenderer(terrainTexture);

        var rewindManager = new RewindManager(lemmingManager, gadgetManager, skillSetManager);

        var updateScheduler = new UpdateScheduler();

        var terrainManager = new TerrainManager(pixelData);

        var gadgetSpriteBank = levelObjectAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = levelObjectAssembler.GetControlPanelSpriteBank(_contentManager);

        var levelCursorSprite = GetLevelCursorSprite(levelCursor);
        var backgroundRenderer = GetBackgroundRenderer(levelData);

        levelObjectAssembler.GetLevelSprites(
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
            teamManager,
            skillSetManager,
            levelObjectiveManager,
            controlPanel,
            updateScheduler,
            levelCursor,
            levelTimer,
            inputController,
            levelViewport,
            rewindManager,
            levelScreenRenderer);

        GC.Collect(2, GCCollectionMode.Forced);

        return result;
    }

    private static TeamManager GetTeamManager(LevelData levelData, LemmingSpriteBank lemmingSpriteBank)
    {
        var numberOfTeams = levelData.NumberOfTeams;

        var teams = new Team[numberOfTeams];
        for (var i = 0; i < teams.Length; i++)
        {
            teams[i] = new Team(i, lemmingSpriteBank);
        }

        return new TeamManager(teams);
    }

    private static LevelTimer CreateLevelTimer(LevelObjectiveManager levelObjectiveManager)
    {
        var primaryObjective = levelObjectiveManager.PrimaryLevelObjective;
        foreach (var requirement in primaryObjective.Requirements)
        {
            if (requirement is TimeRequirement timeRequirement)
                return LevelTimer.CreateCountDownTimer(timeRequirement.TimeLimitInSeconds);
        }

        return LevelTimer.CreateCountUpTimer();
    }

    private static IBackgroundRenderer GetBackgroundRenderer(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
            return new SolidColorBackgroundRenderer(EngineConstants.ClassicLevelBackgroundColor);

        if (backgroundData.IsSolidColor)
            return new SolidColorBackgroundRenderer(backgroundData.Color);

        return new SolidColorBackgroundRenderer(new Color((byte)24, (byte)24, (byte)60, (byte)0xff));
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

        for (var i = 0; i < resultSpan.Length; i++)
        {
            resultSpan[i].RendererId = i;
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
}