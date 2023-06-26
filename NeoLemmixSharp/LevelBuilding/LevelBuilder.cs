using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.BoundaryBehaviours;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Input;
using NeoLemmixSharp.Engine.Terrain;
using NeoLemmixSharp.Rendering.Level;
using NeoLemmixSharp.Rendering.Level.Ui;
using NeoLemmixSharp.Rendering.Level.Viewport.Background;
using NeoLemmixSharp.Rendering.Level.Viewport.Lemming;
using NeoLemmixSharp.Rendering.Text;
using System;
using System.Linq;
using LevelRenderer = NeoLemmixSharp.Rendering.Level.LevelRenderer;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _content;
    private readonly FontBank _fontBank;
    private readonly LevelReader _levelReader;
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
        _levelReader = new LevelReader();
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
        var levelLemmings = _levelAssembler.GetLevelLemmings();
        var levelGadgets = _levelAssembler.GetLevelGadgets();
        var pixelData = _terrainPainter.GetPixelData();

        var horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(levelData.HorizontalBoundaryBehaviour, levelData.LevelWidth);
        var verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(levelData.VerticalBoundaryBehaviour, levelData.LevelHeight);

        var horizontalViewPortBehaviour = BoundaryHelpers.GetHorizontalViewPortBehaviour(levelData.HorizontalViewPortBehaviour, levelData.LevelWidth);
        var verticalViewPortBehaviour = BoundaryHelpers.GetVerticalViewPortBehaviour(levelData.VerticalViewPortBehaviour, levelData.LevelHeight);

        var inputController = new LevelInputController();
        var skillSetManager = new SkillSetManager(levelData.SkillSetData);
        var controlPanel = new LevelControlPanel(skillSetManager, inputController);
        var levelCursor = new LevelCursor(horizontalBoundaryBehaviour, verticalBoundaryBehaviour, controlPanel, inputController);
        var levelViewport = new LevelViewport(levelCursor, inputController, horizontalViewPortBehaviour, verticalViewPortBehaviour, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        var terrainRenderer = new TerrainRenderer(terrainTexture, levelViewport);

        var terrainManager = new TerrainManager(
            levelData.LevelWidth,
            levelData.LevelHeight,
            pixelData,
            levelGadgets,
            terrainRenderer,
            levelData.HorizontalBoundaryBehaviour,
            levelData.VerticalBoundaryBehaviour);

        var levelSprites = _levelAssembler.GetLevelSprites();

        var lemmingSpriteBank = _levelAssembler.GetLemmingSpriteBank();
        var gadgetSpriteBank = _levelAssembler.GetGadgetSpriteBank();
        var controlPanelSpriteBank = _levelAssembler.GetControlPanelSpriteBank(levelCursor);

        var controlPanelRenderer = new ClassicControlPanelRenderer(controlPanelSpriteBank, _fontBank, controlPanel);

        var levelCursorSprite = controlPanelSpriteBank.LevelCursorSprite;

        var backgroundRenderer = new SolidColourBackgroundRenderer(controlPanelSpriteBank, levelViewport, new Color(24, 24, 60));

        var levelRenderer = new LevelRenderer(
            levelData.LevelWidth,
            levelData.LevelHeight,
            levelViewport,
            backgroundRenderer,
            terrainRenderer,
            levelSprites,
            levelCursorSprite,
            controlPanelRenderer,
            lemmingSpriteBank,
            gadgetSpriteBank,
            _fontBank);

        foreach (var lemmingRenderer in levelSprites.OfType<LemmingRenderer>())
        {
            lemmingRenderer.UpdateLemmingState();
        }

        return new LevelScreen(
            levelData,
            terrainManager,
            levelLemmings,
            levelGadgets,
            inputController,
            controlPanel,
            levelCursor,
            levelViewport,
            levelRenderer);
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _terrainPainter.Dispose();
        _levelAssembler.Dispose();
    }
}