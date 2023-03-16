using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using System;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly LevelReader _levelReader;
    private readonly LevelPainter _levelPainter;
    private readonly LevelAssembler _levelAssembler;

    public LevelBuilder(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _levelReader = new LevelReader();
        _levelPainter = new LevelPainter(graphicsDevice, spriteBatch);
        _levelAssembler = new LevelAssembler();
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        _levelPainter.PaintLevel(
            _levelReader.LevelData,
            _levelReader.AllTerrainGroups,
            _levelReader.AllTerrainData);
        
        if (string.IsNullOrWhiteSpace(_levelReader.LevelData.LevelTitle))
        {
            _levelReader.LevelData.LevelTitle = "Untitled";
        }

        return new LevelScreen(
            _levelReader.LevelData,
            _levelPainter.GetLevelTerrain())
        {
            LevelObjects = _levelAssembler.GetLevelTickables(),
            LevelSprites = _levelAssembler.GetLevelRenderables(_graphicsDevice),
            TerrainSprite = _levelPainter.GetTerrainSprite(),
            Viewport = new NeoLemmixViewPort()
        };
    }

    public void Dispose()
    {
        _levelReader.Dispose();
        _levelPainter.Dispose();
        _levelAssembler.Dispose();
    }
}