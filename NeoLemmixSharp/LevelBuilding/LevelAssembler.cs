using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Lemming;
using System;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelAssembler : IDisposable
{
    private Lemming _lemming;

    public LevelAssembler()
    {

        _lemming = new Lemming
        {
            X = 160,
            Y = 80
        };
    }

    public ITickable[] GetLevelTickables()
    {
        return new ITickable[] { _lemming };
    }

    public IRenderable[] GetLevelRenderables(GraphicsDevice graphicsDevice)
    {
        return new IRenderable[] { new LemmingSprite(graphicsDevice, _lemming) };
    }

    public void Dispose()
    {

    }
}