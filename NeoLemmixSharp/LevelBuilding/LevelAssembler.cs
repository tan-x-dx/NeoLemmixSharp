using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelAssembler : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private SpriteBank? _spriteBank;

    private readonly List<Lemming> _lemmings = new();

    public LevelAssembler(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    public void AssembleLevel(
        LevelData levelData,
        ThemeData themeData,
        TerrainSprite terrainSprite)
    {
        var spriteBankBuilder = new SpriteBankBuilder(_graphicsDevice);
        _spriteBank = spriteBankBuilder.BuildSpriteBank(themeData, terrainSprite);

        SetUpTestLemmings();
    }

    public SpriteBank GetSpriteBank()
    {
        return _spriteBank!;
    }

    public ITickable[] GetLevelTickables()
    {
        return _lemmings.ToArray<ITickable>();
    }

    public IRenderable[] GetLevelRenderables()
    {
        return _lemmings
            .Select(GetLemmingSprite)
            .ToArray<IRenderable>();
    }

    private LemmingSprite GetLemmingSprite(Lemming lemming)
    {
        return new LemmingSprite(lemming);
    }

    public void Dispose()
    {
        _spriteBank = null;
        _lemmings.Clear();
    }

    private void SetUpTestLemmings()
    {
        var lemming0 = new Lemming
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(200, 0),
            FacingDirection = LeftFacingDirection.Instance
        };

        var lemming1 = new Lemming
        {
            //    LevelPosition = new LevelPosition(770, 10),
            LevelPosition = new LevelPosition(126, 42),
            Orientation = UpOrientation.Instance,
            // FacingDirection = LeftFacingDirection.Instance
        };

        var lemming2 = new Lemming
        {
            //  LevelPosition = new LevelPosition(692, 72),
            LevelPosition = new LevelPosition(60, 20),
            Orientation = LeftOrientation.Instance
        };

        var lemming3 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(145, 134),
            Orientation = RightOrientation.Instance
        };

        var lemming4 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(232, 130),
            Orientation = LeftOrientation.Instance,
            FacingDirection = LeftFacingDirection.Instance,
            NumberOfBricksLeft = LemmingConstants.StepsMax,
            CurrentAction = BuilderAction.Instance,
            Debug = true
        };

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
        _lemmings.Add(lemming4);
    }
}