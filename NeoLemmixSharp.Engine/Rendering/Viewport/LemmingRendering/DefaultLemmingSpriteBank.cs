using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using ActionSpriteCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering.LemmingActionSprite>;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public static class DefaultLemmingSpriteBank
{
    public static LemmingSpriteBank DefaultLemmingSprites { get; private set; } = null!;

    public static void CreateDefaultLemmingSpriteBank(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        LemmingActionSprite.Initialise(graphicsDevice);

        var spriteRotationReflectionProcessor = new ActionSpriteCreator(graphicsDevice);

#pragma warning disable IDE0039
        LemmingActionLayerRenderer.GetLemmingColor getLemmingHairColor = l => l.State.HairColor;
        LemmingActionLayerRenderer.GetLemmingColor getLemmingSkinColor = l => l.State.SkinColor;
        LemmingActionLayerRenderer.GetLemmingColor getLemmingFootColor = l => l.State.FootColor;
        LemmingActionLayerRenderer.GetLemmingColor getLemmingBodyColor = l => l.State.BodyColor;
        LemmingActionLayerRenderer.GetLemmingColor getLemmingMiscColor = _ => Color.Magenta;
#pragma warning restore IDE0039

        var numberOfActionSprites = LemmingAction.NumberOfItems *
                                    Orientation.NumberOfItems *
                                    FacingDirection.NumberOfItems;

        var actionSprites = new LemmingActionSprite[numberOfActionSprites];

        CreateFourLayerSprite(AscenderAction.Instance, new LevelPosition(2, 10));
        CreateFiveLayerSprite(BasherAction.Instance, new LevelPosition(8, 10));
        CreateFourLayerSprite(BlockerAction.Instance, new LevelPosition(5, 13));
        CreateSixLayerTrueColorSprite(BuilderAction.Instance, new LevelPosition(3, 13));
        CreateFourLayerSprite(ClimberAction.Instance, new LevelPosition(8, 12));
        CreateFourLayerSprite(DehoisterAction.Instance, new LevelPosition(5, 13));
        CreateFiveLayerSprite(DiggerAction.Instance, new LevelPosition(7, 12));
        CreateFiveLayerTrueColorSprite(DisarmerAction.Instance, new LevelPosition(1, 11));
        CreateFourLayerSprite(DrownerAction.Instance, new LevelPosition(5, 10));
        CreateFourLayerSprite(ExiterAction.Instance, new LevelPosition(2, 16));
        CreateOneLayerTrueColorSprite(ExploderAction.Instance, new LevelPosition(17, 21));
        CreateFourLayerSprite(FallerAction.Instance, new LevelPosition(3, 10));
        CreateSixLayerTrueColorSprite(FencerAction.Instance, new LevelPosition(3, 10));
        CreateFiveLayerTrueColorSprite(FloaterAction.Instance, new LevelPosition(4, 16));
        CreateFiveLayerTrueColorSprite(GliderAction.Instance, new LevelPosition(5, 16));
        CreateFourLayerSprite(HoisterAction.Instance, new LevelPosition(5, 12));
        CreateFourLayerSprite(JumperAction.Instance, new LevelPosition(2, 10));
        CreateFiveLayerTrueColorSprite(LasererAction.Instance, new LevelPosition(3, 10));
        CreateFiveLayerSprite(MinerAction.Instance, new LevelPosition(7, 13));
        CreateFourLayerSprite(OhNoerAction.Instance, new LevelPosition(3, 10));
        CreateSixLayerTrueColorSprite(PlatformerAction.Instance, new LevelPosition(3, 13));
        CreateFourLayerSprite(ReacherAction.Instance, new LevelPosition(3, 9));
        CreateFourLayerSprite(ShimmierAction.Instance, new LevelPosition(3, 8));
        CreateFourLayerSprite(ShruggerAction.Instance, new LevelPosition(3, 10));
        CreateFourLayerSprite(SliderAction.Instance, new LevelPosition(4, 11));
        CreateFourLayerSprite(SplatterAction.Instance, new LevelPosition(7, 10));
        CreateSixLayerTrueColorSprite(StackerAction.Instance, new LevelPosition(3, 13));
        CreateOneLayerTrueColorSprite(StonerAction.Instance, new LevelPosition(17, 21));
        CreateFourLayerSprite(SwimmerAction.Instance, new LevelPosition(6, 8));
        CreateFiveLayerTrueColorSprite(VaporiserAction.Instance, new LevelPosition(5, 14));
        CreateFourLayerSprite(WalkerAction.Instance, new LevelPosition(2, 10));
        //CreateFourLayerSprite(RotateClockwiseAction.Instance, new LevelPosition(2, 10));
        //CreateFourLayerSprite(RotateCounterclockwiseAction.Instance, new LevelPosition(2, 10));
        //CreateFourLayerSprite(RotateHalfAction.Instance, new LevelPosition(2, 10));
        CreateLemmingRotationSprites(
            contentManager,
            spriteRotationReflectionProcessor,
            new Span<LemmingActionSprite>(actionSprites),

            getLemmingHairColor,
            getLemmingSkinColor,
            getLemmingFootColor,
            getLemmingBodyColor);

        var teamColorData = GenerateDefaultTeamColorData();

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites, teamColorData);

        foreach (var team in Team.AllItems)
        {
            team.SetSpriteBank(DefaultLemmingSprites);
            DefaultLemmingSprites.SetTeamColors(team);
        }

        return;

        void CreateOneLayerTrueColorSprite(LemmingAction action, LevelPosition levelPosition)
        {
            CreateSprite(
                action,
                1,
                levelPosition,
                (t, w, h, _, p) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t)
                    };

                    return new LemmingActionSprite(t, p, w, h, layerRenderers);
                });
        }

        void CreateFourLayerSprite(LemmingAction action, LevelPosition levelPosition)
        {
            CreateSprite(
                action,
                4,
                levelPosition,
                (t, w, h, _, p) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, getLemmingHairColor),
                        new(t, w, getLemmingSkinColor),
                        new(t, w * 2, getLemmingFootColor),
                        new(t, w * 3, getLemmingBodyColor)
                    };

                    return new LemmingActionSprite(t, p, w, h, layerRenderers);
                });
        }

        void CreateFiveLayerSprite(LemmingAction action, LevelPosition levelPosition)
        {
            CreateSprite(
                action,
                5,
                levelPosition,
                (t, w, h, _, p) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t, 0, getLemmingHairColor),
                        new(t, w, getLemmingSkinColor),
                        new(t, w * 2, getLemmingFootColor),
                        new(t, w * 3, getLemmingBodyColor),
                        new(t, w * 4, getLemmingMiscColor)
                    };

                    return new LemmingActionSprite(t, p, w, h, layerRenderers);
                });
        }

        void CreateFiveLayerTrueColorSprite(LemmingAction action, LevelPosition levelPosition)
        {
            CreateSprite(
                action,
                5,
                levelPosition,
                (t, w, h, _, p) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t),
                        new(t, w, getLemmingHairColor),
                        new(t, w * 2, getLemmingSkinColor),
                        new(t, w * 3, getLemmingFootColor),
                        new(t, w * 4, getLemmingBodyColor)
                    };

                    return new LemmingActionSprite(t, p, w, h, layerRenderers);
                });
        }

        void CreateSixLayerTrueColorSprite(LemmingAction action, LevelPosition levelPosition)
        {
            CreateSprite(
                action,
                6,
                levelPosition,
                (t, w, h, _, p) =>
                {
                    var layerRenderers = new LemmingActionLayerRenderer[]
                    {
                        new(t),
                        new(t, w, getLemmingHairColor),
                        new(t, w * 2, getLemmingSkinColor),
                        new(t, w * 3, getLemmingFootColor),
                        new(t, w * 4, getLemmingBodyColor),
                        new(t, w * 5, getLemmingMiscColor)
                    };

                    return new LemmingActionSprite(t, p, w, h, layerRenderers);
                });
        }

        void CreateSprite(
            LemmingAction action,
            int numberOfLayers,
            LevelPosition levelPosition,
            ActionSpriteCreator.ItemCreator actionSpriteCreator)
        {
            CreateActionSprites(
                contentManager,
                spriteRotationReflectionProcessor,
                new Span<LemmingActionSprite>(actionSprites),
                action,
                numberOfLayers,
                levelPosition,
                actionSpriteCreator);
        }
    }

    private static void CreateActionSprites(
        ContentManager contentManager,
        ActionSpriteCreator spriteRotationReflectionProcessor,
        Span<LemmingActionSprite> actionSprites,
        LemmingAction action,
        int numberOfLayers,
        LevelPosition levelPosition,
        ActionSpriteCreator.ItemCreator itemCreator)
    {
        var spritesTemp = CreateSpriteTypesArray(contentManager, spriteRotationReflectionProcessor, action, numberOfLayers, levelPosition, itemCreator);

        RegisterSprites(actionSprites, action, spritesTemp);
    }

    private static LemmingActionSprite[] CreateSpriteTypesArray(
        ContentManager contentManager,
        ActionSpriteCreator spriteRotationReflectionProcessor,
        LemmingAction action,
        int numberOfLayers,
        LevelPosition levelPosition,
        ActionSpriteCreator.ItemCreator itemCreator)
    {
        using var texture = contentManager.Load<Texture2D>($"sprites/lemming/{action.LemmingActionSpriteFileName}");

        var spriteWidth = texture.Width / numberOfLayers;
        var spriteHeight = texture.Height / action.NumberOfAnimationFrames;

        return spriteRotationReflectionProcessor.CreateAllSpriteTypes(
            texture,
            spriteWidth,
            spriteHeight,
            action.NumberOfAnimationFrames,
            numberOfLayers,
            levelPosition,
            itemCreator);
    }

    private static void RegisterSprites(
        Span<LemmingActionSprite> actionSprites,
        LemmingAction action,
        LemmingActionSprite[] spriteTypes)
    {
        foreach (var orientation in Orientation.AllItems)
        {
            var k0 = LemmingSpriteBank.GetKey(orientation, FacingDirection.RightInstance);
            var k1 = LemmingSpriteBank.GetKey(action, orientation, FacingDirection.RightInstance);

            actionSprites[k1] = spriteTypes[k0];

            k0 = LemmingSpriteBank.GetKey(orientation, FacingDirection.LeftInstance);
            k1 = LemmingSpriteBank.GetKey(action, orientation, FacingDirection.LeftInstance);

            actionSprites[k1] = spriteTypes[k0];
        }
    }

    private static TeamColorData[] GenerateDefaultTeamColorData()
    {
        var result = new TeamColorData[EngineConstants.NumberOfTeams];

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

    private static void CreateLemmingRotationSprites(
        ContentManager contentManager,
        ActionSpriteCreator spriteRotationReflectionProcessor,
        Span<LemmingActionSprite> actionSprites,
        LemmingActionLayerRenderer.GetLemmingColor getLemmingHairColor,
        LemmingActionLayerRenderer.GetLemmingColor getLemmingSkinColor,
        LemmingActionLayerRenderer.GetLemmingColor getLemmingFootColor,
        LemmingActionLayerRenderer.GetLemmingColor getLemmingBodyColor)
    {
        var rotateClockwiseSprites = CreateSpriteTypesArray(
            contentManager,
            spriteRotationReflectionProcessor,
            RotateClockwiseAction.Instance,
            4,
            new LevelPosition(9, 13),
            ItemCreator);

        var rotateCounterclockwiseSprites = CreateSpriteTypesArray(
            contentManager,
            spriteRotationReflectionProcessor,
            RotateCounterclockwiseAction.Instance,
            4,
            new LevelPosition(9, 13),
            ItemCreator);

        var rotateHalfSprites = CreateSpriteTypesArray(
            contentManager,
            spriteRotationReflectionProcessor,
            RotateHalfAction.Instance,
            4,
            new LevelPosition(9, 13),
            ItemCreator);

        foreach (var orientation in Orientation.AllItems)
        {
            var rotateCwK0 = LemmingSpriteBank.GetKey(orientation, FacingDirection.RightInstance);
            var rotateCwK1 = LemmingSpriteBank.GetKey(RotateClockwiseAction.Instance, orientation, FacingDirection.RightInstance);
            var rotateCcwK1 = LemmingSpriteBank.GetKey(RotateCounterclockwiseAction.Instance, orientation, FacingDirection.RightInstance);
            var rotateHalfK1 = LemmingSpriteBank.GetKey(RotateHalfAction.Instance, orientation, FacingDirection.RightInstance);

            (rotateCwK1, rotateCcwK1) = (rotateCcwK1, rotateCwK1);

            actionSprites[rotateCwK1] = rotateClockwiseSprites[rotateCwK0];
            actionSprites[rotateCcwK1] = rotateCounterclockwiseSprites[rotateCwK0];
            actionSprites[rotateHalfK1] = rotateHalfSprites[rotateCwK0];

            rotateCwK0 = LemmingSpriteBank.GetKey(orientation, FacingDirection.LeftInstance);
            rotateCwK1 = LemmingSpriteBank.GetKey(RotateClockwiseAction.Instance, orientation, FacingDirection.LeftInstance);
            rotateCcwK1 = LemmingSpriteBank.GetKey(RotateCounterclockwiseAction.Instance, orientation, FacingDirection.LeftInstance);
            rotateHalfK1 = LemmingSpriteBank.GetKey(RotateHalfAction.Instance, orientation, FacingDirection.LeftInstance);

            actionSprites[rotateCwK1] = rotateClockwiseSprites[rotateCwK0];
            actionSprites[rotateCcwK1] = rotateCounterclockwiseSprites[rotateCwK0];
            actionSprites[rotateHalfK1] = rotateHalfSprites[rotateCwK0];
        }

        return;

        LemmingActionSprite ItemCreator(Texture2D t, int w, int h, int _, LevelPosition p)
        {
            var layerRenderers = new LemmingActionLayerRenderer[]
            {
                new(t, 0, getLemmingHairColor),
                new(t, w, getLemmingSkinColor),
                new(t, w * 2, getLemmingFootColor),
                new(t, w * 3, getLemmingBodyColor)
            };

            return new LemmingActionSprite(t, p, w, h, layerRenderers);
        }
    }
}