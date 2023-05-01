using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Sprites;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;

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
        ContentManager content,
        LevelData levelData,
        TerrainSprite terrainSprite)
    {
        var spriteBankBuilder = new SpriteBankBuilder(_graphicsDevice);
        _spriteBank = spriteBankBuilder.BuildSpriteBank(content, levelData.ThemeData, terrainSprite, levelData.AllGadgetData);

        SetUpTestLemmings();

        levelData.SkillSet = new SkillSet()
        {
            NumberOfBashers = 1,
            NumberOfBlockers = 2,
            NumberOfBombers = 3,
            NumberOfBuilders = 4,
            NumberOfClimbers = 5,
            NumberOfCloners = 6,
            NumberOfDiggers = 7,
            NumberOfDisarmers = 8,
            NumberOfFencers = 9,
            NumberOfFloaters = 10,
            NumberOfGliders = 11,
            NumberOfJumpers = 12,
            NumberOfLaserers = 13,
            NumberOfMiners = 14,
            NumberOfPlatformers = 15,
            NumberOfShimmiers = 16,
            NumberOfSliders = 17,
            NumberOfStackers = 18,
            NumberOfStoners = 19,
            NumberOfSwimmers = 20,
            NumberOfWalkers = 21
        };
    }

    public SpriteBank GetSpriteBank()
    {
        return _spriteBank!;
    }

    public ITickable[] GetLevelTickables()
    {
        return _lemmings.ToArray<ITickable>();
    }

    public ISprite[] GetLevelRenderables()
    {
        return _lemmings
            .Select(GetLemmingSprite)
            .ToArray<ISprite>();
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
            Orientation = LeftOrientation.Instance,
            IsClimber = true
        };

        var lemming3 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(145, 134),
            Orientation = RightOrientation.Instance,
            IsFloater = true
        };

        var lemming4 = new Lemming
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(232, 130),
            Orientation = LeftOrientation.Instance,
            FacingDirection = LeftFacingDirection.Instance,
            CurrentAction = BuilderAction.Instance,
            Debug = true
        };

        BuilderAction.Instance.TransitionLemmingToAction(lemming4, false);

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
        _lemmings.Add(lemming4);
    }
}