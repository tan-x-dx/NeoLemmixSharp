using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Skills;
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
    private readonly LevelObjectAssembler _levelObjectAssembler;

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
        _levelObjectAssembler = new LevelObjectAssembler(graphicsDevice, content, spriteBatch);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _terrainPainter.PaintLevel(_levelReader.LevelData);

        var terrainTexture = _terrainPainter.GetTerrainTexture();

        _levelObjectAssembler.AssembleLevelObjects(
            _content,
            _levelReader.LevelData);

        var levelData = _levelReader.LevelData;
        var lemmingSpriteBank = _levelObjectAssembler.GetLemmingSpriteBank();
        lemmingSpriteBank.SetTeamColors();

        var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
        var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

        var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
        var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);

        var levelGadgets = _levelObjectAssembler.GetLevelGadgets();
        var gadgetManager = new GadgetManager(levelGadgets, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        var inputController = new LevelInputController();
        var skillSetManager = new SkillSetManager(levelData.SkillSetData);
        var controlPanel = new LevelControlPanel(skillSetManager, inputController);

        foreach (var skillAssignButton in controlPanel.SkillAssignButtons)
        {
            var skillTrackingData = skillSetManager.GetSkillTrackingData(skillAssignButton.SkillAssignButtonId)!;
            skillAssignButton.UpdateSkillCount(skillTrackingData.SkillCount);
        }

        LevelTimer levelTimer = levelData.TimeLimit.HasValue
            ? new CountDownLevelTimer(levelData.TimeLimit.Value)
            : new CountUpLevelTimer();

        var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, controlPanel, inputController, skillSetManager);
        var levelViewport = new Viewport(levelCursor, horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        var levelLemmings = _levelObjectAssembler.GetLevelLemmings();
        var lemmingManager = new LemmingManager(levelLemmings);
        var updateScheduler = new UpdateScheduler(levelData.SuperLemmingMode, controlPanel, levelViewport, levelCursor, inputController, levelTimer, lemmingManager, skillSetManager);

        var terrainRenderer = new TerrainRenderer(terrainTexture, levelViewport);

        var pixelData = _terrainPainter.GetPixelData();

        var terrainManager = new TerrainManager(
            pixelData,
            gadgetManager,
            terrainRenderer,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        var levelSprites = _levelObjectAssembler.GetLevelSprites();

        var gadgetSpriteBank = _levelObjectAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelObjectAssembler.GetControlPanelSpriteBank(levelCursor);

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