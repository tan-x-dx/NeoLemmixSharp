using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Terrain;
using NeoLemmixSharp.Engine.Engine.Timer;
using NeoLemmixSharp.Engine.Engine.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport.Background;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using NeoLemmixSharp.Io.LevelReading;
using NeoLemmixSharp.Io.LevelReading.Nxlv;
using Viewport = NeoLemmixSharp.Engine.Engine.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _content;
    private readonly FontBank _fontBank;
    private readonly ILevelReader _levelReader;
    private readonly TerrainPainter _terrainPainter;
    private readonly LevelAssembler _levelAssembler;

    public LevelBuilder(
        ContentManager content,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank)
    {
        _content = content;
        _fontBank = fontBank;
        _levelReader = new NxlvLevelReader();
        _terrainPainter = new TerrainPainter(graphicsDevice);
        _levelAssembler = new LevelAssembler(graphicsDevice, content, spriteBatch);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _terrainPainter.PaintLevel(_levelReader.LevelData);

        var terrainTexture = _terrainPainter.GetTerrainTexture();

        _levelAssembler.AssembleLevel(
            _content,
            _levelReader.LevelData);

        var levelData = _levelReader.LevelData;
        var lemmingSpriteBank = _levelAssembler.GetLemmingSpriteBank();
        lemmingSpriteBank.SetTeamColors();

        var levelGadgets = _levelAssembler.GetLevelGadgets();
        GadgetCollections.SetGadgets(levelGadgets);

        var inputController = new LevelInputController();
        var skillSetManager = new SkillSetManager(levelData.SkillSetData);
        var controlPanel = new LevelControlPanel(skillSetManager, inputController);
        LevelTimer levelTimer = levelData.TimeLimit.HasValue
            ? new CountDownLevelTimer(levelData.TimeLimit.Value)
            : new CountUpLevelTimer();

        var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
        var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

        var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
        var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);

        var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, controlPanel, inputController, skillSetManager);
        var levelViewport = new Viewport(levelCursor, horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        var levelLemmings = _levelAssembler.GetLevelLemmings();
        var lemmingManager = new LemmingManager(levelLemmings);
        var updateScheduler = new UpdateScheduler(levelData.SuperLemmingMode, controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager);

        var terrainRenderer = new TerrainRenderer(terrainTexture, levelViewport);

        var pixelData = _terrainPainter.GetPixelData();

        var terrainManager = new TerrainManager(
            levelData.LevelWidth,
            levelData.LevelHeight,
            pixelData,
            terrainRenderer,
            levelData.HorizontalBoundaryBehaviour,
            levelData.VerticalBoundaryBehaviour);

        var levelSprites = _levelAssembler.GetLevelSprites();

        var gadgetSpriteBank = _levelAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelAssembler.GetControlPanelSpriteBank(levelCursor);

        var controlPanelRenderer = new ClassicControlPanelRenderer(controlPanelSpriteBank, _fontBank, controlPanel);

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

        return new LevelScreen(
            levelData,
            levelGadgets,
            updateScheduler,
            inputController,
            levelTimer,
            skillSetManager,
            controlPanel,
            levelCursor,
            levelViewport,
            lemmingManager,
            terrainManager,
            levelRenderer);
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _terrainPainter.Dispose();
        _levelAssembler.Dispose();
    }
}