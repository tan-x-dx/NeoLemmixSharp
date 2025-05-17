using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Tribes;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public static class DefaultLemmingSpriteBank
{
    private delegate LemmingActionSprite GenerateLayers(Texture2D texture, Point anchorPoint, Size spriteSize);

    public static LemmingSpriteBank DefaultLemmingSprites { get; private set; } = null!;

    public static void CreateDefaultLemmingSpriteBank(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        LemmingActionSprite.Initialise(graphicsDevice);

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

        var tribeColorData = GenerateDefaultTribeColorData();

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites, tribeColorData);

        return;

        void CreateOneLayerTrueColorSprite(LemmingAction action, Point anchorPoint)
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

        void CreateFourLayerSprite(LemmingAction action, Point anchorPoint)
        {
            CreateSprite(
                action,
                4,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, TribeColorChooser.GetHairColor),
                        new(t, s.W, TribeColorChooser.GetSkinColor),
                        new(t, s.W * 2, TribeColorChooser.GetFootColor),
                        new(t, s.W * 3, TribeColorChooser.GetBodyColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateFiveLayerSprite(LemmingAction action, Point anchorPoint)
        {
            CreateSprite(
                action,
                5,
                anchorPoint,
                (t, p, s) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, TribeColorChooser.GetHairColor),
                        new(t, s.W, TribeColorChooser.GetSkinColor),
                        new(t, s.W * 2, TribeColorChooser.GetFootColor),
                        new(t, s.W * 3, TribeColorChooser.GetBodyColor),
                        new(t, s.W * 4, TribeColorChooser.GetPaintColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateFiveLayerTrueColorSprite(LemmingAction action, Point anchorPoint)
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
                        new(t, s.W, TribeColorChooser.GetHairColor),
                        new(t, s.W * 2, TribeColorChooser.GetSkinColor),
                        new(t, s.W * 3, TribeColorChooser.GetFootColor),
                        new(t, s.W * 4, TribeColorChooser.GetBodyColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateSixLayerTrueColorSprite(LemmingAction action, Point anchorPoint)
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
                        new(t, s.W, TribeColorChooser.GetHairColor),
                        new(t, s.W * 2, TribeColorChooser.GetSkinColor),
                        new(t, s.W * 3, TribeColorChooser.GetFootColor),
                        new(t, s.W * 4, TribeColorChooser.GetBodyColor),
                        new(t, s.W * 5, TribeColorChooser.GetPaintColor)
                    };

                    return new LemmingActionSprite(t, p, s, layerRenderers);
                });
        }

        void CreateSprite(
            LemmingAction action,
            int numberOfLayers,
            Point anchorPoint,
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
        Point anchorPoint,
        GenerateLayers spriteLayerGenerator)
    {
        var texture = contentManager.Load<Texture2D>($"sprites/lemming/{action.LemmingActionSpriteFileName}");

        var spriteSize = new Size(
            texture.Width / numberOfLayers,
            texture.Height / action.NumberOfAnimationFrames);

        actionSprites[action.Id] = spriteLayerGenerator(texture, anchorPoint, spriteSize);
    }

    private static TribeColorData[] GenerateDefaultTribeColorData()
    {
        var result = new TribeColorData[EngineConstants.MaxNumberOfTribes];

        var defaultSkinColor = new Color(0xF0, 0xD0, 0xD0);
        var defaultAcidLemmingFootColor = new Color(0x00, 0xF0, 0x00);
        var defaultWaterLemmingFootColor = new Color(0x00, 0xF0, 0xF0);
        var defaultZombieSkinColor = new Color(0x77, 0x77, 0x77);
        var defaultNeutralBodyColor = new Color(0x99, 0x99, 0x99);

        var defaultPaintColor = new Color(0xff, 0x00, 0xff);

        var tribe0HairColor = new Color(0x04, 0xB0, 0x00);
        var tribe0BodyColor = new Color(0x40, 0x44, 0xDF);

        var tribe1HairColor = new Color(0x00, 0xB0, 0xA9);
        var tribe1BodyColor = new Color(0xD5, 0x3F, 0xDE);

        var tribe2HairColor = new Color(0x00, 0x04, 0xB0);
        var tribe2BodyColor = new Color(0xDE, 0x3F, 0x46);

        var tribe3HairColor = new Color(0xAD, 0x00, 0xB0);
        var tribe3BodyColor = new Color(0xDE, 0xD1, 0x3F);

        var tribe4HairColor = new Color(0xB0, 0x00, 0x00);
        var tribe4BodyColor = new Color(0x4A, 0xDE, 0x3F);

        var tribe5HairColor = new Color(0xB0, 0xA9, 0x00);
        var tribe5BodyColor = new Color(0x3F, 0xDE, 0xD5);

        result[0] = new TribeColorData
        {
            HairColor = tribe0HairColor,
            PermanentSkillHairColor = tribe0BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe0BodyColor,
            PermanentSkillBodyColor = tribe0HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        result[1] = new TribeColorData
        {
            HairColor = tribe1HairColor,
            PermanentSkillHairColor = tribe1BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe1BodyColor,
            PermanentSkillBodyColor = tribe1HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        result[2] = new TribeColorData
        {
            HairColor = tribe2HairColor,
            PermanentSkillHairColor = tribe2BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe2BodyColor,
            PermanentSkillBodyColor = tribe2HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        result[3] = new TribeColorData
        {
            HairColor = tribe3HairColor,
            PermanentSkillHairColor = tribe3BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe3BodyColor,
            PermanentSkillBodyColor = tribe3HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        result[4] = new TribeColorData
        {
            HairColor = tribe4HairColor,
            PermanentSkillHairColor = tribe4BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe4BodyColor,
            PermanentSkillBodyColor = tribe4HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        result[5] = new TribeColorData
        {
            HairColor = tribe5HairColor,
            PermanentSkillHairColor = tribe5BodyColor,
            SkinColor = defaultSkinColor,
            AcidLemmingFootColor = defaultAcidLemmingFootColor,
            WaterLemmingFootColor = defaultWaterLemmingFootColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = tribe5BodyColor,
            PermanentSkillBodyColor = tribe5HairColor,
            NeutralBodyColor = defaultNeutralBodyColor,

            PaintColor = defaultPaintColor
        };

        return result;
    }
}