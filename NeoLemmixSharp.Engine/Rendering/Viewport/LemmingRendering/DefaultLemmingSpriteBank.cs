using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public static class DefaultLemmingSpriteBank
{
    private delegate LemmingActionSprite GenerateLayers(Texture2D texture, Common.Point anchorPoint, Size spriteSize);

    public static LemmingSpriteBank DefaultLemmingSprites { get; private set; } = null!;

    public static void CreateDefaultLemmingSpriteBank(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        LemmingActionSprite.Initialise(graphicsDevice);

#pragma warning disable IDE0039
        LemmingActionLayerRenderer.GetLayerColor getLemmingHairColor = l => l.State.HairColor;
        LemmingActionLayerRenderer.GetLayerColor getLemmingSkinColor = l => l.State.SkinColor;
        LemmingActionLayerRenderer.GetLayerColor getLemmingFootColor = l => l.State.FootColor;
        LemmingActionLayerRenderer.GetLayerColor getLemmingBodyColor = l => l.State.BodyColor;
        LemmingActionLayerRenderer.GetLayerColor getLemmingMiscColor = _ => Color.Magenta;
#pragma warning restore IDE0039

        var actionSprites = new LemmingActionSprite[EngineConstants.NumberOfLemmingActions];

        CreateFourLayerSprite(AscenderAction.Instance, new Point(2, 10));
        CreateFiveLayerSprite(BasherAction.Instance, new Point(8, 10));
        CreateFourLayerSprite(BlockerAction.Instance, new Point(5, 13));
        CreateSixLayerTrueColorSprite(BuilderAction.Instance, new Point(3, 13));
        CreateFourLayerSprite(ClimberAction.Instance, new Point(8, 12));
        CreateFourLayerSprite(DehoisterAction.Instance, new Point(5, 13));
        CreateFiveLayerSprite(DiggerAction.Instance, new Point(7, 12));
        CreateFiveLayerTrueColorSprite(DisarmerAction.Instance, new Point(1, 11));
        CreateFourLayerSprite(DrownerAction.Instance, new Point(5, 10));
        CreateFourLayerSprite(ExiterAction.Instance, new Point(2, 16));
        CreateOneLayerTrueColorSprite(ExploderAction.Instance, new Point(17, 21));
        CreateFourLayerSprite(FallerAction.Instance, new Point(3, 10));
        CreateSixLayerTrueColorSprite(FencerAction.Instance, new Point(3, 10));
        CreateFiveLayerTrueColorSprite(FloaterAction.Instance, new Point(4, 16));
        CreateFiveLayerTrueColorSprite(GliderAction.Instance, new Point(5, 16));
        CreateFourLayerSprite(HoisterAction.Instance, new Point(5, 12));
        CreateFourLayerSprite(JumperAction.Instance, new Point(2, 10));
        CreateFiveLayerTrueColorSprite(LasererAction.Instance, new Point(3, 10));
        CreateFiveLayerSprite(MinerAction.Instance, new Point(7, 13));
        CreateFourLayerSprite(OhNoerAction.Instance, new Point(3, 10));
        CreateSixLayerTrueColorSprite(PlatformerAction.Instance, new Point(3, 13));
        CreateFourLayerSprite(ReacherAction.Instance, new Point(3, 9));
        CreateFourLayerSprite(ShimmierAction.Instance, new Point(3, 8));
        CreateFourLayerSprite(ShruggerAction.Instance, new Point(3, 10));
        CreateFourLayerSprite(SliderAction.Instance, new Point(4, 11));
        CreateFourLayerSprite(SplatterAction.Instance, new Point(7, 10));
        CreateSixLayerTrueColorSprite(StackerAction.Instance, new Point(3, 13));
        CreateOneLayerTrueColorSprite(StonerAction.Instance, new Point(17, 21));
        CreateFourLayerSprite(SwimmerAction.Instance, new Point(6, 8));
        CreateFiveLayerTrueColorSprite(VaporiserAction.Instance, new Point(5, 14));
        CreateFourLayerSprite(WalkerAction.Instance, new Point(2, 10));
        CreateFourLayerSprite(RotateClockwiseAction.Instance, new Point(2, 10));
        CreateFourLayerSprite(RotateCounterclockwiseAction.Instance, new Point(2, 10));
        CreateFourLayerSprite(RotateHalfAction.Instance, new Point(2, 10));

        var teamColorData = GenerateDefaultTeamColorData();

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites, teamColorData);

        return;

        void CreateOneLayerTrueColorSprite(LemmingAction action, Common.Point anchorPoint)
        {
            CreateSprite(
                action,
                1,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateFourLayerSprite(LemmingAction action, Common.Point anchorPoint)
        {
            CreateSprite(
                action,
                4,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, getLemmingHairColor),
                        new(t, s.W, getLemmingSkinColor),
                        new(t, s.W * 2, getLemmingFootColor),
                        new(t, s.W * 3, getLemmingBodyColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateFiveLayerSprite(LemmingAction action, Common.Point anchorPoint)
        {
            CreateSprite(
                action,
                5,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, getLemmingHairColor),
                        new(t, s.W, getLemmingSkinColor),
                        new(t, s.W * 2, getLemmingFootColor),
                        new(t, s.W * 3, getLemmingBodyColor),
                        new(t, s.W * 4, getLemmingMiscColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateFiveLayerTrueColorSprite(LemmingAction action, Common.Point anchorPoint)
        {
            CreateSprite(
                action,
                5,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t),
                        new(t, s.W, getLemmingHairColor),
                        new(t, s.W * 2, getLemmingSkinColor),
                        new(t, s.W * 3, getLemmingFootColor),
                        new(t, s.W * 4, getLemmingBodyColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateSixLayerTrueColorSprite(LemmingAction action, Common.Point anchorPoint)
        {
            CreateSprite(
                action,
                6,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t),
                        new(t, s.W, getLemmingHairColor),
                        new(t, s.W * 2, getLemmingSkinColor),
                        new(t, s.W * 3, getLemmingFootColor),
                        new(t, s.W * 4, getLemmingBodyColor),
                        new(t, s.W * 5, getLemmingMiscColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateSprite(
            LemmingAction action,
            int numberOfLayers,
            Common.Point anchorPoint,
            GenerateLayers spriteLayerGenerator)
        {
            CreateActionSprites(
                contentManager,
                new Span<LemmingActionSprite>(actionSprites),
                action,
                numberOfLayers,
                anchorPoint,
                spriteLayerGenerator);
        }
    }

    private static void CreateActionSprites(
        ContentManager contentManager,
        Span<LemmingActionSprite> actionSprites,
        LemmingAction action,
        int numberOfLayers,
        Common.Point anchorPoint,
        GenerateLayers spriteLayerGenerator)
    {
        var texture = contentManager.Load<Texture2D>($"sprites/lemming/{action.LemmingActionSpriteFileName}");

        var spriteSize = new Size(
            texture.Width / numberOfLayers,
            texture.Height / action.NumberOfAnimationFrames);

        actionSprites[action.Id] = spriteLayerGenerator(texture, anchorPoint, spriteSize);
    }

    private static TeamColorData[] GenerateDefaultTeamColorData()
    {
        var result = new TeamColorData[EngineConstants.MaxNumberOfTeams];

        var defaultSkinColor = new Color(0xF0, 0xD0, 0xD0);
        var defaultAcidLemmingFootColor = new Color(0x00, 0xF0, 0x00);
        var defaultWaterLemmingFootColor = new Color(0x00, 0xF0, 0xF0);
        var defaultZombieSkinColor = new Color(0x77, 0x77, 0x77);
        var defaultNeutralBodyColor = new Color(0x99, 0x99, 0x99);

        var team0HairColor = new Color(0x04, 0xB0, 0x00);
        var team0BodyColor = new Color(0x40, 0x44, 0xDF);

        var team1HairColor = new Color(0x00, 0xB0, 0xA9);
        var team1BodyColor = new Color(0xD5, 0x3F, 0xDE);

        var team2HairColor = new Color(0x00, 0x04, 0xB0);
        var team2BodyColor = new Color(0xDE, 0x3F, 0x46);

        var team3HairColor = new Color(0xAD, 0x00, 0xB0);
        var team3BodyColor = new Color(0xDE, 0xD1, 0x3F);

        var team4HairColor = new Color(0xB0, 0x00, 0x00);
        var team4BodyColor = new Color(0x4A, 0xDE, 0x3F);

        var team5HairColor = new Color(0xB0, 0xA9, 0x00);
        var team5BodyColor = new Color(0x3F, 0xDE, 0xD5);

        result[0] = new TeamColorData
        {
            HairColor = team0HairColor,
            PermanentSkillHairColor = team0BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team0BodyColor,
            PermanentSkillBodyColor = team0HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        result[1] = new TeamColorData
        {
            HairColor = team1HairColor,
            PermanentSkillHairColor = team1BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team1BodyColor,
            PermanentSkillBodyColor = team1HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        result[2] = new TeamColorData
        {
            HairColor = team2HairColor,
            PermanentSkillHairColor = team2BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team2BodyColor,
            PermanentSkillBodyColor = team2HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        result[3] = new TeamColorData
        {
            HairColor = team3HairColor,
            PermanentSkillHairColor = team3BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team3BodyColor,
            PermanentSkillBodyColor = team3HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        result[4] = new TeamColorData
        {
            HairColor = team4HairColor,
            PermanentSkillHairColor = team4BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team4BodyColor,
            PermanentSkillBodyColor = team4HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        result[5] = new TeamColorData
        {
            HairColor = team5HairColor,
            PermanentSkillHairColor = team5BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = team5BodyColor,
            PermanentSkillBodyColor = team5HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,
        };

        return result;
    }
}