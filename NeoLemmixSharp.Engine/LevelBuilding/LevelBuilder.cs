using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;
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

        var terrainBuilder = new TerrainBuilder(_graphicsDevice, levelData);
        terrainBuilder.BuildTerrain();

        var horizontalBoundaryBehaviour = levelData.HorizontalBoundaryBehaviour.GetHorizontalBoundaryBehaviour(levelData.LevelDimensions.W);
        var verticalBoundaryBehaviour = levelData.VerticalBoundaryBehaviour.GetVerticalBoundaryBehaviour(levelData.LevelDimensions.H);

        var levelLemmings = new LemmingBuilder(levelData).BuildLevelLemmings();

        var hatchGroups = HatchGroupBuilder.BuildHatchGroups(levelData);

        var lemmingManager = new LemmingManager(
            levelLemmings,
            levelData.PrePlacedLemmingData.Count,
            hatchGroups,
            levelData.MaxNumberOfClonedLemmings,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        var lemmingSpriteBank = LemmingSpriteBankBuilder.BuildLemmingSpriteBank(levelData);
        var tribeManager = BuildTribeManager(levelData, lemmingSpriteBank);

        var gadgetBuilder = new GadgetBuilder(levelData);
        gadgetBuilder.BuildLevelGadgets(lemmingManager, tribeManager);

        var levelGadgets = gadgetBuilder.GetGadgets();
        var gadgetTriggers = gadgetBuilder.GetGadgetTriggers();
        var gadgetBehaviours = gadgetBuilder.GetGadgetBehaviours();

        foreach (var hatchGroup in hatchGroups)
        {
            HatchGroupBuilder.SetHatchesForHatchGroup(hatchGroup, levelGadgets);
        }

        var inputController = new LevelInputController();

        var selectedTalismanId = -1;

        var levelObjectiveBuilder = new LevelObjectiveBuilder(levelData.LevelObjective, selectedTalismanId);
        var levelObjectiveManager = levelObjectiveBuilder.BuildLevelObjectiveManager();
        var skillSetManager = levelObjectiveBuilder.BuildSkillSetManager(tribeManager);
        var levelTimer = levelObjectiveBuilder.BuildLevelTimer();

        var controlPanel = new LevelControlPanel(levelData.ControlParameters, inputController, lemmingManager, skillSetManager);
        // Need to call this here instead of initialising in LevelScreen
        controlPanel.SetWindowDimensions(IGameWindow.Instance.WindowSize);

        var gadgetManager = new GadgetManager(levelGadgets, gadgetTriggers, gadgetBehaviours, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        SetUpGadgetConnections(levelData, gadgetManager);

        var terrainTexture = terrainBuilder.GetTerrainTexture();
        terrainBuilder.GetPixelData(out var pixelData);
        terrainBuilder.GetTerrainColors(out var terrainColorData);
        var terrainRenderer = new TerrainRenderer(terrainTexture);

        var rewindManager = new RewindManager(lemmingManager, gadgetManager, skillSetManager);
        var updateScheduler = new UpdateScheduler();
        var terrainManager = new TerrainManager(pixelData);

        var viewportObjectRendererBuilder = new ViewportObjectRendererBuilder(levelGadgets, levelLemmings);
        var controlPanelSpriteBank = viewportObjectRendererBuilder.GetControlPanelSpriteBank(_contentManager);

        var levelCursor = new LevelCursor();
        var levelCursorSprite = BuildLevelCursorSprite(levelCursor);
        var backgroundRenderer = BuildBackgroundRenderer(levelData);

        viewportObjectRendererBuilder.GetLevelSprites(
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
            controlPanelSpriteBank);

        var result = new LevelScreen(
            levelData,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour,
            levelData.LevelParameters,
            terrainManager,
            new TerrainUpdater(terrainTexture, in pixelData, in terrainColorData),
            lemmingManager,
            gadgetManager,
            tribeManager,
            skillSetManager,
            levelObjectiveManager,
            controlPanel,
            updateScheduler,
            levelCursor,
            levelTimer,
            inputController,
            new Level.Viewport(),
            rewindManager,
            lemmingSpriteBank,
            levelScreenRenderer);

        return result;
    }

    private static TribeManager BuildTribeManager(LevelData levelData, LemmingSpriteBank lemmingSpriteBank)
    {
        var numberOfTribes = levelData.TribeIdentifiers.Count;

        var tribes = new Tribe[numberOfTribes];
        for (var i = 0; i < tribes.Length; i++)
        {
            var tribeIdentifier = levelData.TribeIdentifiers[i];
            tribes[i] = new Tribe(i, lemmingSpriteBank, tribeIdentifier);
        }

        return new TribeManager(tribes);
    }

    private static IBackgroundRenderer BuildBackgroundRenderer(LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
        {
            var levelTheme = StyleCache.GetThemeData(new StyleFormatPair(levelData.LevelStyle, levelData.FileFormatType));
            return new SolidColorBackgroundRenderer(levelTheme.Background);
        }

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

    private static LevelCursorSprite BuildLevelCursorSprite(LevelCursor levelCursor)
    {
        return new LevelCursorSprite(
            levelCursor,
            CommonSprites.CursorCrossHair,
            CommonSprites.CursorHandHiRes);
    }

    private static void SetUpGadgetConnections(LevelData levelData, GadgetManager gadgetManager)
    {
        /* foreach (var gadgetLinkDatum in levelData.AllGadgetLinkData)
         {
             var sourceGadget = gadgetManager.AllItems[gadgetLinkDatum.SourceGadgetIdentifier.GadgetId];
             var targetGadget = gadgetManager.AllItems[gadgetLinkDatum.TargetGadgetIdentifier.GadgetId];

             if (targetGadget is not IFunctionalGadget targetFunctionalGadget)
                 throw new InvalidOperationException("Target gadget is not a functional gadget! Cannot set up gadget link!");

             var sourceGadgetOutput = sourceGadget.GetGadgetOutputForState(gadgetLinkDatum.SourceGadgetStateId);

             if (!targetFunctionalGadget.TryGetInputWithName(gadgetLinkDatum.TargetGadgetInputName, out var targetGadgetInput))
                 throw new InvalidOperationException("Could not locate target gadget input!");

             sourceGadgetOutput.RegisterInput(targetGadgetInput);
         }*/
    }

    int IComparer<IViewportObjectRenderer>.Compare(IViewportObjectRenderer? x, IViewportObjectRenderer? y)
    {
        var idX = -1;
        if (x is not null)
            idX = x.ItemId;

        var idY = -1;
        if (y is not null)
            idY = y.ItemId;

        var gt = (idX > idY) ? 1 : 0;
        var lt = (idX < idY) ? 1 : 0;
        return gt - lt;
    }
}
