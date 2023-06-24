using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Actions;
using NeoLemmixSharp.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;
using static NeoLemmixSharp.Rendering.Level.Viewport.SpriteRotationReflectionProcessor;

namespace NeoLemmixSharp.Rendering.Level.Viewport.Lemming;

public static class DefaultLemmingSpriteBank
{
    public static LemmingSpriteBank DefaultLemmingSprites { get; private set; }

    public static void CreateDefaultLemmingSpriteBank(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch)
    {
        var spriteRotationReflectionProcessor = new SpriteRotationReflectionProcessor(graphicsDevice);
        var actionSprites =
            new ActionSprite[LemmingAction.AllActions.Count * 4 *
                             2]; // Number of actions * 4 orientations * 2 facing directions.

        CreateThreeLayerSprite(AscenderAction.Instance, new LevelPosition());
        CreateFourLayerSprite(BasherAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(BlockerAction.Instance, new LevelPosition());
        CreateFiveLayerTrueColourSprite(BuilderAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(ClimberAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(DehoisterAction.Instance, new LevelPosition());
        CreateFourLayerSprite(DiggerAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(DisarmerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(DrownerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(ExiterAction.Instance, new LevelPosition());
        CreateOneLayerTrueColourSprite(ExploderAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(FallerAction.Instance, new LevelPosition());
        CreateFiveLayerTrueColourSprite(FencerAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(FloaterAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(GliderAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(HoisterAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(JumperAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(LasererAction.Instance, new LevelPosition());
        CreateFourLayerSprite(MinerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(OhNoerAction.Instance, new LevelPosition());
        CreateFiveLayerTrueColourSprite(PlatformerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(ReacherAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(ShimmierAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(ShruggerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(SliderAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(SplatterAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(StackerAction.Instance, new LevelPosition());
        CreateOneLayerTrueColourSprite(StonerAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(SwimmerAction.Instance, new LevelPosition());
        CreateFourLayerTrueColourSprite(VaporiserAction.Instance, new LevelPosition());
        CreateThreeLayerSprite(WalkerAction.Instance, new LevelPosition());

        DefaultLemmingSprites = new LemmingSpriteBank(actionSprites);

        void CreateOneLayerTrueColourSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 1,
                (t, w, h, f, l, p) => new SingleColourLayerActionSprite(t, w, h, f, l, p));
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

        void CreateFourLayerTrueColourSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 4,
                (t, w, h, f, l, p) => new FourLayerColourActionSprite(t, w, h, f, l, p));
        }

        void CreateFiveLayerTrueColourSprite(LemmingAction action, LevelPosition anchorPoint)
        {
            CreateSprite(action, anchorPoint, 5,
                (t, w, h, f, l, p) => new FiveLayerColourActionSprite(t, w, h, f, l, p));
        }

        void CreateSprite(
            LemmingAction action,
            LevelPosition anchorPoint,
            int numberOfLayers,
            ActionSpriteCreator actionSpriteCreator)
        {
            CreateActionSprites(
                contentManager,
                spriteRotationReflectionProcessor,
                actionSprites,
                action,
                numberOfLayers,
                anchorPoint,
                actionSpriteCreator);
        }
    }

    private static void CreateActionSprites(
        ContentManager contentManager,
        SpriteRotationReflectionProcessor spriteRotationReflectionProcessor,
        ActionSprite[] actionSprites,
        LemmingAction action,
        int numberOfLayers,
        LevelPosition anchorPoint,
        ActionSpriteCreator actionSpriteCreator)
    {
        var texture = contentManager.Load<Texture2D>($"sprites/lemming/{action.LemmingActionName}");

        var spriteWidth = texture.Width / numberOfLayers;
        var spriteHeight = texture.Height / action.NumberOfAnimationFrames;

        var spritesTemp = spriteRotationReflectionProcessor.GenerateAllSpriteTypes(
            spriteWidth,
            spriteHeight,
            numberOfLayers,
            anchorPoint,
            actionSpriteCreator);

        foreach (var orientation in Orientation.AllOrientations)
        {
            var k0 = LemmingSpriteBank.GetKey(orientation, RightFacingDirection.Instance);
            var k1 = LemmingSpriteBank.GetKey(action, orientation, RightFacingDirection.Instance);

            actionSprites[k1] = spritesTemp[k0];

            k0 = LemmingSpriteBank.GetKey(orientation, LeftFacingDirection.Instance);
            k1 = LemmingSpriteBank.GetKey(action, orientation, LeftFacingDirection.Instance);

            actionSprites[k1] = spritesTemp[k0];
        }
    }
}