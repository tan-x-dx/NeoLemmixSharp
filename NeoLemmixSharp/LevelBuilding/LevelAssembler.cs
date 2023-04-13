using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Sprites;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

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
        TerrainSprite terrainSprite)
    {
        var spriteBankBuilder = new SpriteBankBuilder(_graphicsDevice);
        _spriteBank = spriteBankBuilder.BuildSpriteBank(levelData.ThemeData, terrainSprite, levelData.AllGadgetData);

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
            LevelPosition = new Point(200, 0),
            FacingDirection = LeftFacingDirection.Instance
        };

        var lemming1 = new Lemming
        {
            //    LevelPosition = new LevelPosition(770, 10),
            LevelPosition = new Point(126, 42),
            Orientation = UpOrientation.Instance,
            // FacingDirection = LeftFacingDirection.Instance
        };

        var lemming2 = new Lemming
        {
            //  LevelPosition = new LevelPosition(692, 72),
            LevelPosition = new Point(60, 20),
            Orientation = LeftOrientation.Instance,
            IsClimber = true
        };

        var lemming3 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new Point(145, 134),
            Orientation = RightOrientation.Instance,
            IsFloater = true
        };

        var lemming4 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new Point(232, 130),
            Orientation = LeftOrientation.Instance,
            FacingDirection = LeftFacingDirection.Instance,
            CurrentAction = BuilderAction.Instance,
            Debug = true
        };

        BuilderAction.Instance.OnTransitionToAction(lemming4, false);

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
        _lemmings.Add(lemming4);
    }
}