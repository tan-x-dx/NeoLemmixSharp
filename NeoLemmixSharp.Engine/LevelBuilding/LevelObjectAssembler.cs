using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional.SawBlade;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
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
    private readonly List<GadgetBase> _gadgets = new();
    private readonly List<IViewportObjectRenderer> _gadgetRenderers = new();

    private readonly LemmingSpriteBankBuilder _lemmingSpriteBankBuilder;
    private readonly GadgetSpriteBankBuilder _gadgetSpriteBankBuilder;
    private readonly ControlPanelSpriteBankBuilder _controlPanelSpriteBankBuilder;

    public LevelObjectAssembler(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager,
        SpriteBatch spriteBatch,
        RootDirectoryManager rootDirectoryManager)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _lemmingSpriteBankBuilder = new LemmingSpriteBankBuilder();
        _gadgetSpriteBankBuilder = new GadgetSpriteBankBuilder(_graphicsDevice, contentManager, rootDirectoryManager);
        _controlPanelSpriteBankBuilder = new ControlPanelSpriteBankBuilder(graphicsDevice, contentManager);
    }

    public void AssembleLevelObjects(
        ContentManager contentManager,
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
                    NumberOfSkills = 100,
                    TeamId = Team.AllItems[0].Id,
                };

                levelData.SkillSetData.Add(item);
                i++;
            }
        }

        var id = 0;
        var p = new RectangularLevelRegion(250, 90, 40, 2);
        var input = new MetalGrateGadget.MetalGrateGadgetInput("input");

        var metalGrateGadget = new MetalGrateGadget(
            id++,
            p,
            input,
            true);

        _gadgets.Add(metalGrateGadget);
        _gadgetRenderers.Add(new MetalGrateRenderer(metalGrateGadget));

        p = new RectangularLevelRegion(296, 142, 19, 13);
        var switchGadget = new SwitchGadget(id++, p, true);
        switchGadget.Output.RegisterInput(input);

        _gadgets.Add(switchGadget);
        _gadgetRenderers.Add(new SwitchRenderer(switchGadget));

        var sawBladeGadget = LoadSawBlade(contentManager);
        
        var mover = new GadgetMover(
            3,
            new RectangularLevelRegion(0, 0, 1, 1),
            new IMoveableGadget[] { sawBladeGadget },
            0, 1, 1);

        switchGadget.Output.RegisterInput(mover.GetInputWithName("Input")!);
        _gadgets.Add(mover);

    }

    private SawBladeGadget LoadSawBlade(ContentManager contentManager)
    {
        var p = new RectangularLevelRegion(100, 100, 14, 14);
        var sawBladeGadget = new SawBladeGadget(2, p);

        _gadgets.Add(sawBladeGadget);
        _gadgetRenderers.Add(new SawBladeRenderer(sawBladeGadget));

        var numberOfFrames = 4;

        var s = new SpriteRotationReflectionProcessor<SawBladeHitMask>(_graphicsDevice);

        using var texture = contentManager.Load<Texture2D>("sprites/style/common/spinner_mask");

        var spriteWidth = texture.Width;
        var spriteHeight = texture.Height / numberOfFrames;

        var sawBladeHitMasks = new List<SawBladeHitMask>();

        for (var i = 0; i < numberOfFrames; i++)
        {
            var sawBladeHitMask = s.CreateSpriteType(
                texture,
                DownOrientation.Instance,
                RightFacingDirection.Instance,
                spriteWidth,
                spriteHeight,
                numberOfFrames,
                1,
                p.TopLeft,
                (a, b, c, d, e, f) => ItemCreator(a, b, c, d, e, f, i, sawBladeGadget));

            sawBladeHitMasks.Add(sawBladeHitMask);
        }

        sawBladeGadget.SetHitMasks(sawBladeHitMasks.ToArray());

        return sawBladeGadget;
    }

    private static SawBladeHitMask ItemCreator(
            Texture2D texture,
            int spriteWidth,
            int spriteHeight,
            int numberOfFrames,
            int numberOfLayers,
            LevelPosition anchorPoint,
            int frame,
            SawBladeGadget sawBladeGadget)
    {
        var uints = new uint[texture.Width * texture.Height];
        texture.GetData(uints);

        var levelPositions = new List<LevelPosition>();

        var y0 = frame * spriteHeight;

        for (var x = 0; x < spriteWidth; x++)
        {
            for (var y = 0; y < spriteHeight; y++)
            {
                var index = x + spriteWidth * (y0 + y);

                var pixel = uints[index];

                if (pixel != 0U)
                {
                    var p = new LevelPosition(x, y);
                    if (levelPositions.Contains(p))
                    {
                        ;
                    }

                    levelPositions.Add(p);
                }
            }
        }

        texture.Dispose();

        return new SawBladeHitMask(sawBladeGadget, sawBladeGadget.GadgetBounds, levelPositions.ToArray());
    }

    public Lemming[] GetLevelLemmings()
    {
        SetUpTestLemmings();

        return _lemmings.ToArray();
    }

    public GadgetBase[] GetLevelGadgets()
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
        return _gadgetSpriteBankBuilder.BuildGadgetSpriteBank();
    }

    public ControlPanelSpriteBank GetControlPanelSpriteBank(LevelCursor levelCursor)
    {
        return _controlPanelSpriteBankBuilder.BuildControlPanelSpriteBank(levelCursor);
    }

    private void SetUpTestLemmings()
    {
        int id = 0;

        var lemmingX = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(160, 0),

            Debug = true
        };

        var lemmingA = new Lemming(
            id++,
            orientation: UpOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(126, 42)
        };


        var lemmingB = new Lemming(
            id++,
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
            id++,
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(60, 24),
        };

        var lemmingC = new Lemming(
            id++,
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
            id++,
            orientation: LeftOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance,
            currentAction: BuilderAction.Instance)
        {
            LevelPosition = new LevelPosition(232, 130)
        };

        BuilderAction.Instance.TransitionLemmingToAction(lemmingD, false);

        var lemming0 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(2, 152)
        };

        var lemming1 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(6, 152)
        };

        var lemming2 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(10, 152)
        };

        var lemming3 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(14, 152)
        };

        var lemming4 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(18, 152)
        };

        var lemming5 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: LeftFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(22, 152)
        };


        var lemming6 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(80, 40)
        };

        var lemming7 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(90, 40)
        };

        var lemming8 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(298, 152)
        };

        var lemming9 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(302, 152)
        };

        var lemming10 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(306, 152)
        };

        var lemming11 = new Lemming(
            id++,
            orientation: DownOrientation.Instance,
            facingDirection: RightFacingDirection.Instance)
        {
            LevelPosition = new LevelPosition(310, 152)
        };

        var miner = new Lemming(
            id++,
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