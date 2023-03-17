using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Lemming;
using System;
using System.Linq;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelAssembler : IDisposable
{
    private Lemming[] _lemmings;

    public LevelAssembler()
    {
        var lemming0 = new Lemming
        {
            X = 213,
            Y = 0
        };

        var lemming1 = new Lemming
        {
            X = 126,
            Y = 42,
            Orientation = UpOrientation.Instance
        };

        var lemming2 = new Lemming
        {
            X = 60,
            Y = 20,
            Orientation = LeftOrientation.Instance
        };

        var lemming3 = new Lemming
        {
            X = 145,
            Y = 134,
            Orientation = RightOrientation.Instance
        };

        _lemmings = new[] { lemming0, lemming1, lemming2, lemming3 };
    }

    public ITickable[] GetLevelTickables()
    {
        return _lemmings.ToArray<ITickable>();
    }

    public IRenderable[] GetLevelRenderables(GraphicsDevice graphicsDevice)
    {
        return _lemmings.Select(l => new LemmingSprite(graphicsDevice, l))
            .ToArray<IRenderable>(); //new IRenderable[] { new LemmingSprite(graphicsDevice, _lemming0) };
    }

    public void Dispose()
    {

    }
}