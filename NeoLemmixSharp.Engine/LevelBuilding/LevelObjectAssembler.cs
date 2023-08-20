using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Engine.Engine.Teams;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.Gadget;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectAssembler : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly List<Lemming> _lemmings = new();
    private readonly List<IGadget> _gadgets = new();
    private readonly List<IViewportObjectRenderer> _gadgetRenderers = new();

    private readonly LemmingSpriteBankBuilder _lemmingSpriteBankBuilder;
    private readonly GadgetSpriteBankBuilder _gadgetSpriteBankBuilder;
    private readonly ControlPanelSpriteBankBuilder _controlPanelSpriteBankBuilder;

    public LevelObjectAssembler(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager,
        SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _lemmingSpriteBankBuilder = new LemmingSpriteBankBuilder();
        _gadgetSpriteBankBuilder = new GadgetSpriteBankBuilder(_graphicsDevice);
        _controlPanelSpriteBankBuilder = new ControlPanelSpriteBankBuilder(graphicsDevice, contentManager);
    }

    public void AssembleLevelObjects(
        ContentManager content,
        LevelData levelData)
    {
        //SetUpTestLemmings();
        //SetUpLemmings();
        //SetUpGadgets(content, levelData.AllGadgetData);

        var x = new List<LemmingSkill>
        {
            BuilderSkill.Instance,
            ClimberSkill.Instance,
            DiggerSkill.Instance,
            MinerSkill.Instance
        };

        var i = 10;
        //  foreach (var team in Team.AllItems)
        {
            foreach (var skill in x)
            {
                var item = new SkillSetData
                {
                    SkillName = skill.LemmingSkillName,
                    NumberOfSkills = i,
                    TeamId = Team.AllItems[0].Id,
                };

                levelData.SkillSetData.Add(item);
                i++;
            }
        }
    }

    public Lemming[] GetLevelLemmings()
    {
        SetUpTestLemmings();

        return _lemmings.ToArray();
    }

    public IGadget[] GetLevelGadgets()
    {
        return _gadgets.ToArray();
    }

    public IViewportObjectRenderer[] GetLevelSprites()
    {
        var lemmingSprites = _lemmings
            .Select(l => l.Renderer);

        return lemmingSprites
            .Concat(_gadgetRenderers)
            .ToArray();
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
        var lemmingX = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(160, 0),

            Debug = true
        };

        var lemmingA = new Lemming(
            orientation: UpOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(126, 42)
        };


        var lemmingB = new Lemming(
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(60, 20),
            State =
            {
                IsClimber = true,
            },
            FastForwardTime = 1
        };

        var lemmingE = new Lemming(
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(60, 24),
        };

        var lemmingC = new Lemming(
            orientation: RightOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(145, 134),
            State =
            {
                IsFloater = true
            }
        };

        var lemmingD = new Lemming(
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance,
            currentAction: BuilderAction.Instance)
        {
            LevelPosition = new LevelPosition(232, 130)
        };

        BuilderAction.Instance.TransitionLemmingToAction(lemmingD, false);

        var lemming0 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(2, 152)
        };

        var lemming1 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(6, 152)
        };

        var lemming2 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(10, 152)
        };

        var lemming3 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(14, 152)
        };

        var lemming4 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(18, 152)
        };

        var lemming5 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(22, 152)
        };


        var lemming6 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(290, 152)
        };

        var lemming7 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(294, 152)
        };

        var lemming8 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(298, 152)
        };

        var lemming9 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(302, 152)
        };

        var lemming10 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(306, 152)
        };

        var lemming11 = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(310, 152)
        };

        var miner = new Lemming(
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance,
            currentAction: MinerAction.Instance)
        {
            LevelPosition = new LevelPosition(110, 19)
        };
        MinerAction.Instance.TransitionLemmingToAction(miner, false);

        _lemmings.Add(lemmingX);

        _lemmings.Add(lemming0);
        _lemmings.Add(lemming1);
        _lemmings.Add(lemming2);
        _lemmings.Add(lemming3);
        _lemmings.Add(lemming4);
        _lemmings.Add(lemming5);
        _lemmings.Add(lemming6);
        _lemmings.Add(lemming7);
        _lemmings.Add(lemming8);
        _lemmings.Add(lemming9);
        _lemmings.Add(lemming10);
        _lemmings.Add(lemming11);

        _lemmings.Add(lemmingA);
        _lemmings.Add(lemmingB);
        _lemmings.Add(lemmingC);
        _lemmings.Add(lemmingD);
        _lemmings.Add(lemmingE);

        _lemmings.Add(miner);

        lemming0.State.TeamAffiliation = Team.AllItems[0];
        lemming1.State.TeamAffiliation = Team.AllItems[1];
        lemming2.State.TeamAffiliation = Team.AllItems[2];
        lemming3.State.TeamAffiliation = Team.AllItems[3];
        lemming4.State.TeamAffiliation = Team.AllItems[4];
        lemming5.State.TeamAffiliation = Team.AllItems[5];

        lemming6.State.TeamAffiliation = Team.AllItems[0];
        lemming7.State.TeamAffiliation = Team.AllItems[1];
        lemming8.State.TeamAffiliation = Team.AllItems[2];
        lemming9.State.TeamAffiliation = Team.AllItems[3];
        lemming10.State.TeamAffiliation = Team.AllItems[4];
        lemming11.State.TeamAffiliation = Team.AllItems[5];
        lemming6.State.IsSwimmer = true;
        lemming7.State.IsSwimmer = true;
        lemming8.State.IsSwimmer = true;
        lemming9.State.IsSwimmer = true;
        lemming10.State.IsSwimmer = true;
        lemming11.State.IsSwimmer = true;

        lemming0.State.IsNeutral = true;
        lemming1.State.IsNeutral = true;
        lemming2.State.IsNeutral = true;
        lemming3.State.IsNeutral = true;
        lemming4.State.IsNeutral = true;
        lemming5.State.IsNeutral = true;
        lemming6.State.IsNeutral = true;
        lemming7.State.IsNeutral = true;
        lemming8.State.IsNeutral = true;
        lemming9.State.IsNeutral = true;
        lemming10.State.IsNeutral = true;
        lemming11.State.IsNeutral = true;

        lemming0.State.IsZombie = true;
        lemming1.State.IsZombie = true;
        lemming2.State.IsZombie = true;
        lemming3.State.IsZombie = true;
        lemming4.State.IsZombie = true;
        lemming5.State.IsZombie = true;
    }

    private void SetUpLemmings()
    {

    }

    private void SetUpGadgets(ContentManager contentManager, ICollection<GadgetData> allGadgetData)
    {
        var texture = Texture2D.FromFile(_graphicsDevice, "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\styles\\namida_systemtest\\objects\\nineslicetest.png");

        var sW = texture.Width;
        var sH = texture.Height;
        var sL = 8;
        var sR = texture.Width - 6;
        var sT = 3;
        var sB = texture.Height - 2;

        /*
        var texture = contentManager.Load<Texture2D>("sprites/style/common/water");

        var sW = 64;
        var sH = 32;
        var sL = 0;
        var sR = 64;
        var sT = 16;
        var sB = 32;
        */

        var c = new RectangularLevelRegion(20, 20, 64, 32);

        /*  var water = new ResizeableGadget(0, GadgetType.Water, DownOrientation.Instance, c);
          var r = new NineSliceRenderer(c, texture, sW, sH, sT, sB, sL, sR);

          _gadgets.Add(water);
          _gadgetRenderers.Add(r);*/

        foreach (var gadgetData in allGadgetData)
        {
            _gadgetSpriteBankBuilder.LoadGadgetSprite(gadgetData);
        }
    }
}