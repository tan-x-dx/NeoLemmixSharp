using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Engine.Actions;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Gadgets;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering.Level.Ui;
using NeoLemmixSharp.Rendering.Level.Viewport;
using NeoLemmixSharp.Rendering.Level.Viewport.Gadget;
using NeoLemmixSharp.Rendering.Level.Viewport.Lemming;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;

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
        SetUpTestLemmings();
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
            .Select(l => l.Renderer)
            .ToArray<ILevelObjectRenderer>();
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
        var lemming0 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(160, 0),

            Debug = true
        };

        var lemming1 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(170, 0),

            Debug = true
        };

        var lemming2 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(180, 0),

            Debug = true
        };

        var lemming3 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(190, 0),

            Debug = true
        };

        var lemming4 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(200, 0),

            Debug = true
        };

        var lemming5 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(470, 76),
            LevelPosition = new LevelPosition(210, 0),

            Debug = true
        };

        var lemmingA = new Lemming(
            orientation: UpOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //    LevelPosition = new LevelPosition(770, 10),
            LevelPosition = new LevelPosition(126, 42),
            // FacingDirection = LeftFacingDirection.Instance
        };


        var lemmingB = new Lemming(
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //  LevelPosition = new LevelPosition(692, 72),
            LevelPosition = new LevelPosition(60, 20),
            IsClimber = true,
            //   FastForwardTime = 1
        };

        var lemmingC = new Lemming(
            orientation: RightOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(145, 134),
            IsFloater = true
        };

        var lemmingD = new Lemming(
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance,
            currentAction: BuilderAction.Instance)
        {
            //     LevelPosition = new LevelPosition(612, 42),
            LevelPosition = new LevelPosition(232, 130),
            Debug = true
        };

        BuilderAction.Instance.TransitionLemmingToAction(lemmingD, false);

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
        _lemmings.Add(lemming4);
        _lemmings.Add(lemming5);
        _lemmings.Add(lemmingA);
        _lemmings.Add(lemmingB);
        _lemmings.Add(lemmingC);
        _lemmings.Add(lemmingD);

        lemming0.State.TeamAffiliation = Team.Team0;
        lemming1.State.TeamAffiliation = Team.Team1;
        lemming2.State.TeamAffiliation = Team.Team2;
        lemming3.State.TeamAffiliation = Team.Team3;
        lemming3.State.IsAthlete = true;
        lemming4.State.TeamAffiliation = Team.Team4;
        lemming5.State.TeamAffiliation = Team.Team5;
    }

    private void SetUpLemmings()
    {

    }

    private void SetUpGadgets()
    {

    }
}