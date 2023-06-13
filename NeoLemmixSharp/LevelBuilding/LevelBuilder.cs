﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelGadgets;
using NeoLemmixSharp.Engine.LevelPixels;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Text;
using System;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly ContentManager _content;
    private readonly FontBank _fontBank;
    private readonly LevelReader _levelReader;
    private readonly LevelPainter _levelPainter;
    private readonly LevelAssembler _levelAssembler;

    private SpriteBank _spriteBank;

    public LevelBuilder(
        ContentManager content,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        FontBank fontBank)
    {
        _content = content;
        _fontBank = fontBank;
        _levelReader = new LevelReader();
        _levelPainter = new LevelPainter(graphicsDevice);
        _levelAssembler = new LevelAssembler(graphicsDevice, spriteBatch);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _levelPainter.PaintLevel(
            _levelReader.LevelData);

        var terrainSprite = _levelPainter.GetTerrainSprite();

        _levelAssembler.AssembleLevel(
            _content,
            _levelReader.LevelData,
            terrainSprite);

        _spriteBank = _levelAssembler.GetSpriteBank();

        var levelData = _levelReader.LevelData;
        var levelLemmings = _levelAssembler.GetLevelLemmings();
        var levelGadgets = _levelAssembler.GetLevelGadgets();

        var pixelData = _levelPainter.GetPixelData();

        var pixelManager = new PixelManager(
            levelData.LevelWidth,
            levelData.LevelHeight,
            BoundaryBehaviourType.Void,
            BoundaryBehaviourType.Void);

        pixelManager.SetData(pixelData, levelGadgets, terrainSprite);

        return new LevelScreen(
            levelData,
            levelLemmings,
            levelGadgets,
            pixelManager,
            _spriteBank);
    }

    public ISprite[] GetLevelSprites() => _levelAssembler.GetLevelSprites();
    public SpriteBank GetSpriteBank() => _spriteBank;

    public void Dispose()
    {
        _levelReader.Dispose();
        _levelPainter.Dispose();
        _levelAssembler.Dispose();
    }
}