using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport.Background;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using NeoLemmixSharp.Io.LevelReading;
using NeoLemmixSharp.Io.LevelReading.Nxlv;
using Viewport = NeoLemmixSharp.Engine.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _content;
    private readonly FontBank _fontBank;
    private readonly ILevelReader _levelReader;
    private readonly TerrainPainter _terrainPainter;
    private readonly LevelObjectAssembler _levelObjectAssembler;

    public LevelBuilder(
        ContentManager content,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank,
        RootDirectoryManager rootDirectoryManager)
    {
        _content = content;
        _fontBank = fontBank;
        _levelReader = new NxlvLevelReader(rootDirectoryManager);
        _terrainPainter = new TerrainPainter(graphicsDevice, rootDirectoryManager);
        _levelObjectAssembler = new LevelObjectAssembler(graphicsDevice, content, spriteBatch, rootDirectoryManager);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _terrainPainter.PaintLevel(_levelReader.LevelData);

        var terrainTexture = _terrainPainter.GetTerrainTexture();

        var levelData = _levelReader.LevelData;
        var lemmingSpriteBank = _levelObjectAssembler.GetLemmingSpriteBank();
        lemmingSpriteBank.SetTeamColors();

        var inputController = new LevelInputController();
        var skillSetManager = new SkillSetManager(levelData.SkillSetData);
        var controlPanel = new LevelControlPanel(skillSetManager, inputController);
        LevelTimer levelTimer = levelData.TimeLimit.HasValue
            ? new CountDownLevelTimer(levelData.TimeLimit.Value)
            : new CountUpLevelTimer();

        foreach (var skillAssignButton in controlPanel.SkillAssignButtons)
        {
            var skillTrackingData = skillSetManager.GetSkillTrackingData(skillAssignButton.SkillAssignButtonId)!;
            skillAssignButton.UpdateSkillCount(skillTrackingData.SkillCount);
        }

        var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
        var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

        var levelLemmings = _levelObjectAssembler.GetLevelLemmings();
        var lemmingManager = new LemmingManager(levelLemmings, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
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

        var updateScheduler = new UpdateScheduler(levelData.SuperLemmingMode, controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager, gadgetManager, skillSetManager);

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
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(levelCursor);

        var levelSprites = _levelObjectAssembler.GetLevelSprites();

        var controlPanelRenderer = new ClassicControlPanelRenderer(controlPanelSpriteBank, controlPanel, _fontBank);

        var levelCursorSprite = controlPanelSpriteBank.LevelCursorSprite;

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
            gadgetSpriteBank,
            _fontBank);

        foreach (var lemmingRenderer in levelSprites.OfType<LemmingRenderer>())
        {
            lemmingRenderer.UpdateLemmingState();
        }

        var wp = controlPanelSpriteBank.GetTexture("WhitePixel");
        foreach (var metalGrateRenderer in levelSprites.OfType<MetalGrateRenderer>())
        {
            metalGrateRenderer.SetWhitePixelTexture(wp);
        }

        var switchTexture = gadgetSpriteBank.GetTexture("switch");
        foreach (var switchRenderer in levelSprites.OfType<SwitchRenderer>())
        {
            switchRenderer.SetSwitchTexture(switchTexture);
        }

        var sawBladeTexture = gadgetSpriteBank.GetTexture("sawblade");
        foreach (var sawBladeRenderer in levelSprites.OfType<SawBladeRenderer>())
        {
            sawBladeRenderer.SetSawBladeTexture(sawBladeTexture);
        }

        return new LevelScreen(
            levelData,
            updateScheduler,
            inputController,
            levelTimer,
            skillSetManager,
            controlPanel,
            levelCursor,
            levelViewport,
            lemmingManager,
            terrainManager,
            gadgetManager,
            levelRenderer);
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _terrainPainter.Dispose();
        _levelObjectAssembler.Dispose();
    }
}