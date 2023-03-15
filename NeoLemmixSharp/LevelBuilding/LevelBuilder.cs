using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using System;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelBuilder : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly LevelReader _levelReader;

    public LevelBuilder(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _levelReader = new LevelReader();
    }

    public LevelScreen BuildLevel(string levelFilePath)
    {
        _levelReader.ReadLevel(levelFilePath);

        return null;
    }

    public void Dispose()
    {
        _levelReader.Dispose();
    }
}