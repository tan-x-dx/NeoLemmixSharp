using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoLemmixSharp.Engine.Actions;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Gadgets;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Rendering.Level.Ui;
using NeoLemmixSharp.Rendering.Level.Viewport;
using NeoLemmixSharp.Rendering.Level.Viewport.Gadget;
using NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

namespace NeoLemmixSharp.LevelBuilding;

public sealed class LevelAssembler : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly List<Lemming> _lemmings = new();

    private readonly LemmingSpriteBankBuilder _lemmingSpriteBankBuilder;
    private readonly GadgetSpriteBankBuilder _gadgetSpriteBankBuilder;
    private readonly ControlPanelSpriteBankBuilder _controlPanelSpriteBankBuilder;

    public LevelAssembler(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager,
        SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _lemmingSpriteBankBuilder = new LemmingSpriteBankBuilder();
        _gadgetSpriteBankBuilder = new GadgetSpriteBankBuilder();
        _controlPanelSpriteBankBuilder = new ControlPanelSpriteBankBuilder(graphicsDevice, contentManager);
    }

    public void AssembleLevel(
        ContentManager content,
        LevelData levelData)
    {
        // SetUpTestLemmings();
        SetUpLemmings();
        SetUpGadgets();

        levelData.SkillSetData = new SkillSetData
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

    public Lemming[] GetLevelLemmings()
    {
        return _lemmings.ToArray();
    }

    public Gadget[] GetLevelGadgets()
    {
        return Array.Empty<Gadget>();
    }

    public ILevelObjectRenderer[] GetLevelSprites()
    {
        return _lemmings
            .Select(GetLemmingSprite)
            .ToArray<ILevelObjectRenderer>();
    }

    private LemmingRenderer GetLemmingSprite(Lemming lemming)
    {
        return new LemmingRenderer(lemming);
    }

    public void Dispose()
    {
        //  _spriteBank = null;
        _lemmings.Clear();
    }

    public LemmingSpriteBank GetLemmingSpriteBank()
    {
        return DefaultLemmingSpriteBank.DefaultLemmingSprites;
    }

    public GadgetSpriteBank GetGadgetSpriteBank()
    {
        return new GadgetSpriteBank(); // _gadgetSpriteBankBuilder.BuildGadgetSpriteBank();
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(LevelCursor levelCursor)
    {
        return _controlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(levelCursor);
    }

    private void SetUpTestLemmings()
    {
        var lemming0 = new Lemming
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(200, 0),
            FacingDirection = LeftFacingDirection.Instance,
            Orientation = DownOrientation.Instance,

            Debug = true
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
            IsClimber = true,
            FastForwardTime = 1
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

    private void SetUpLemmings()
    {

    }

    private void SetUpGadgets()
    {

    }
}