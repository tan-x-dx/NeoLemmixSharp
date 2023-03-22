using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly LevelReader _levelReader;
    private readonly LevelPainter _levelPainter;
    private readonly LevelAssembler _levelAssembler;

    public LevelBuilder(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _levelReader = new LevelReader();
        _levelPainter = new LevelPainter(graphicsDevice);
        _levelAssembler = new LevelAssembler(graphicsDevice, spriteBatch);
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _levelPainter.PaintLevel(
            _levelReader.LevelData,
            _levelReader.AllTerrainGroups,
            _levelReader.AllTerrainData);

        _levelAssembler.AssembleLevel(
            _levelReader.LevelData,
            _levelReader.ThemeData,
            _levelPainter.GetTerrainSprite());

        return new LevelScreen(
            _levelReader.LevelData,
            _levelPainter.GetTerrainData(),
            _levelAssembler.GetSpriteBank())
        {
            LevelObjects = _levelAssembler.GetLevelTickables(),
            LevelSprites = _levelAssembler.GetLevelRenderables()
        };
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _levelPainter.Dispose();
        _levelAssembler.Dispose();
    }
}