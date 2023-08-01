using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;
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

        var numberOfActionSprites = LemmingAction.AllLemmingActions.Length *
                                    Orientation.AllOrientations.Length *
                                    FacingDirection.AllFacingDirections.Length;

        var actionSprites = new ActionSprite[numberOfActionSprites];

        CreateThreeLayerSprite(AscenderAction.Instance, new LevelPosition(2, 10));
        CreateFourLayerSprite(BasherAction.Instance, new LevelPosition(8, 10));
        CreateThreeLayerSprite(BlockerAction.Instance, new LevelPosition(5, 13));
        CreateFiveLayerTrueColourSprite(BuilderAction.Instance, new LevelPosition(3, 13));
        CreateThreeLayerSprite(ClimberAction.Instance, new LevelPosition(8, 12));
        CreateThreeLayerSprite(DehoisterAction.Instance, new LevelPosition(5, 13));
        CreateFourLayerSprite(DiggerAction.Instance, new LevelPosition(7, 12));
        CreateFourLayerTrueColourSprite(DisarmerAction.Instance, new LevelPosition(1, 11));
        CreateThreeLayerSprite(DrownerAction.Instance, new LevelPosition(5, 10));
        CreateThreeLayerSprite(ExiterAction.Instance, new LevelPosition(7, 16));
        CreateOneLayerTrueColourSprite(ExploderAction.Instance, new LevelPosition(16, 25));
        CreateThreeLayerSprite(FallerAction.Instance, new LevelPosition(2, 11));
        CreateFiveLayerTrueColourSprite(FencerAction.Instance, new LevelPosition(5, 10));
        CreateFourLayerTrueColourSprite(FloaterAction.Instance, new LevelPosition(4, 16));
        CreateFourLayerTrueColourSprite(GliderAction.Instance, new LevelPosition(5, 16));
        CreateThreeLayerSprite(HoisterAction.Instance, new LevelPosition(5, 12));
        CreateThreeLayerSprite(JumperAction.Instance, new LevelPosition(2, 10));
        CreateFourLayerTrueColourSprite(LasererAction.Instance, new LevelPosition(3, 10));
        CreateFourLayerSprite(MinerAction.Instance, new LevelPosition(7, 13));
        CreateThreeLayerSprite(OhNoerAction.Instance, new LevelPosition(3, 10));
        CreateFiveLayerTrueColourSprite(PlatformerAction.Instance, new LevelPosition(3, 13));
        CreateThreeLayerSprite(ReacherAction.Instance, new LevelPosition(3, 10));
        CreateThreeLayerSprite(ShimmierAction.Instance, new LevelPosition(3, 8));
        CreateThreeLayerSprite(ShruggerAction.Instance, new LevelPosition(3, 10));
        CreateThreeLayerSprite(SliderAction.Instance, new LevelPosition(4, 11));
        CreateThreeLayerSprite(SplatterAction.Instance, new LevelPosition(7, 10));
        CreateFourLayerTrueColourSprite(StackerAction.Instance, new LevelPosition(5, 13));
        CreateOneLayerTrueColourSprite(StonerAction.Instance, new LevelPosition(16, 25));
        CreateThreeLayerSprite(SwimmerAction.Instance, new LevelPosition(6, 8));
        CreateFourLayerTrueColourSprite(VaporiserAction.Instance, new LevelPosition(5, 14));
        CreateThreeLayerSprite(WalkerAction.Instance, new LevelPosition(2, 10));

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