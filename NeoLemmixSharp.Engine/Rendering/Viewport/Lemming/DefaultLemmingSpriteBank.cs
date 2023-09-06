using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
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

        CreateThreeLayerSprite(AscenderAction.Instance);
        CreateFourLayerSprite(BasherAction.Instance);
        CreateThreeLayerSprite(BlockerAction.Instance);
        CreateFiveLayerTrueColorSprite(BuilderAction.Instance);
        CreateThreeLayerSprite(ClimberAction.Instance);
        CreateThreeLayerSprite(DehoisterAction.Instance);
        CreateFourLayerSprite(DiggerAction.Instance);
        CreateFourLayerTrueColorSprite(DisarmerAction.Instance);
        CreateThreeLayerSprite(DrownerAction.Instance);
        CreateThreeLayerSprite(ExiterAction.Instance);
        CreateOneLayerTrueColorSprite(ExploderAction.Instance);
        CreateThreeLayerSprite(FallerAction.Instance);
        CreateFiveLayerTrueColorSprite(FencerAction.Instance);
        CreateFourLayerTrueColorSprite(FloaterAction.Instance);
        CreateFourLayerTrueColorSprite(GliderAction.Instance);
        CreateThreeLayerSprite(HoisterAction.Instance);
        CreateThreeLayerSprite(JumperAction.Instance);
        CreateFourLayerTrueColorSprite(LasererAction.Instance);
        CreateFourLayerSprite(MinerAction.Instance);
        CreateThreeLayerSprite(OhNoerAction.Instance);
        CreateFiveLayerTrueColorSprite(PlatformerAction.Instance);
        CreateThreeLayerSprite(ReacherAction.Instance);
        CreateThreeLayerSprite(ShimmierAction.Instance);
        CreateThreeLayerSprite(ShruggerAction.Instance);
        CreateThreeLayerSprite(SliderAction.Instance);
        CreateThreeLayerSprite(SplatterAction.Instance);
        CreateFourLayerTrueColorSprite(StackerAction.Instance);
        CreateOneLayerTrueColorSprite(StonerAction.Instance);
        CreateThreeLayerSprite(SwimmerAction.Instance);
        CreateFourLayerTrueColorSprite(VaporiserAction.Instance);
        CreateThreeLayerSprite(WalkerAction.Instance);

        var teamColorData = GenerateDefaultTeamColorData();

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites, teamColorData);

        void CreateOneLayerTrueColorSprite(LemmingAction action)
        {
            CreateSprite(action, 1,
                (t, w, h, f, l, p) => new SingleColorLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateThreeLayerSprite(LemmingAction action)
        {
            CreateSprite(action, 3,
                (t, w, h, f, l, p) => new ThreeLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateFourLayerSprite(LemmingAction action)
        {
            CreateSprite(action, 4,
                (t, w, h, f, l, p) => new FourLayerActionSprite(t, w, h, f, l, p));
        }

        void CreateFourLayerTrueColorSprite(LemmingAction action)
        {
            CreateSprite(action, 4,
                (t, w, h, f, l, p) => new FourLayerColorActionSprite(t, w, h, f, l, p));
        }

        void CreateFiveLayerTrueColorSprite(LemmingAction action)
        {
            CreateSprite(action, 5,
                (t, w, h, f, l, p) => new FiveLayerColorActionSprite(t, w, h, f, l, p));
        }

        void CreateSprite(
            LemmingAction action,
            int numberOfLayers,
            ActionSpriteCreator.ItemCreator actionSpriteCreator)
        {
            CreateActionSprites(
                contentManager,
                spriteRotationReflectionProcessor,
                new Span<ActionSprite>(actionSprites),
                action,
                numberOfLayers,
                actionSpriteCreator);
        }
    }

    private static void CreateActionSprites(
        ContentManager contentManager,
        ActionSpriteCreator spriteRotationReflectionProcessor,
        Span<ActionSprite> actionSprites,
        LemmingAction action,
        int numberOfLayers,
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
            action.GetAnchorPosition(),
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