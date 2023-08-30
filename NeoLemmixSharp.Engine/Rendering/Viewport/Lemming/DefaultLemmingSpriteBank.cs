using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Teams;
using ActionSpriteCreator = NeoLemmixSharp.Engine.Rendering.Viewport.SpriteRotationReflectionProcessor<NeoLemmixSharp.Engine.Rendering.Viewport.Lemming.ActionSprite>;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

public static class DefaultLemmingSpriteBank
{
    public static LemmingSpriteBank DefaultLemmingSprites { get; private set; }

    public static void CreateDefaultLemmingSpriteBank(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        var spriteRotationReflectionProcessor = new ActionSpriteCreator(graphicsDevice);

        var numberOfActionSprites = LemmingAction.AllItems.Length *
                                    Orientation.AllItems.Length *
                                    FacingDirection.AllItems.Length;

        var actionSprites = new ActionSprite[numberOfActionSprites];

        CreateThreeLayerSprite(AscenderAction.Instance, new LevelPosition(2, 10));
        CreateFourLayerSprite(BasherAction.Instance, new LevelPosition(8, 10));
        CreateThreeLayerSprite(BlockerAction.Instance, new LevelPosition(5, 13));
        CreateFiveLayerTrueColorSprite(BuilderAction.Instance, new LevelPosition(3, 13));
        CreateThreeLayerSprite(ClimberAction.Instance, new LevelPosition(8, 12));
        CreateThreeLayerSprite(DehoisterAction.Instance, new LevelPosition(5, 13));
        CreateFourLayerSprite(DiggerAction.Instance, new LevelPosition(7, 12));
        CreateFourLayerTrueColorSprite(DisarmerAction.Instance, new LevelPosition(1, 11));
        CreateThreeLayerSprite(DrownerAction.Instance, new LevelPosition(5, 10));
        CreateThreeLayerSprite(ExiterAction.Instance, new LevelPosition(7, 16));
        CreateOneLayerTrueColorSprite(ExploderAction.Instance, new LevelPosition(16, 25));
        CreateThreeLayerSprite(FallerAction.Instance, new LevelPosition(2, 11));
        CreateFiveLayerTrueColorSprite(FencerAction.Instance, new LevelPosition(5, 10));
        CreateFourLayerTrueColorSprite(FloaterAction.Instance, new LevelPosition(4, 16));
        CreateFourLayerTrueColorSprite(GliderAction.Instance, new LevelPosition(5, 16));
        CreateThreeLayerSprite(HoisterAction.Instance, new LevelPosition(5, 12));
        CreateThreeLayerSprite(JumperAction.Instance, new LevelPosition(2, 10));
        CreateFourLayerTrueColorSprite(LasererAction.Instance, new LevelPosition(3, 10));
        CreateFourLayerSprite(MinerAction.Instance, new LevelPosition(7, 13));
        CreateThreeLayerSprite(OhNoerAction.Instance, new LevelPosition(3, 10));
        CreateFiveLayerTrueColorSprite(PlatformerAction.Instance, new LevelPosition(3, 13));
        CreateThreeLayerSprite(ReacherAction.Instance, new LevelPosition(3, 10));
        CreateThreeLayerSprite(ShimmierAction.Instance, new LevelPosition(3, 8));
        CreateThreeLayerSprite(ShruggerAction.Instance, new LevelPosition(3, 10));
        CreateThreeLayerSprite(SliderAction.Instance, new LevelPosition(4, 11));
        CreateThreeLayerSprite(SplatterAction.Instance, new LevelPosition(7, 10));
        CreateFourLayerTrueColorSprite(StackerAction.Instance, new LevelPosition(5, 13));
        CreateOneLayerTrueColorSprite(StonerAction.Instance, new LevelPosition(16, 25));
        CreateThreeLayerSprite(SwimmerAction.Instance, new LevelPosition(6, 8));
        CreateFourLayerTrueColorSprite(VaporiserAction.Instance, new LevelPosition(5, 14));
        CreateThreeLayerSprite(WalkerAction.Instance, new LevelPosition(2, 10));

        var teamColorData = GenerateDefaultTeamColorData();

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites, teamColorData);

        void CreateOneLayerTrueColorSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 1,
                (t, w, h, f, l, p) => new SingleColorLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateThreeLayerSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 3,
                (t, w, h, f, l, p) => new ThreeLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateFourLayerSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 4,
                (t, w, h, f, l, p) => new FourLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateFourLayerTrueColorSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 4,
                (t, w, h, f, l, p) => new FourLayerColorActionSprite(t, w, h, f, l, p));
        }

        void CreateFiveLayerTrueColorSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 5,
                (t, w, h, f, l, p) => new FiveLayerColorActionSprite(t, w, h, f, l, p));
        }

        void CreateSprite(
            LemmingAction action,
            LevelPosition anchorPoint,
            int numberOfLayers,
            ActionSpriteCreator.ItemCreator actionSpriteCreator)
        {
            CreateActionSprites(
                contentManager,
                spriteRotationReflectionProcessor,
                new Span<ActionSprite>(actionSprites),
                action,
                numberOfLayers,
                anchorPoint,
                actionSpriteCreator);
        }
    }

    private static void CreateActionSprites(
        ContentManager contentManager,
        ActionSpriteCreator spriteRotationReflectionProcessor,
        Span<ActionSprite> actionSprites,
        LemmingAction action,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ActionSpriteCreator.ItemCreator itemCreator)
    {
        using var texture = contentManager.Load<Texture2D>($"sprites/lemming/{action.LemmingActionName}");

        var spriteWidth = texture.Width / numberOfLayers;
        var spriteHeight = texture.Height / action.NumberOfAnimationFrames;

        var spritesTemp = spriteRotationReflectionProcessor.CreateAllSpriteTypes(
            texture,
            spriteWidth,
            spriteHeight,
            action.NumberOfAnimationFrames,
            numberOfLayers,
            anchorPoint,
            itemCreator);

        foreach (var orientation in Orientation.AllItems)
        {
            var k0 = LemmingSpriteBank.GetKey(orientation, RightFacingDirection.Instance);
            var k1 = LemmingSpriteBank.GetKey(action, orientation, RightFacingDirection.Instance);

            actionSprites[k1] = spritesTemp[k0];

            k0 = LemmingSpriteBank.GetKey(orientation, LeftFacingDirection.Instance);
            k1 = LemmingSpriteBank.GetKey(action, orientation, LeftFacingDirection.Instance);

            actionSprites[k1] = spritesTemp[k0];
        }
    }

    private static TeamColorData[] GenerateDefaultTeamColorData()
    {
        var result = new TeamColorData[GameConstants.NumberOfTeams];

        var defaultSkinColor = new Color(0xF0, 0xD0, 0xD0);
        var defaultZombieSkinColor = new Color(0x77, 0x77, 0x77);
        var defaultNeutralBodyColor = new Color(0x99, 0x99, 0x99);

        result[0] = new TeamColorData
        {
            HairColor = new Color(0x04, 0xB0, 0x00),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0x40, 0x44, 0xDF),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        result[1] = new TeamColorData
        {
            HairColor = new Color(0x00, 0xB0, 0xA9),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0xD5, 0x3F, 0xDE),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        result[2] = new TeamColorData
        {
            HairColor = new Color(0x00, 0x04, 0xB0),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0xDE, 0x3F, 0x46),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        result[3] = new TeamColorData
        {
            HairColor = new Color(0xAD, 0x00, 0xB0),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0xDE, 0xD1, 0x3F),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        result[4] = new TeamColorData
        {
            HairColor = new Color(0xB0, 0x00, 0x00),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0x4A, 0xDE, 0x3F),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        result[5] = new TeamColorData
        {
            HairColor = new Color(0xB0, 0xA9, 0x00),
            SkinColor = defaultSkinColor,
            ZombieSkinColor = defaultZombieSkinColor,
            BodyColor = new Color(0x3F, 0xDE, 0xD5),
            NeutralBodyColor = defaultNeutralBodyColor
        };

        return result;
    }
}